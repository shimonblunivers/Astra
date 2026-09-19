using Godot;

[GlobalClass]
public partial class Canon : Node2D
{
    private static readonly PackedScene ProjectileScene = GD.Load<PackedScene>("res://Ship/Walls/Canon/Projectile.tscn");

    [Export] public float RotationSpeedDegrees { get; set; } = 60.0f; // 60 deg/sec equivalent to 1 deg per 60Hz frame

    public override void _PhysicsProcess(double delta)
    {
        RotationDegrees += RotationSpeedDegrees * (float)delta;
    }

    public void Shoot()
    {
        if (ProjectileScene == null) return;

        var instance = ProjectileScene.Instantiate<Node2D>();
        if (instance == null) return;

        instance.Set("dir", GlobalRotation);
        instance.Set("spawn_position", GlobalPosition);
        instance.Set("spawn_rotation", GlobalRotation - Mathf.Pi / 2.0f);

        GetTree().Root.CallDeferred(Node.MethodName.AddChild, instance);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
        {
            Shoot();
        }
    }
}