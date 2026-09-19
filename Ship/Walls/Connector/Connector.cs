using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class Connector : ShipPart
{
    [Export] public AnimatedSprite2D Sprite { get; set; }
    [Export] public PinJoint2D PinJoint1 { get; set; }
    [Export] public PinJoint2D PinJoint2 { get; set; }

    public int Layer { get; set; } = 0;
    public Connector ConnectedTo { get; set; } = null;
    public List<Connector> ConnectorsInRange { get; } = new List<Connector>();

    public override void Init(Ship targetShip, Vector2I coords, float durability = 200f, float mass = 5f)
    {
        base.Init(targetShip, coords, durability, mass);

        var connectorsList = (Godot.Collections.Array)Ship?.Get("connectors");
        connectorsList?.Add(this);
    }

    public override void _Ready()
    {
        base._Ready();

        Sprite ??= GetNodeOrNull<AnimatedSprite2D>("Sprite2D");
        PinJoint1 ??= GetNodeOrNull<PinJoint2D>("PinJoint2D1");
        PinJoint2 ??= GetNodeOrNull<PinJoint2D>("PinJoint2D2");

        if (Ship != null)
        {
            NodePath shipPath = Ship.GetPath();
            if (PinJoint1 != null) PinJoint1.NodeA = shipPath;
            if (PinJoint2 != null) PinJoint2.NodeA = shipPath;
        }
    }

    public void ConnectTo(Connector to)
    {
        if (ConnectedTo != null)
        {
            if (ConnectedTo.PinJoint1 != null) ConnectedTo.PinJoint1.NodeB = new NodePath();
            if (ConnectedTo.PinJoint2 != null) ConnectedTo.PinJoint2.NodeB = new NodePath();
            ConnectedTo.ConnectedTo = null;
            if (ConnectedTo.Sprite != null) ConnectedTo.Sprite.Frame = 0;

            if (PinJoint1 != null) PinJoint1.NodeB = new NodePath();
            if (PinJoint2 != null) PinJoint2.NodeB = new NodePath();
        }

        if (to != null && to.Ship != null && Ship != null)
        {
            NodePath toShipPath = to.Ship.GetPath();
            if (PinJoint1 != null) PinJoint1.NodeB = toShipPath;
            if (PinJoint2 != null) PinJoint2.NodeB = toShipPath;
            if (Sprite != null) Sprite.Frame = 1;

            ConnectedTo = to;

            NodePath thisShipPath = Ship.GetPath();
            if (ConnectedTo.PinJoint1 != null) ConnectedTo.PinJoint1.NodeB = thisShipPath;
            if (ConnectedTo.PinJoint2 != null) ConnectedTo.PinJoint2.NodeB = thisShipPath;
            if (ConnectedTo.Sprite != null) ConnectedTo.Sprite.Frame = 1;
        }
        else
        {
            if (Sprite != null) Sprite.Frame = 0;
            ConnectedTo = null;
            if (PinJoint1 != null) PinJoint1.NodeB = new NodePath();
            if (PinJoint2 != null) PinJoint2.NodeB = new NodePath();
        }
    }

    public void OnConnectorAreaAreaEntered(Area2D area)
    {
        if (ConnectedTo == null && area != null)
        {
            var body = area.GetParent();
            if (body is Connector candidate && !ConnectorsInRange.Contains(candidate))
            {
                ConnectorsInRange.Add(candidate);
            }
        }
    }

    public void OnConnectorAreaAreaExited(Area2D area)
    {
        if (area == null) return;

        var body = area.GetParent();
        if (body is Connector candidate)
        {
            ConnectorsInRange.Remove(candidate);
        }
    }
}