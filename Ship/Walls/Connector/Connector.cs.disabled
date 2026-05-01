using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class Connector : ShipPart
{
    private dynamic N(string path) => GetNode(path);



    public AnimatedSprite2D sprite;
    // onready: sprite = GetNode<AnimatedSprite2D>("Sprite2D");
    public PinJoint2D pin_joint1;
    // onready: pin_joint1 = GetNode<PinJoint2D>("PinJoint2D1");
    public PinJoint2D pin_joint2;
    // onready: pin_joint2 = GetNode<PinJoint2D>("PinJoint2D2");

    public int layer = 0;

    public Connector connected_to = null;

    public Array connectors_in_range = new Array();


    public void init(dynamic _ship, Vector2I _coords, float _durability = 200, float _mass = 5)
    {
    super(_ship, _coords, _durability, _mass);
    ship.connectors.append(this);

    }

    public override void _Ready()
    {
    pin_joint1.node_a = ship.get_path();
    pin_joint2.node_a = ship.get_path();


    }

    public void connect_to(Connector to)
    {
    if (connected_to != null)
    {
        }
    connected_to.pin_joint1.node_b = "";
    connected_to.pin_joint2.node_b = "";
    connected_to.connected_to = null;
    connected_to.sprite.Frame = 0;
    if (to != null)
    {
        }
    pin_joint1.node_b = to.ship.get_path();
    pin_joint2.node_b = to.ship.get_path();
    sprite.Frame = 1;

    connected_to = to;
    connected_to.pin_joint1.node_b = this.ship.get_path();
    connected_to.pin_joint2.node_b = this.ship.get_path();
    connected_to.sprite.Frame = 1;
    }
    else
    {
        }
    sprite.Frame = 0;
    connected_to = null;
    pin_joint1.node_b = "";
    pin_joint2.node_b = "";


    }

    public void _on_connector_area_area_entered(Area2D area)
    {
    if (connected_to == null)
    {
        }
    dynamic body = area.get_parent();
    if (body is Connector)
    {
        }
    connectors_in_range.append(body);


    }

    public void _on_connector_area_area_exited(Area2D area)
    {
    dynamic body = area.get_parent();
    if (body is Connector)
    {
        }
    connectors_in_range.erase(body);
    }

}