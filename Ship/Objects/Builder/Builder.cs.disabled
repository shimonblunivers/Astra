using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class Builder : InteractableShipPart
{
    private dynamic N(string path) => GetNode(path);


    public dynamic connector_finder_hitbox;
    // onready: connector_finder_hitbox = GetNode<Node>("ConnectorFinder/CollisionShape2D");

    public bool controlled = false;

    public Connector connector;


    public void init(dynamic _ship, Vector2I _coords, float _durability = 10, float _mass = 1)
    {
    _coords = Vector2I(_coords.x / 4, _coords.y / 4);
    super(_ship, _coords, _durability, _mass);

    }

    public override void _Ready()
    {
    connector_finder_hitbox.shape.radius = 1;
    connector = null;

    }

    public void _interact()
    {
    if (connector.connected_to == null)
    {
        }
    World.instance.open_editor(this);

    }

    public void play_sound()
    {
    N("DeploySound").play();

    }

    public override void _Process(double delta)
    {
    if connector == null: connector_finder_hitbox.shape.radius += 1;

    }

    public Vector2 get_spawn_position()
    {
    return connector.GlobalPosition + Vector2(80, -80).rotated(Mathf.DegToRad(connector.GlobalRotationDegrees)) + ship.difference_in_position

    }

    public float get_ship_rotation()
    {
    return Mathf.DegToRad(connector.GlobalRotationDegrees)

    }

    public void _on_area_area_entered(Area2D area)
    {
    if (area.is_in_group("PlayerArea"))
    {
        }
    player_in_range = area.get_owner();
    player_in_range.hovering_controllables.append(this);

    }

    public void _on_area_area_exited(Area2D area)
    {
    if (area.is_in_group("PlayerArea"))
    {
        }
    player_in_range.hovering_controllables.erase(this);
    player_in_range = null;

    }

    public void _on_connector_finder_body_entered(Node2D body)
    {
    if (connector == null)
    {
        }
    body = body.get_parent();
    if (body.ship == ship && body is Connector)
    {
        }
    connector = body;
    connector_finder_hitbox.set_deferred("disabled", true);
    }

}