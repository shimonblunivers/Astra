using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class ShipEditor : Node2D
{
    public static ShipEditor Instance { get; private set; }
    public static TextureRect ToolPreview { get; private set; }

    [Export] public Console ConsoleLog { get; set; }
    [Export] public TileMapLayer WallTileMap { get; set; }
    [Export] public TileMapLayer ObjectTileMap { get; set; }

    public Inventory Inventory { get; set; }

    public static readonly Dictionary<int, string> Directions = new Dictionary<int, string>
    {
        { 0, "right" },
        { 1, "down" },
        { 2, "left" },
        { 3, "up" }
    };

    public static int Direction { get; set; } = 0;
    public static bool Autoflooring { get; set; } = false;

    public static Dictionary<string, Tool> Tools { get; } = new Dictionary<string, Tool>();
    public static Tool CurrentTool { get; set; } = null;

    public static int CurrentShipPrice { get; set; } = 0;
    public static Vector2 StartingBlockCoords { get; set; } = Vector2.Zero;

    public override void _Ready()
    {
        Instance = this;

        ConsoleLog ??= GetNodeOrNull<Console>("../HUD/Console/ConsoleLog");
        WallTileMap ??= GetNode<TileMapLayer>("WallTileMap");
        ObjectTileMap ??= GetNode<TileMapLayer>("ObjectTileMap");
        ToolPreview ??= GetNode<TextureRect>("../HUD/ToolPreview");

        LoadTools();
        ChangeTool("wall");

        Callable.From(UpdatePreview).CallDeferred();
    }

    public void EvideTiles()
    {
        StartingBlockCoords = Vector2.Zero;
        foreach (var toolEntry in Tools.Values)
        {
            toolEntry.NumberOfInstances = 0;
        }

        CurrentShipPrice = 0;

        var shipValidator = (Node)GetNode("/root/ShipValidator");

        foreach (Vector2I coords in WallTileMap.GetUsedCells())
        {
            string type = (string)shipValidator.Call("get_tile_type", WallTileMap, coords);
            if (type == "connector")
            {
                StartingBlockCoords = new Vector2(coords.X, coords.Y) * 32f + new Vector2(16f, 16f) + GlobalPosition;
            }

            if (!string.IsNullOrEmpty(type) && Tools.TryGetValue(type, out var foundTool))
            {
                foundTool.NumberOfInstances++;
                CurrentShipPrice += foundTool.Price;
            }
        }

        foreach (Vector2I coords in ObjectTileMap.GetUsedCells())
        {
            string type = (string)shipValidator.Call("get_tile_type", ObjectTileMap, coords);
            if (!string.IsNullOrEmpty(type) && Tools.TryGetValue(type, out var foundTool))
            {
                foundTool.NumberOfInstances++;
                CurrentShipPrice += foundTool.Price;
            }
        }
    }

    public void UpdatePreview()
    {
        if (Editor.Instance?.DirectionLabel != null && Directions.TryGetValue(Direction, out string dirName))
        {
            Editor.Instance.DirectionLabel.Text = "Direction: " + dirName;
        }
        UpdatePreviewRotation();
    }

    public void LoadTools()
    {
        const string path = "res://Editor/Tools";
        using var dir = DirAccess.Open(path);
        if (dir == null) return;

        dir.ListDirBegin();
        string fileName = dir.GetNext();

        while (!string.IsNullOrEmpty(fileName))
        {
            if (!dir.CurrentIsDir())
            {
                if (fileName.Contains(".tres.remap"))
                {
                    fileName = fileName.TrimSuffix(".remap");
                }

                if (fileName.Contains(".tres"))
                {
                    var res = GD.Load<Tool>($"{path}/{fileName}");
                    res?.Create();
                }
            }
            fileName = dir.GetNext();
        }

        dir.ListDirEnd();
    }

    public static Vector2I GetMouseTile(TileMapLayer tilemap)
    {
        if (Instance == null || tilemap == null) return Vector2I.Zero;
        return tilemap.LocalToMap(Instance.ToLocal(Instance.GetGlobalMousePosition()));
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        var options = (Node)GetNode("/root/Options");
        bool devMode = options != null && (bool)options.Get("DEVELOPMENT_MODE");
        var shipValidator = (Node)GetNode("/root/ShipValidator");

        if (@event is InputEventMouseMotion || @event is InputEventMouseButton)
        {
            var mouseMask = Input.IsMouseButtonPressed(MouseButton.Left) ? 1 : 0;
            if (Input.IsMouseButtonPressed(MouseButton.Right)) mouseMask = 2;

            if (mouseMask == 1)
            {
                UseTool();
            }
            else if (mouseMask == 2)
            {
                Vector2I wallTile = GetMouseTile(WallTileMap);
                string wallType = (string)shipValidator.Call("get_tile_type", WallTileMap, wallTile);

                // Fixed: Correct operator precedence (!= instead of !type == "connector")
                if (wallType != "connector" || devMode)
                {
                    Vector2I objTile = GetMouseTile(ObjectTileMap);
                    string objType = (string)shipValidator.Call("get_tile_type", ObjectTileMap, objTile);

                    if (!string.IsNullOrEmpty(objType) && Tools.TryGetValue(objType, out var objTool) && objTool.Object)
                    {
                        SellTile(ObjectTileMap, objTile);
                    }
                    else
                    {
                        SellTile(WallTileMap, wallTile);
                    }
                }
            }
        }

        if (@event.IsActionPressed("editor_change_direction"))
        {
            Direction = (Direction + 1) % 4;
            UpdatePreview();
        }
    }

    public void UseTool()
    {
        if (CurrentTool == null) return;

        var options = (Node)GetNode("/root/Options");
        bool devMode = options != null && (bool)options.Get("DEVELOPMENT_MODE");

        if (!devMode && !(CurrentTool.NumberOfInstances < CurrentTool.WorldLimit || CurrentTool.WorldLimit < 0))
        {
            return;
        }

        TileMapLayer targetMap = CurrentTool.Object ? ObjectTileMap : WallTileMap;
        Vector2I tile = GetMouseTile(targetMap);

        var shipValidator = (Node)GetNode("/root/ShipValidator");
        string tileType = (string)shipValidator.Call("get_tile_type", targetMap, tile);

        if (tileType == CurrentTool.ToolName) return;

        bool placingOnSomething = false;
        if (CurrentTool.PlaceableOnAtlasChoords != new Vector2I(-1, -1))
        {
            placingOnSomething = true;
            if (CurrentTool.PlaceableOnAtlasChoords != WallTileMap.GetCellAtlasCoords(GetMouseTile(WallTileMap)))
            {
                return;
            }
        }

        if (tileType == "connector") return;

        SellTile(targetMap, tile, false);

        if (CurrentTool.Price != 0 && !Inventory.AddCurrency(-CurrentTool.Price) && !devMode)
        {
            return;
        }

        CurrentTool.NumberOfInstances++;

        if (!placingOnSomething)
        {
            targetMap.SetCellsTerrainConnect(new Godot.Collections.Array<Vector2I> { tile }, 0, -1, false);
        }

        if (CurrentTool.TerrainId != -1)
        {
            targetMap.SetCellsTerrainConnect(new Godot.Collections.Array<Vector2I> { tile }, 0, CurrentTool.TerrainId);
        }
        else if (CurrentTool.AtlasCoords != new Vector2I(-1, -1))
        {
            int alt = CurrentTool.Rotatable ? Direction : 0;
            targetMap.SetCell(tile, 0, CurrentTool.AtlasCoords, alt);
        }

        var wallsList = (Godot.Collections.Array)shipValidator.Get("walls");
        if (wallsList != null && wallsList.Contains(CurrentTool.ToolName) && Autoflooring)
        {
            shipValidator.Call("autofill_floor", targetMap);
        }
    }

    public static bool SellTile(TileMapLayer tilemap, Vector2I coords, bool deleteTile = true, bool reactAutofill = false)
    {
        if (tilemap == null) return false;

        bool sold = false;
        var shipValidator = (Node)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("ShipValidator");
        string type = (string)shipValidator?.Call("get_tile_type", tilemap, coords);

        if (!string.IsNullOrEmpty(type) && Tools.TryGetValue(type, out var foundTool))
        {
            foundTool.NumberOfInstances--;
            Inventory.AddCurrency(foundTool.Price, deleteTile);
            sold = true;
        }

        if (deleteTile)
        {
            tilemap.SetCellsTerrainConnect(new Godot.Collections.Array<Vector2I> { coords }, 0, -1, false);
        }

        if (Autoflooring && !reactAutofill && shipValidator != null)
        {
            shipValidator.Call("autofill_floor", tilemap);
        }

        return sold;
    }

    public static void ChangeTool(string key)
    {
        if (!Tools.TryGetValue(key, out var selectedTool))
        {
            GD.PushWarning($"Warning: Tool key '{key}' not found in registered tools.");
            return;
        }

        CurrentTool = selectedTool;
        if (ToolPreview != null)
        {
            ToolPreview.Texture = CurrentTool.Texture;
        }

        UpdatePreviewRotation();
    }

    public static void UpdatePreviewRotation()
    {
        if (ToolPreview == null || CurrentTool == null) return;

        ToolPreview.RotationDegrees = CurrentTool.Rotatable ? (90f * Direction) : 0f;
    }

    public void SaveShip(string path = "_default_ship")
    {
        EvideTiles();

        string location = path.StartsWith("_") ? "res://DefaultSave/ships/" : "user://saves/ships/";
        string dirPath = location + path + "/";

        DirAccess.MakeDirRecursiveAbsolute(dirPath);

        using var wallsSaveFile = FileAccess.Open(dirPath + "walls.dat", FileAccess.ModeFlags.Write);
        using var objectsSaveFile = FileAccess.Open(dirPath + "objects.dat", FileAccess.ModeFlags.Write);
        using var detailsSaveFile = FileAccess.Open(dirPath + "details.dat", FileAccess.ModeFlags.Write);

        if (wallsSaveFile != null)
        {
            foreach (Vector2I cell in WallTileMap.GetUsedCells())
            {
                wallsSaveFile.StoreFloat(cell.X);
                wallsSaveFile.StoreFloat(cell.Y);
                wallsSaveFile.Store16((ushort)WallTileMap.GetCellSourceId(cell));
                wallsSaveFile.StoreFloat(WallTileMap.GetCellAtlasCoords(cell).X);
                wallsSaveFile.StoreFloat(WallTileMap.GetCellAtlasCoords(cell).Y);
                wallsSaveFile.Store16((ushort)WallTileMap.GetCellAlternativeTile(cell));
            }
        }

        if (objectsSaveFile != null)
        {
            foreach (Vector2I cell in ObjectTileMap.GetUsedCells())
            {
                objectsSaveFile.StoreFloat(cell.X);
                objectsSaveFile.StoreFloat(cell.Y);
                objectsSaveFile.Store16((ushort)ObjectTileMap.GetCellSourceId(cell));
                objectsSaveFile.StoreFloat(ObjectTileMap.GetCellAtlasCoords(cell).X);
                objectsSaveFile.StoreFloat(ObjectTileMap.GetCellAtlasCoords(cell).Y);
                objectsSaveFile.Store16((ushort)ObjectTileMap.GetCellAlternativeTile(cell));
            }
        }

        detailsSaveFile?.Store16((ushort)CurrentShipPrice);

        ConsoleLog?.PrintOut("Ship saved as: " + path);
    }

    public bool LoadShip(string path = "_default_ship", bool charge = true)
    {
        string location = path.StartsWith("_") ? "res://DefaultSave/ships/" : "user://saves/ships/";
        string basePath = location + path;

        if (!FileAccess.FileExists(basePath + "/walls.dat") || !FileAccess.FileExists(basePath + "/objects.dat"))
        {
            return false;
        }

        if (Editor.Instance?.InventoryNode != null)
        {
            Inventory.Currency += CurrentShipPrice;
        }

        WallTileMap.Clear();
        ObjectTileMap.Clear();

        using (var wallsSaveFile = FileAccess.Open(basePath + "/walls.dat", FileAccess.ModeFlags.Read))
        {
            if (wallsSaveFile != null)
            {
                while (wallsSaveFile.GetPosition() < wallsSaveFile.GetLength())
                {
                    float x = wallsSaveFile.GetFloat();
                    float y = wallsSaveFile.GetFloat();
                    int sourceId = wallsSaveFile.Get16();
                    float atlasX = wallsSaveFile.GetFloat();
                    float atlasY = wallsSaveFile.GetFloat();
                    int alt = wallsSaveFile.Get16();

                    WallTileMap.SetCell(new Vector2I((int)x, (int)y), sourceId, new Vector2I((int)atlasX, (int)atlasY), alt);
                }
            }
        }

        using (var objectsSaveFile = FileAccess.Open(basePath + "/objects.dat", FileAccess.ModeFlags.Read))
        {
            if (objectsSaveFile != null)
            {
                while (objectsSaveFile.GetPosition() < objectsSaveFile.GetLength())
                {
                    float x = objectsSaveFile.GetFloat();
                    float y = objectsSaveFile.GetFloat();
                    int sourceId = objectsSaveFile.Get16();
                    float atlasX = objectsSaveFile.GetFloat();
                    float atlasY = objectsSaveFile.GetFloat();
                    int alt = objectsSaveFile.Get16();

                    ObjectTileMap.SetCell(new Vector2I((int)x, (int)y), sourceId, new Vector2I((int)atlasX, (int)atlasY), alt);
                }
            }
        }

        EvideTiles();

        if (charge)
        {
            Inventory.Currency -= CurrentShipPrice;
        }

        if (Inventory?.CurrencyValue != null)
        {
            Inventory.CurrencyValue.Text = Inventory.Currency.ToString();
        }

        ConsoleLog?.PrintOut("Ship loaded: " + path);
        return true;
    }
}