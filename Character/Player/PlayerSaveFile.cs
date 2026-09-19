using Godot;

[GlobalClass]
public partial class PlayerSaveFile : Resource
{
    [Export] public Vector2 Position { get; set; }
    [Export] public Vector2 OldPosition { get; set; }
    [Export] public float Currency { get; set; }
    [Export] public float Health { get; set; }
    [Export] public float Rotation { get; set; }

    public static PlayerSaveFile Save()
    {
        if (Player.MainPlayer == null) return null;

        var world = World.Instance ?? (World)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("World");

        var file = new PlayerSaveFile
        {
            Position = world != null ? world.GetDistanceFromCenter(Player.MainPlayer.GlobalPosition) : Player.MainPlayer.GlobalPosition,
            OldPosition = (Vector2)Player.MainPlayer.Get("_old_position"),
            Currency = Player.MainPlayer.Currency,
            Health = Player.MainPlayer.Health,
            Rotation = (float)Player.MainPlayer.GlobalRotation
        };

        return file;
    }

    public void Load()
    {
        if (Player.MainPlayer == null) return;

        var world = World.Instance ?? (World)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("World");
        Vector2 universeCenter = world != null ? (Vector2)world.Get("_center_of_universe") : Vector2.Zero;

        Player.MainPlayer.GlobalPosition = Position - universeCenter;
        Player.MainPlayer.Set("_old_position", Position);
        Player.MainPlayer.Currency = Currency;
        Player.MainPlayer.Health = Health;
        Player.MainPlayer.GlobalRotation = Rotation;

        // Line 58 fix: instance call on Player.MainPlayer rather than static GodotObject.Call
        Player.MainPlayer.Call("spawn", Position - universeCenter, Vector2.Zero, Rotation);
    }
}