using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class ShipPart : Node2D
{
    private dynamic N(string path) => GetNode(path);


    public dynamic _hitbox;

    public float: durability_max;
    // TODO: get: return durability_max
    // TODO: set(value):
    // TODO: durability_max = value
    // TODO: durability_current = min(durability_current, durability_max)

    public dynamic durability_current; // TODO: manual property translation


    public float mass;

    public Ship ship;

    public Vector2I tilemap_coords;



    public void init(dynamic _ship, Vector2I _coords, float _durability = 60, float _mass = 1)
    {
    ship = _ship;
    tilemap_coords = _coords;
    durability_max = _durability;
    durability_current = _durability;
    mass = _mass;

    ship.mass += mass;

    _reset_hitbox.CallDeferred();

    }

    public void destroy()
    {
    remove();

    }

    public void remove()
    {
    ship.mass -= mass;
    queue_free();

    }

    public void _reset_hitbox()
    {
    _hitbox = get_node_or_null("Hitbox");
    if (_hitbox != null): _hitbox.Position = Vector2.Zero;
    {
        }
    }

}