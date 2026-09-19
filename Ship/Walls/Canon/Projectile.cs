using Godot;

[GlobalClass]
public partial class Projectile : CharacterBody2D
{
    [Export] public float Speed { get; set; } = 500f;
    [Export] public int Damage { get; set; } = 10;
    [Export] public float LifetimeSeconds { get; set; } = 5.0f;

    public float Dir { get; set; }
    public Vector2 SpawnPosition { get; set; }
    public float SpawnRotation { get; set; }

    // Backward-compatibility aliases for dynamic GDScript calls
    public float SPEED
    {
        get => Speed;
        set => Speed = value;
    }
    public float dir
    {
        get => Dir;
        set => Dir = value;
    }
    public Vector2 spawn_position
    {
        get => SpawnPosition;
        set
        {
            SpawnPosition = value;
            GlobalPosition = value;
        }
    }
    public float spawn_rotation
    {
        get => SpawnRotation;
        set
        {
            SpawnRotation = value;
            GlobalRotation = value;
        }
    }
    public int damage
    {
        get => Damage;
        set => Damage = value;
    }

    private double _lifetimeTimer = 0;

    public override void _Ready()
    {
        GlobalPosition = SpawnPosition;
        GlobalRotation = SpawnRotation;
    }

    public override void _PhysicsProcess(double delta)
    {
        Velocity = new Vector2(0, -Speed).Rotated(Dir);
        MoveAndSlide();

        _lifetimeTimer += delta;
        if (_lifetimeTimer >= LifetimeSeconds)
        {
            QueueFree();
        }
    }

    public void OnArea2DBodyEntered(Node2D body)
    {
        if (body == null) return;

        if (body.IsInGroup("Wall"))
        {
            var parent = body.GetParent();
            if (GodotObject.IsInstanceValid(parent) && parent.HasMethod("damage"))
            {
                parent.Call("damage", Damage);
            }

            QueueFree();
        }
    }
}