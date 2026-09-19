using Godot;

[GlobalClass]
public partial class Options : Node
{
    public static Options Instance { get; private set; }

    [Export] public bool DevelopmentMode { get; set; }
    [Export] public bool Fullscreen { get; set; } = false;

    // Instance property aliases for dynamic GDScript calls
    public bool DEVELOPMENT_MODE => DevelopmentMode;
    public bool FULLSCREEN
    {
        get => Fullscreen;
        set => Fullscreen = value;
    }

    public override void _Ready()
    {
        Instance = this;

        // Initialize editor check safely during node setup
        DevelopmentMode = OS.HasFeature("editor");
        Fullscreen = DisplayServer.WindowGetMode() == DisplayServer.WindowMode.ExclusiveFullscreen;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("toggle_fullscreen"))
        {
            Fullscreen = !Fullscreen;

            DisplayServer.WindowSetMode(Fullscreen
                ? DisplayServer.WindowMode.ExclusiveFullscreen
                : DisplayServer.WindowMode.Windowed);
        }
    }
}