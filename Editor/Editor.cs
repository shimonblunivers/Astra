using Godot;
using System.Text;

[GlobalClass]
public partial class Editor : Node2D
{
    public static Editor Instance { get; private set; }

    [Export] public Console ConsoleLog { get; set; }
    [Export] public ShipEditor ShipEditorNode { get; set; }
    [Export] public Control SavemenuUI { get; set; }
    [Export] public Camera2D Camera { get; set; }
    [Export] public LineEdit ShipNameLabel { get; set; }
    [Export] public RichTextLabel ShipList { get; set; }
    [Export] public Inventory InventoryNode { get; set; }
    [Export] public Label DirectionLabel { get; set; }
    [Export] public Control LimitRect { get; set; }

    public Godot.Collections.Array Ships { get; } = new Godot.Collections.Array();

    private bool _inventoryOpen = false;
    private readonly Vector2 _inventoryPositions = new Vector2(160, -165); // X = Open, Y = Closed

    public override void _Ready()
    {
        Instance = this;

        ConsoleLog ??= GetNode<Console>("HUD/Console/ConsoleLog");
        ShipEditorNode ??= GetNode<ShipEditor>("Ship");
        SavemenuUI ??= GetNode<Control>("HUD/SavemenuUI");
        Camera ??= GetNode<Camera2D>("Camera2D");
        ShipNameLabel ??= GetNode<LineEdit>("HUD/SavemenuUI/ShipName");
        ShipList ??= GetNode<RichTextLabel>("HUD/SavemenuUI/Control/ShipList");
        InventoryNode ??= GetNode<Inventory>("HUD/Inventory");
        DirectionLabel ??= GetNodeOrNull<Label>("HUD/DirectionLabel");
        LimitRect ??= GetNode<Control>("LimitRect");

        var options = (Node)GetNode("/root/Options");
        bool devMode = options != null && (bool)options.Get("DEVELOPMENT_MODE");

        LimitRect.Visible = !devMode;

        UpdateShipList();

        ShipEditorNode.Inventory = InventoryNode;
        InventoryNode.LoadGrid();

        if (Player.MainPlayer?.OwnedShip == null)
        {
            ShipEditorNode.LoadShip("_start_ship", false);
        }
        else
        {
            string shipPath = (string)Player.MainPlayer.OwnedShip.Get("path");
            ShipEditorNode.LoadShip(shipPath, false);
        }

        Camera.MakeCurrent();
        CenterCamera();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("editor_toggle_toolmenu"))
        {
            _inventoryOpen = !_inventoryOpen;
            var tween = CreateTween();
            const float duration = 0.5f;

            float targetX = _inventoryOpen ? _inventoryPositions.X : _inventoryPositions.Y;
            var ease = _inventoryOpen ? Tween.EaseType.Out : Tween.EaseType.In;

            tween.TweenProperty(InventoryNode, "position", new Vector2(targetX, 0), duration)
                .SetEase(ease);
        }

