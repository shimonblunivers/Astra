using Godot;

[GlobalClass]
public partial class Floor : ShipPart
{
    [Export] public Sprite2D Sprite { get; set; }

    public int Layer { get; set; } = 0;

    public override void Init(Ship targetShip, Vector2I coords, float durability = 100f, float mass = 4f)
    {
        base.Init(targetShip, coords, durability, mass);
    }

    public override void _Ready()
    {
        base._Ready();
        Sprite ??= GetNodeOrNull<Sprite2D>("Sprite2D");
    }
}