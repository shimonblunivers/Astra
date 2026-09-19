using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class WallTileMapLayer : TileMapLayer
{
    private static readonly PackedScene DoorScene = GD.Load<PackedScene>("res://Ship/Walls/Door/Door.tscn");
    private static readonly PackedScene WallScene = GD.Load<PackedScene>("res://Ship/Walls/Wall/Wall.tscn");
    private static readonly PackedScene FloorScene = GD.Load<PackedScene>("res://Ship/Walls/Floor/Floor.tscn");
    private static readonly PackedScene CoreScene = GD.Load<PackedScene>("res://Ship/Walls/Core/Core.tscn");
    private static readonly PackedScene ThrusterScene = GD.Load<PackedScene>("res://Ship/Walls/Thruster/Thruster.tscn");
    private static readonly PackedScene ConnectorScene = GD.Load<PackedScene>("res://Ship/Walls/Connector/Connector.tscn");

    public Ship Ship { get; set; } = null;

    private Rect2I _firstRect;
    public Vector2[] TestPolygon { get; set; } = null;

    public Rect2I GetRect() => _firstRect;
    public Rect2I get_rect() => _firstRect;

    private void LoadHitbox()
    {
        var edges = CreateEdges();
        var outerEdges = DeleteEdges(edges);
        Vector2[] shape = ToShape(outerEdges);

        if (Ship == null || shape.Length == 0) return;

        Ship.Polygon = shape;

        if (Ship.Hitbox != null)
        {
            Ship.Hitbox.Polygon = shape;
        }

        if (Ship.Visual is Polygon2D visualPoly)
        {
            visualPoly.Polygon = shape;
        }

        var areaHitbox = Ship.GetNodeOrNull<CollisionPolygon2D>("Area/AreaHitbox");
        if (areaHitbox != null)
        {
            areaHitbox.Polygon = shape;
        }
    }

    private Vector2[] GetPoints(Vector2I tile)
    {
        Vector2I tileSize = TileSet.TileSize;
        return new Vector2[]
        {
            new Vector2(tile.X * tileSize.X, tile.Y * tileSize.Y + tileSize.Y),
            new Vector2(tile.X * tileSize.X, tile.Y * tileSize.Y),
            new Vector2(tile.X * tileSize.X + tileSize.X, tile.Y * tileSize.Y),
            new Vector2(tile.X * tileSize.X + tileSize.X, tile.Y * tileSize.Y + tileSize.Y)
        };
    }

    private List<(Vector2 Start, Vector2 End)> GetLines(Vector2[] points, float scale)
    {
        return new List<(Vector2, Vector2)>
        {
            (points[0] * scale, points[1] * scale),
            (points[1] * scale, points[2] * scale),
            (points[2] * scale, points[3] * scale),
            (points[3] * scale, points[0] * scale)
        };
    }

    private List<(Vector2 Start, Vector2 End)> CreateEdges()
    {
        var edges = new List<(Vector2, Vector2)>();
        foreach (Vector2I tile in GetUsedCells())
        {
            edges.AddRange(GetLines(GetPoints(tile), Constants.TileScale));
        }
        return edges;
    }

    private List<(Vector2 Start, Vector2 End)> DeleteEdges(List<(Vector2 Start, Vector2 End)> edges)
    {
        var seen = new HashSet<string>();
        var marked = new HashSet<string>();

        foreach (var edge in edges)
        {
            string k1 = $"{edge.Start.Snapped(new Vector2(0.01f, 0.01f))}_{edge.End.Snapped(new Vector2(0.01f, 0.01f))}";
            string k2 = $"{edge.End.Snapped(new Vector2(0.01f, 0.01f))}_{edge.Start.Snapped(new Vector2(0.01f, 0.01f))}";

            if (seen.Contains(k1) || seen.Contains(k2))
            {
                marked.Add(k1);
                marked.Add(k2);
            }
            else
            {
                seen.Add(k1);
                seen.Add(k2);
            }
        }

        var result = new List<(Vector2 Start, Vector2 End)>();
        foreach (var edge in edges)
        {
            string key = $"{edge.Start.Snapped(new Vector2(0.01f, 0.01f))}_{edge.End.Snapped(new Vector2(0.01f, 0.01f))}";
            if (!marked.Contains(key))
            {
                result.Add(edge);
            }
        }

        return result;
    }

    private Vector2[] ToShape(List<(Vector2 Start, Vector2 End)> edges)
    {
        if (edges.Count == 0) return Array.Empty<Vector2>();

        var result = new List<Vector2>();
        var remaining = new List<(Vector2 Start, Vector2 End)>(edges);
        var currentLine = remaining[0];
        remaining.RemoveAt(0);

        result.Add(currentLine.Start);

        int maxIterations = edges.Count * 2;
        int iteration = 0;

        while (remaining.Count > 0 && iteration++ < maxIterations)
        {
            int foundIndex = -1;
            bool invert = false;

            for (int i = 0; i < remaining.Count; i++)
            {
                if (currentLine.End.IsEqualApprox(remaining[i].Start))
                {
                    foundIndex = i;
                    invert = false;
                    break;
                }
                if (currentLine.End.IsEqualApprox(remaining[i].End))
                {
                    foundIndex = i;
                    invert = true;
                    break;
                }
            }

            if (foundIndex != -1)
            {
                var next = remaining[foundIndex];
                remaining.RemoveAt(foundIndex);
                currentLine = invert ? (next.End, next.Start) : next;
                result.Add(currentLine.Start);
            }
            else
            {
                break;
            }
        }

        return result.ToArray();
    }

    public void UpdateCenterOfMass()
    {
        var cells = GetUsedCells();
        if (cells.Count == 0 || Ship == null) return;

        float minX = cells[0].X;
        float maxX = cells[0].X;
        float minY = cells[0].Y;
        float maxY = cells[0].Y;

        foreach (var cell in cells)
        {
            if (cell.X < minX) minX = cell.X;
            if (cell.X > maxX) maxX = cell.X;
            if (cell.Y < minY) minY = cell.Y;
            if (cell.Y > maxY) maxY = cell.Y;
        }

        Vector2I tileSize = TileSet.TileSize;
        Vector2 com = new Vector2(
            5f * tileSize.X * (0.5f + (minX + maxX) / 2f),
            5f * tileSize.Y * (0.5f + (minY + maxY) / 2f)
        );

        Ship.Set("center_of_mass", com);
    }

    public bool load_ship(Ship targetShip, string path) => LoadShip(targetShip, path);

    public bool LoadShip(Ship targetShip, string path)
    {
        Ship = targetShip;
        Clear();

        GD.Print($"[WallTileMap] Loading ship file for path '{path}'...");
        string userPath = $"user://saves/ships/{path}/walls.dat";
        string defaultPath = $"res://DefaultSave/ships/{path}/walls.dat";
        string resolvedPath;

        if (!FileAccess.FileExists(userPath))
        {
            if (!FileAccess.FileExists(defaultPath))
            {
                GD.PushWarning($"[WallTileMap] walls.dat not found in '{userPath}' or '{defaultPath}'!");
                return false;
            }
            resolvedPath = defaultPath;
        }
        else
        {
            resolvedPath = userPath;
        }

        using (var saveFile = FileAccess.Open(resolvedPath, FileAccess.ModeFlags.Read))
        {
            if (saveFile == null) return false;

            int cellsLoaded = 0;
            while (saveFile.GetPosition() < saveFile.GetLength())
            {
                float posX = saveFile.GetFloat();
                float posY = saveFile.GetFloat();
                int sourceId = saveFile.Get16();
                int atlasX = (int)saveFile.GetFloat();
                int atlasY = (int)saveFile.GetFloat();
                int altTile = saveFile.Get16();

                SetCell(new Vector2I((int)posX, (int)posY), sourceId, new Vector2I(atlasX, atlasY), altTile);
                cellsLoaded++;
            }
            GD.Print($"[WallTileMap] Successfully loaded {cellsLoaded} cells from {resolvedPath}");
        }

        LoadHitbox();
        UpdateCenterOfMass();

        _firstRect = GetUsedRect();

        return ReplaceTiles();
    }

    private bool ReplaceTiles()
    {
        const int layer = 0;
        var atlas = TileSet?.GetSource(0) as TileSetAtlasSource;
        var atlasImage = atlas?.Texture?.GetImage();
        var textureCache = new Dictionary<string, ImageTexture>();

        var wallTilesNode = Ship?.WallTiles ?? (Node2D)Ship?.Get("wall_tiles");

        foreach (Vector2I cellPos in GetUsedCells())
        {
            TileData cell = GetCellTileData(cellPos);
            int objectDirection = GetCellAlternativeTile(cellPos);
            Vector2 tilePosition = MapToLocal(cellPos) * Constants.TileScale;

            if (cell == null)
            {
                if (ConnectorScene != null)
                {
                    var connector = ConnectorScene.Instantiate<Connector>();
                    connector.Init(Ship, cellPos);
                    connector.Position = tilePosition;
                    connector.RotationDegrees = objectDirection * 90f;
                    wallTilesNode?.AddChild(connector);
                }
                SetCell(cellPos, -1);
                continue;
            }

            string type = cell.GetCustomData("type").AsString();

            switch (type)
            {
                case "floor":
                    SpawnFloor(wallTilesNode, cellPos, tilePosition);
                    break;

                case "door":
                    if (DoorScene != null)
                    {
                        var door = DoorScene.Instantiate<Door>();
                        door.Init(Ship, cellPos);
                        door.Direction = cell.GetCustomData("direction").AsString();
                        door.Position = tilePosition;
                        wallTilesNode?.AddChild(door);
                    }
                    SpawnFloor(wallTilesNode, cellPos, tilePosition);
                    break;

                case "wall":
                    if (WallScene != null)
                    {
                        var wall = WallScene.Instantiate<Wall>();
                        wall.Init(Ship, cellPos);
                        wall.Position = tilePosition;
                        wallTilesNode?.AddChild(wall);

                        if (wall.LightOccluder != null)
                        {
                            wall.LightOccluder.Occluder = cell.GetOccluderPolygon(layer, 0);
                            wall.LightOccluder.Scale = Vector2.One * Constants.TileScale;
                        }

                        Vector2I atlasCoords = GetCellAtlasCoords(cellPos);
                        string cacheKey = $"{atlasCoords}_{objectDirection}";

                        if (!textureCache.TryGetValue(cacheKey, out var tileTexture) && atlasImage != null && atlas != null)
                        {
                            var tileImage = atlasImage.GetRegion(atlas.GetTileTextureRegion(atlasCoords));
                            for (int i = 0; i < objectDirection; i++)
                            {
                                tileImage.Rotate90(ClockDirection.Clockwise);
                            }
                            tileTexture = ImageTexture.CreateFromImage(tileImage);
                            tileTexture.SetSizeOverride(new Vector2I(32, 32));
                            textureCache[cacheKey] = tileTexture;
                        }

                        if (tileTexture != null)
                        {
                            wall.SetTexture(tileTexture);
                        }
                    }
                    break;

                case "core":
                    if (CoreScene != null)
                    {
                        var core = CoreScene.Instantiate<Core>();
                        core.Init(Ship, cellPos);
                        core.Position = tilePosition;
                        wallTilesNode?.AddChild(core);
                    }
                    SpawnFloor(wallTilesNode, cellPos, tilePosition);
                    break;

                case "thruster":
                    if (ThrusterScene != null)
                    {
                        var thruster = ThrusterScene.Instantiate<Thruster>();
                        thruster.Init(Ship, cellPos, 150f, 5f, objectDirection);
                        thruster.Position = tilePosition;
                        thruster.RotationDegrees = objectDirection * 90f;
                        wallTilesNode?.AddChild(thruster);
                    }
                    break;

                case "connector":
                    if (ConnectorScene != null)
                    {
                        var connector = ConnectorScene.Instantiate<Connector>();
                        connector.Init(Ship, cellPos);
                        connector.Position = tilePosition;
                        connector.RotationDegrees = objectDirection * 90f;
                        wallTilesNode?.AddChild(connector);
                    }
                    break;
            }
        }

        return true;
    }

    private void SpawnFloor(Node2D parent, Vector2I cellPos, Vector2 position)
    {
        if (FloorScene == null) return;

        var floor = FloorScene.Instantiate<Floor>();
        floor.Init(Ship, cellPos);
        floor.Position = position;
        parent?.AddChild(floor);
    }
}