        if (@event.IsActionPressed("game_toggle_menu"))
        {
            Exit();
        }
    }

    public override void _Process(double delta)
    {
        if (LimitRect != null && Camera != null)
        {
            LimitRect.Position = new Vector2(
                LimitRect.Position.X,
                Camera.Position.Y - LimitRect.Size.Y / 2f
            );
        }
    }

    public void CenterCamera()
    {
        Vector2I startingCoords = (Vector2I)typeof(ShipEditor).GetField("starting_block_coords")?.GetValue(null)!;
        Camera.Position = new Vector2(startingCoords.X, startingCoords.Y);
        Camera.Zoom = new Vector2(1f, 1f);
    }

    private void Exit()
    {
        if (Instance == this) Instance = null;
        QueueFree();

        if (Player.MainPlayer?.Camera != null)
        {
            Player.MainPlayer.Camera.MakeCurrent();
        }

        GetTree().Paused = false;

        World.Instance.Visible = true;
        var uiNode = (CanvasItem)World.Instance.Get("ui_node");
        if (uiNode != null) uiNode.Visible = true;

        World.Instance.Set("used_builder", default);
    }

    public void OnSavePressed()
    {
        var wallTileMap = (TileMap)ShipEditorNode.Get("wall_tile_map");
        var shipValidator = (Node)GetNode("/root/ShipValidator");

        bool isValid = (bool)shipValidator.Call("check_validity", wallTileMap);
        if (!isValid)
        {
            var console = (Console)ShipEditorNode.Get("console");
            console.PrintOut("[color=red]Ship does not meet the requirements for saving![/color]\nPlease check that you have a core in your ship.\nAlso check that all blocks are connected.");
            return;
        }

        if (string.IsNullOrEmpty(ShipNameLabel.Text))
        {
            ShipEditorNode.Call("save_ship");
        }
        else
        {
            ShipEditorNode.Call("save_ship", ShipNameLabel.Text);
        }

        UpdateShipList();
    }

    public void UpdateShipList()
    {
        ShipEditorNode.Call("evide_tiles");
        var sb = new StringBuilder("[center][table=3]");

        var options = (Node)GetNode("/root/Options");
        bool devMode = options != null && (bool)options.Get("DEVELOPMENT_MODE");

        AppendDirectoryShips("user://saves/ships", sb, devMode);

        if (devMode)
        {
            AppendDirectoryShips("res://DefaultSave/ships", sb, true);
        }

        sb.Append("[/table][/center]");
        ShipList.Text = sb.ToString();
    }

    private void AppendDirectoryShips(string path, StringBuilder sb, bool devMode)
    {
        using var dir = DirAccess.Open(path);
        if (dir == null) return;

        dir.ListDirBegin();
        string fileName = dir.GetNext();

        while (!string.IsNullOrEmpty(fileName))
        {
            if (dir.CurrentIsDir())
            {
                // Fixed: Explicit, clean visibility filtering without operator confusion
                if (!devMode && (fileName.StartsWith('_') || fileName.StartsWith('%')))
                {
                    fileName = dir.GetNext();
                    continue;
                }

                sb.Append($"[cell=1][left][url={fileName}]{fileName}[/url][/left][/cell]");

                // Fixed: Pointing to dir.GetCurrentDir() resolves details.dat in both user:// and res://DefaultSave/
                string detailsPath = $"{dir.GetCurrentDir()}/{fileName}/details.dat";

                if (!fileName.StartsWith('_') && FileAccess.FileExists(detailsPath))
                {
                    using var details = FileAccess.Open(detailsPath, FileAccess.ModeFlags.Read);
                    ushort price = details.Get16();

                    int currentShipPrice = (int)ShipEditorNode.Get("current_ship_price");
                    float totalAvailable = currentShipPrice + Inventory.Currency;
                    bool cannotAfford = price > totalAvailable;

                    sb.Append("[cell=1]      ->      [/cell][cell=1][right]");
                    if (cannotAfford) sb.Append("[color=red]");
                    sb.Append(price);
                    if (cannotAfford) sb.Append("[/color]");
                    sb.Append(" [img]res://UI/currency.png[/img][/right][/cell]");
                }
                else
                {
                    sb.Append("[cell=1][/cell][cell=1][/cell]");
                }
            }

            fileName = dir.GetNext();
        }

        dir.ListDirEnd();
    }

    public void OnLoadPressed()
    {
        var options = (Node)GetNode("/root/Options");
        bool devMode = options != null && (bool)options.Get("DEVELOPMENT_MODE");
        string targetShip = ShipNameLabel.Text;

        if (!targetShip.StartsWith('_'))
        {
            string detailsPath = $"user://saves/ships/{targetShip}/details.dat";
            if (FileAccess.FileExists(detailsPath))
            {
                // Fixed: using statement guarantees FileAccess is closed immediately
                using var details = FileAccess.Open(detailsPath, FileAccess.ModeFlags.Read);
                ushort price = details.Get16();

                int currentShipPrice = (int)ShipEditorNode.Get("current_ship_price");
                if (price > currentShipPrice + Inventory.Currency && !devMode)
                {
                    ConsoleLog.PrintOut("[color=red]Insufficient funds![/color]");
                    return;
                }
            }
        }

        bool success = string.IsNullOrEmpty(targetShip)
            ? (bool)ShipEditorNode.Call("load_ship")
            : (bool)ShipEditorNode.Call("load_ship", targetShip);

        if (!success)
        {
            ConsoleLog.PrintOut($"[color=red]Ship with name '{targetShip}' was not found![/color]");
        }
        else
        {
            OnExitPressed();
            CenterCamera();
        }
    }

    public void OnOpenSavemenuPressed()
    {
        var wallTileMap = (TileMap)ShipEditorNode.Get("wall_tile_map");
        var shipValidator = (Node)GetNode("/root/ShipValidator");
        shipValidator.Call("autofill_floor", wallTileMap);

        SavemenuUI.Visible = true;
        GetNode<Control>("HUD/Savemenu").Visible = false;
        Camera.Set("locked", true);
        UpdateShipList();
    }

    public void OnExitPressed()
    {
        SavemenuUI.Visible = false;
        GetNode<Control>("HUD/Savemenu").Visible = true;
        Camera.Set("locked", false);
    }

    public void OnShipListMetaClicked(Variant meta)
    {
        ShipNameLabel.Text = meta.AsString();
    }

    public void OnAutofloorPressed()
    {
        var wallTileMap = (TileMap)ShipEditorNode.Get("wall_tile_map");
        var shipValidator = (Node)GetNode("/root/ShipValidator");
        shipValidator.Call("autofill_floor", wallTileMap);
    }

    public void OnAutofloorButtonToggled(bool toggledOn)
    {
        typeof(ShipEditor).GetField("autoflooring")?.SetValue(null, toggledOn);
        if (toggledOn)
        {
            OnAutofloorPressed();
        }
    }

    public void OnDeployPressed()
    {
        OnAutofloorPressed();

        var wallTileMap = (TileMap)ShipEditorNode.Get("wall_tile_map");
        var shipValidator = (Node)GetNode("/root/ShipValidator");

        bool isValid = (bool)shipValidator.Call("check_validity", wallTileMap);
        if (!isValid)
        {
            var console = (Console)ShipEditorNode.Get("console");
            console.PrintOut("[color=red]Ship does not meet the requirements for saving![/color]\nPlease check that you have a core in your ship.\nAlso check that all blocks are connected.");
            return;
        }

        ShipEditorNode.Call("save_ship", "%player_ship_new");

        var usedBuilder = World.Instance.Get("used_builder");
        if (usedBuilder.VariantType != Variant.Type.Nil)
        {
            var shipManager = (Node)GetNode("/root/ShipManager");
            shipManager.Call("build_ship", usedBuilder, true, "%player_ship_new");

            int finalCurrency = (int)ShipEditorNode.Inventory.Get("currency");
            if (Player.MainPlayer != null)
            {
                Player.MainPlayer.Currency = finalCurrency;
            }
        }

        Exit();
    }
}