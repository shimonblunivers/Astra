using Godot;

[GlobalClass]
public partial class Debris : ShipPart
{
    [Export] public Sprite2D Sprite { get; set; }
    [Export] public AudioStreamPlayer2D SpawnSound { get; set; }

    public override void Init(Ship targetShip, Vector2I coords, float durability = -1f, float mass = 4f)
    {
        base.Init(targetShip, coords, durability, mass);
    }

    public override void _Ready()
    {
        base._Ready();

        Sprite ??= GetNodeOrNull<Sprite2D>("Sprite2D");
        SpawnSound ??= GetNodeOrNull<AudioStreamPlayer2D>("Sounds/Spawn");

        var rng = new RandomNumberGenerator();
        rng.Randomize();

        if (Sprite != null)
        {
            Sprite.RotationDegrees = 90f * rng.RandiRange(0, 3);
        }

        if (SpawnSound != null)
        {
            SpawnSound.PitchScale = (float)GD.RandRange(0.9, 1.1);
            SpawnSound.Play();
        }
    }
}