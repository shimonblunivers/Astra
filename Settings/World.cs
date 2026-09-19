using Godot;
using Godot.Collections;
using System.Collections.Generic;

[GlobalClass]
public partial class World : Node2D
{
    public static World Instance { get; set; }

    private Vector2 _centerOfUniverse = Vector2.Zero;

    public static SaveFile SaveFile { get; set; }
    public static Builder UsedBuilder { get; set; } = null;
    public static float DifficultyMultiplier { get; set; } = 0f;

    [Export] public CanvasModulate CanvasModulateNode { get; set; }
    [Export] public UIManager UiNode { get; set; }
    [Export] public AudioStreamPlayer AudioStreamPlayerNode { get; set; }

    private static readonly PackedScene EditorScene = GD.Load<PackedScene>("res://Scenes/Editor.tscn");
    private static readonly PackedScene MenuScene = GD.Load<PackedScene>("res://Scenes/Menu.tscn");

    public void LoadMissions()
    {
        const string path = "res://Quests/Missions";
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
                    var resource = GD.Load<Resource>($"{path}/{fileName}");
                    resource?.Call("create");
                }
            }
            fileName = dir.GetNext();
        }

        dir.ListDirEnd();
    }

    public override void _Ready()
    {
        Instance = this;

        CanvasModulateNode ??= GetNodeOrNull<CanvasModulate>("CanvasModulate");
        UiNode ??= GetNodeOrNull<UIManager>("UI");
        AudioStreamPlayerNode ??= GetNodeOrNull<AudioStreamPlayer>("AudioStreamPlayer");

        DisplayServer.WindowSetMaxSize(new Vector2I(3840, 2160));

        var objectList = ObjectList.Instance ?? (ObjectList)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("ObjectList");
        if (objectList != null)
        {
            objectList.StartedGame = true;
        }

        LoadMissions();

        SaveFile = new SaveFile();
        SaveFile.InitializeFiles();
    }

    public static void ResetValues()
    {
        var uiManager = (Node)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("UIManager");
        var uiInstance = (Node)uiManager?.Get("instance");
        uiInstance?.Call("loading_screen");

        if (Ship.Ships != null)
        {
            var shipsCopy = new List<Ship>(Ship.Ships);
            foreach (var ship in shipsCopy)
            {
                if (GodotObject.IsInstanceValid(ship))
                {
                    ship.Call("delete");
                }
            }
            Ship.Ships.Clear();
        }

        if (NPC.Npcs != null)
        {
            var npcsCopy = new List<NPC>(NPC.Npcs);
            foreach (var npc in npcsCopy)
            {
                if (GodotObject.IsInstanceValid(npc))
                {
                    npc.Call("delete");
                }
            }
            NPC.Npcs.Clear();
        }

        if (Item.Items != null)
        {
            var itemsCopy = new List<Item>(Item.Items);
            foreach (var item in itemsCopy)
            {
                if (GodotObject.IsInstanceValid(item))
                {
                    item.Delete();
                }
            }
            Item.Items.Clear();
            Item.ItemIdHistory.Clear();
        }

        var questManager = QuestManager.Instance ?? (QuestManager)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("QuestManager");
        if (questManager != null)
        {
            var questsCopy = new List<Quest>(questManager.ActiveQuests.Values);
            foreach (var quest in questsCopy)
            {
                quest?.Delete();
            }

            questManager.QuestIdHistory.Clear();
            questManager.HighlightedQuestId = -1;
            questManager.HighlightMainStation = false;
        }

        if (Player.MainPlayer != null)
        {
            Player.MainPlayer.Health = Player.MainPlayer.MaxHealth;
            Player.MainPlayer.Currency = 0;

            // Check if currency_updated_signal exists before emitting to prevent runtime crash
            if (Player.MainPlayer.HasSignal("currency_updated_signal"))
            {
                Player.MainPlayer.EmitSignal("currency_updated_signal");
            }
        }

        if (Instance != null)
        {
            Instance._centerOfUniverse = Vector2.Zero;
            Instance.Transform = new Transform2D(Instance.Transform.X, Instance.Transform.Y, Vector2.Zero);
        }
    }

    public void NewWorld()
    {
        SaveFile = new SaveFile();
        SaveFile.InitializeFiles();

        ResetValues();

        GD.Print("Generating ships..");
        var shipManager = (Node)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("ShipManager");
        shipManager?.Call("randomly_generate_ships");

        GD.Print("Spawning player..");
        // Direct strongly-typed call instead of dynamic .Call("spawn")
        if (Player.MainPlayer != null)
        {
            Player.MainPlayer.Spawn(Vector2.Zero, Vector2.Zero, 0f);
        }

        GD.Print("New world created.");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("game_toggle_menu"))
        {
            OpenMenu();
        }
    }

    public void ShiftOrigin(Vector2 by)
    {
        GlobalPosition += by;
        _centerOfUniverse += by;
    }

    public Vector2 GetDistanceFromCenter(Vector2 pos)
    {
        return pos;
    }

    public override void _Process(double delta)
    {
        var options = Options.Instance ?? (Options)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("Options");
        bool devMode = options != null && options.DevelopmentMode;

        if (!devMode) return;
        QueueRedraw();
    }

    public override void _Draw()
    {
        var options = Options.Instance ?? (Options)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("Options");
        bool devMode = options != null && options.DevelopmentMode;

        if (!devMode) return;
        DrawCircle(-_centerOfUniverse, 25f, Colors.LightBlue);
    }

    public void OpenEditor(Builder builder = null)
    {
        UsedBuilder = builder;

        Visible = false;
        if (UiNode != null) UiNode.Visible = false;

        GetTree().Paused = true;
        var editorObject = EditorScene.Instantiate<Node>();

        if (editorObject is Node node)
        {
            node.ProcessMode = ProcessModeEnum.WhenPaused;
        }

        GetTree().Root.CallDeferred(Node.MethodName.AddChild, editorObject);
    }

    public void OpenMenu()
    {
        GetTree().Paused = true;
        var menuObject = MenuScene.Instantiate<Node>();

        if (menuObject is Node node)
        {
            node.ProcessMode = ProcessModeEnum.WhenPaused;
        }

        GetTree().Root.CallDeferred(Node.MethodName.AddChild, menuObject);
    }

    public void OnAudioStreamPlayerFinished()
    {
        AudioStreamPlayerNode?.Play();
    }
}