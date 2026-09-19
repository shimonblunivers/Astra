using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class Thruster : ShipPart
{
    public enum ThrusterDirection
    {
        Right = 0,
        Down = 1,
        Left = 2,
        Up = 3
    }

    [Export] public Sprite2D Sprite { get; set; }
    [Export] public GpuParticles2D JetParticles { get; set; }
    [Export] public GpuParticles2D LeftSideJetParticles { get; set; }
    [Export] public GpuParticles2D RightSideJetParticles { get; set; }

    [Export] public AudioStreamPlayer2D JetSound { get; set; }
    [Export] public AudioStreamPlayer2D SideJetSound { get; set; }

    public int Layer { get; set; } = 0;
    public float Power { get; set; }
    public int Direction { get; set; }
    public bool Running { get; set; } = false;
    public bool Blocked { get; set; } = false;

    // Index 0: Left side, Index 1: Right side
    public bool[] BlockedSides { get; } = new bool[2];

    /// <summary>
    /// Required empty space in tiles to function.
    /// </summary>
    [Export] private int _requiredSpace = 4;

    private bool _thrustContributed = false;

    public void Init(Ship targetShip, Vector2I coords, float durability = 150f, float mass = 5f, int direction = 0, float power = 1000f)
    {
        base.Init(targetShip, coords, durability, mass);

        Direction = direction;
        Power = power;

        if (Ship != null)
        {
            var thrustersArray = (Godot.Collections.Array)Ship.Get("thrusters");
            if (thrustersArray != null && Direction >= 0 && Direction < thrustersArray.Count)
            {
                var directionList = (Godot.Collections.Array)thrustersArray[Direction];
                directionList?.Add(this);
            }
        }

        Callable.From(GetBlockedSides).CallDeferred();
    }

    public override void _Ready()
    {
        base._Ready();

        Sprite ??= GetNodeOrNull<Sprite2D>("Sprite2D");
        JetParticles ??= GetNodeOrNull<GpuParticles2D>("JetParticles");
        LeftSideJetParticles ??= GetNodeOrNull<GpuParticles2D>("LeftSideJetParticles");
        RightSideJetParticles ??= GetNodeOrNull<GpuParticles2D>("RightSideJetParticles");

        JetSound ??= GetNodeOrNull<AudioStreamPlayer2D>("Sounds/Jet");
        SideJetSound ??= GetNodeOrNull<AudioStreamPlayer2D>("Sounds/SideJet");
    }

    public void SetStatus(bool status)
    {
        Running = status;

        if (GodotObject.IsInstanceValid(JetParticles))
        {
            JetParticles.Emitting = Running;
        }

        if (GodotObject.IsInstanceValid(JetSound))
        {
            JetSound.Playing = status;
        }
    }

    public void SideThrusters(float dir)
    {
        bool emitting = false;

        if (!BlockedSides[0])
        {
            bool emitLeft = dir < 0;
            if (GodotObject.IsInstanceValid(LeftSideJetParticles))
            {
                LeftSideJetParticles.Emitting = emitLeft;
            }
            if (emitLeft) emitting = true;
        }

        if (!BlockedSides[1])
        {
            bool emitRight = dir > 0;
            if (GodotObject.IsInstanceValid(RightSideJetParticles))
            {
                RightSideJetParticles.Emitting = emitRight;
            }
            if (emitRight) emitting = true;
        }

        if (GodotObject.IsInstanceValid(SideJetSound) && SideJetSound.Playing != emitting)
        {
            SideJetSound.Playing = emitting;
        }
    }

    public void GetBlockedSides()
    {
        if (Ship == null) return;

        var tilemapCoords = (Vector2I)Get("tilemap_coords");

        // Spatial checks: Left (-X), Up (-Y), Right (+X), Down (+Y)
        bool hasLeft = Ship.Call("get_tile", tilemapCoords + new Vector2I(-1, 0)).Obj != null;
        bool hasUp = Ship.Call("get_tile", tilemapCoords + new Vector2I(0, -1)).Obj != null;
        bool hasRight = Ship.Call("get_tile", tilemapCoords + new Vector2I(1, 0)).Obj != null;
        bool hasDown = Ship.Call("get_tile", tilemapCoords + new Vector2I(0, 1)).Obj != null;

        // Map perpendicular sides based on thruster exhaust heading
        switch (Direction)
        {
            case 0: // Exhaust Right -> Left is Up, Right is Down
                BlockedSides[0] = hasUp;
                BlockedSides[1] = hasDown;
                break;
            case 1: // Exhaust Down -> Left is Right, Right is Left
                BlockedSides[0] = hasRight;
                BlockedSides[1] = hasLeft;
                break;
            case 2: // Exhaust Left -> Left is Down, Right is Up
                BlockedSides[0] = hasDown;
                BlockedSides[1] = hasUp;
                break;
            case 3: // Exhaust Up -> Left is Left, Right is Right
                BlockedSides[0] = hasLeft;
                BlockedSides[1] = hasRight;
                break;
        }

        var blockedCells = new List<Vector2I>();

        switch (Direction)
        {
            case 0:
                for (int x = 1; x <= _requiredSpace; x++)
                    blockedCells.Add(new Vector2I(x, 0));
                break;
            case 1:
                for (int y = 1; y <= _requiredSpace; y++)
                    blockedCells.Add(new Vector2I(0, y));
                break;
            case 2:
                for (int x = -_requiredSpace; x < 0; x++)
                    blockedCells.Add(new Vector2I(x, 0));
                break;
            case 3:
                for (int y = -_requiredSpace; y < 0; y++)
                    blockedCells.Add(new Vector2I(0, y));
                break;
        }

        Blocked = false;
        foreach (var cell in blockedCells)
        {
            if (Ship.Call("get_tile", tilemapCoords + cell).Obj != null)
            {
                Blocked = true;
                break;
            }
        }

        if (!Blocked && !_thrustContributed)
        {
            var thrustPower = (Godot.Collections.Array)Ship.Get("thrust_power");
            if (thrustPower != null && Direction >= 0 && Direction < thrustPower.Count)
            {
                float currentPower = (float)thrustPower[Direction];
                thrustPower[Direction] = currentPower + Power;
                _thrustContributed = true;
            }
        }
        else if (Blocked && _thrustContributed)
        {
            var thrustPower = (Godot.Collections.Array)Ship.Get("thrust_power");
            if (thrustPower != null && Direction >= 0 && Direction < thrustPower.Count)
            {
                float currentPower = (float)thrustPower[Direction];
                thrustPower[Direction] = Mathf.Max(0f, currentPower - Power);
                _thrustContributed = false;
            }
        }

        SetStatus(false);
    }
}