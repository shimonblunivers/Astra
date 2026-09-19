using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class InteractableShipPart : ShipPart
{
    private static readonly PackedScene SceneButtonIndicator = GD.Load<PackedScene>("res://UI/ButtonIndicator.tscn");

    [Export] public string Direction { get; set; } = "horizontal";

    public Player PlayerInRange { get; set; } = null;
    public List<Node2D> HitboxesToShift { get; set; } = new List<Node2D>();

    public ButtonIndicator ButtonIndicatorInstance { get; set; }
    public bool ButtonIndicatorVisible { get; set; } = false;

    public override void Init(Ship targetShip, Vector2I coords, float durability = 10f, float mass = 1f)
    {
        base.Init(targetShip, coords, durability, mass);

        ButtonIndicatorInstance = SceneButtonIndicator.Instantiate<ButtonIndicator>();
        ButtonIndicatorInstance.Visible = ButtonIndicatorVisible;

        Callable.From(() =>
        {
            AddChild(ButtonIndicatorInstance);
            ButtonIndicatorInstance.Call("init", this);
        }).CallDeferred();
    }

    public override void _PhysicsProcess(double delta)
    {
        bool hasPlayer = PlayerInRange != null;

        // Fixed: Corrected GDScript operator precedence bug (!player_in_range == null)
        if (ButtonIndicatorVisible != hasPlayer)
        {
            ButtonIndicatorVisible = hasPlayer;

            if (GodotObject.IsInstanceValid(ButtonIndicatorInstance))
            {
                ButtonIndicatorInstance.Visible = ButtonIndicatorVisible;
            }
        }
    }

    public override void _Process(double delta)
    {
        if (Ship == null) return;

        Vector2 diff = (Vector2)Ship.Get("difference_in_position");
        Vector2 shiftedPos = (-diff).Rotated(-(float)GlobalRotation);

        foreach (var hitbox in HitboxesToShift)
        {
            if (GodotObject.IsInstanceValid(hitbox))
            {
                hitbox.Position = shiftedPos;
            }
        }
    }

    public void Interact()
    {
        OnInteract();
    }

    /// <summary>
    /// Virtual method intended to be overridden by inheriting interactive ship parts.
    /// </summary>
    protected virtual void OnInteract()
    {
    }
}