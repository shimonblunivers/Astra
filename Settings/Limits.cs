using Godot;

[GlobalClass]
public partial class Constants : Node
{
    public static Constants Instance { get; private set; }

    public const float VelocityMax = 69420.0f;
    public const int TileScale = 5;

    // Instance property accessors in case GDScript calls them dynamically via Get()
    public float VELOCITY_MAX => VelocityMax;
    public int TILE_SCALE => TileScale;

    public override void _Ready()
    {
        Instance = this;
    }
}