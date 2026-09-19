using Godot;

[GlobalClass]
public partial class ShipPart : Node2D
{
    private Node2D _hitbox;
    private float _durabilityMax;
    private float _durabilityCurrent;
    private bool _removed = false;

    public float DurabilityMax
    {
        get => _durabilityMax;
        set
        {
            _durabilityMax = value;
            if (_durabilityMax >= 0)
            {
                _durabilityCurrent = Mathf.Min(_durabilityCurrent, _durabilityMax);
            }
        }
    }

    public float DurabilityCurrent
    {
        get => _durabilityMax >= 0 ? _durabilityCurrent : 6942069f;
        set
        {
            if (_durabilityMax >= 0)
            {
                _durabilityCurrent = Mathf.Min(value, _durabilityMax);
            }
            else
            {
                _durabilityCurrent = value;
            }
        }
    }

    // Dynamic property aliases for GDScript compatibility
    public float durability_max
    {
        get => DurabilityMax;
        set => DurabilityMax = value;
    }

    public float durability_current
    {
        get => DurabilityCurrent;
        set => DurabilityCurrent = value;
    }

    [Export] public float Mass { get; set; }
    public Ship Ship { get; set; }
    public Vector2I TilemapCoords { get; set; }

    public float mass
    {
        get => Mass;
        set => Mass = value;
    }

    public Ship ship
    {
        get => Ship;
        set => Ship = value;
    }

    public Vector2I tilemap_coords
    {
        get => TilemapCoords;
        set => TilemapCoords = value;
    }

    public virtual void Init(Ship targetShip, Vector2I coords, float durability = 60f, float mass = 1f)
    {
        Ship = targetShip;
        TilemapCoords = coords;
        DurabilityMax = durability;
        DurabilityCurrent = durability;
        Mass = mass;

        if (Ship != null)
        {
            Ship.Mass += Mass;
        }

        Callable.From(ResetHitbox).CallDeferred();
    }

    public virtual void Destroy()
    {
        Remove();
    }

    public virtual void Remove()
    {
        if (_removed) return;
        _removed = true;

        if (GodotObject.IsInstanceValid(Ship))
        {
            Ship.Mass = Mathf.Max(0.01f, Ship.Mass - Mass);
        }

        QueueFree();
    }

    protected void ResetHitbox()
    {
        _hitbox = GetNodeOrNull<Node2D>("Hitbox");
        if (_hitbox != null)
        {
            _hitbox.Position = Vector2.Zero;
        }
    }
}