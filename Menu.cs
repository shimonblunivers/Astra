using Godot;

[GlobalClass]
public partial class Menu : CanvasLayer
{
    private static readonly PackedScene SettingsScene = GD.Load<PackedScene>("res://Scenes/Settings.tscn");
    private static readonly PackedScene CreditsScene = GD.Load<PackedScene>("res://Scenes/Credits.tscn");

    public static Menu Instance { get; private set; }

    [Export] public Button PlayButton { get; set; }
    [Export] public Button NewGameButton { get; set; }
    [Export] public Button SettingsButton { get; set; }
    [Export] public Button CreditsButton { get; set; }
    [Export] public Button QuitButton { get; set; }

    [Export] public AudioStreamPlayer AudioStreamPlayerNode { get; set; }
    [Export] public CanvasItem FurtherBackground { get; set; }
    [Export] public CanvasItem CloserBackground { get; set; }

    public override void _Ready()
    {
        Instance = this;
        ProcessMode = ProcessModeEnum.Always; // Ensure it processes even if the tree was paused

        PlayButton ??= GetNodeOrNull<Button>("MarginContainer/VBoxContainer/Play");
        NewGameButton ??= GetNodeOrNull<Button>("MarginContainer/VBoxContainer/NewGame");
        SettingsButton ??= GetNodeOrNull<Button>("MarginContainer/VBoxContainer/Settings");
        CreditsButton ??= GetNodeOrNull<Button>("MarginContainer/VBoxContainer/Credits");
        QuitButton ??= GetNodeOrNull<Button>("MarginContainer/VBoxContainer/Quit");

        AudioStreamPlayerNode ??= GetNodeOrNull<AudioStreamPlayer>("AudioStreamPlayer");
        FurtherBackground ??= GetNodeOrNull<CanvasItem>("FurtherBackground");
        CloserBackground ??= GetNodeOrNull<CanvasItem>("CloserBackground");

        // Explicitly connect signals in C#
        if (PlayButton != null) PlayButton.Pressed += OnPlayPressed;
        if (NewGameButton != null) NewGameButton.Pressed += OnNewGamePressed;
        if (SettingsButton != null) SettingsButton.Pressed += OnSettingsPressed;
        if (CreditsButton != null) CreditsButton.Pressed += OnCreditsPressed;
        if (QuitButton != null) QuitButton.Pressed += OnQuitPressed;

        var objectList = ObjectList.Instance ?? (ObjectList)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("ObjectList");
        bool gameStarted = objectList != null && objectList.StartedGame;

        if (AudioStreamPlayerNode != null)
        {
            AudioStreamPlayerNode.Playing = !gameStarted;
        }

        if (gameStarted)
        {
            if (PlayButton != null)
            {
                PlayButton.Text = "Resume";
            }

            if (FurtherBackground != null) FurtherBackground.Visible = false;
            if (CloserBackground != null) CloserBackground.Visible = false;
        }
        else if (!DirAccess.DirExistsAbsolute("user://saves/worlds/last_save"))
        {
            PlayButton?.QueueFree();
        }
    }

    public void OnNewGamePressed()
    {
        GD.Print("[Menu] New Game clicked!");
        var objectList = ObjectList.Instance ?? (ObjectList)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("ObjectList");
        bool gameStarted = objectList != null && objectList.StartedGame;

        string savePath = SaveFile.GetSavePath();
        string saveDir = "user://saves/worlds/last_save";

        if (gameStarted)
        {
            if (Player.MainPlayer != null)
            {
                var camera = (Camera2D)Player.MainPlayer.Get("camera");
                camera?.MakeCurrent();
            }

            GetTree().Paused = false;

            if (World.Instance != null)
            {
                World.Instance.Visible = true;
                if (World.Instance.UiNode != null)
                {
                    World.Instance.UiNode.Visible = true;
                }
            }

            QueueFree();

            if (DirAccess.DirExistsAbsolute(saveDir))
            {
                SaveFile.DeleteDirectory(saveDir);
            }
            else if (FileAccess.FileExists(savePath))
            {
                DirAccess.RemoveAbsolute(savePath);
            }

            World.Instance?.NewWorld();
        }
        else
        {
            if (DirAccess.DirExistsAbsolute(saveDir))
            {
                SaveFile.DeleteDirectory(saveDir);
            }
            else if (FileAccess.FileExists(savePath))
            {
                DirAccess.RemoveAbsolute(savePath);
            }

            GetTree().ChangeSceneToFile("res://Scenes/Game.tscn");
        }
    }

    public void OnPlayPressed()
    {
        GD.Print("[Menu] Play clicked!");
        var objectList = ObjectList.Instance ?? (ObjectList)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("ObjectList");
        bool gameStarted = objectList != null && objectList.StartedGame;

        if (gameStarted)
        {
            GetTree().Paused = false;

            if (World.Instance != null)
            {
                World.Instance.Visible = true;
                if (World.Instance.UiNode != null)
                {
                    World.Instance.UiNode.Visible = true;
                }
            }

            QueueFree();
        }
        else
        {
            GetTree().ChangeSceneToFile("res://Scenes/Game.tscn");
        }
    }

    public void OnSettingsPressed()
    {
        GD.Print("[Menu] Settings clicked!");
        OpenModalScene(SettingsScene);
    }

    public void OnCreditsPressed()
    {
        GD.Print("[Menu] Credits clicked!");
        OpenModalScene(CreditsScene);
    }

    public void OnQuitPressed()
    {
        GD.Print("[Menu] Quit clicked!");
        GetTree().Quit();
    }

    private void OpenModalScene(PackedScene packedScene)
    {
        if (packedScene == null) return;

        Visible = false;

        var modalInstance = packedScene.Instantiate<Node>();
        if (modalInstance is Node node)
        {
            node.ProcessMode = ProcessModeEnum.Always;
            node.TreeExited += () =>
            {
                if (GodotObject.IsInstanceValid(this))
                {
                    Visible = true;
                }
            };
        }

        GetTree().Root.CallDeferred(Node.MethodName.AddChild, modalInstance);
    }
}