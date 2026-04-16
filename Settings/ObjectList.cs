using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class ObjectList : Node
{
    private dynamic N(string path) => GetNode(path);


    public static dynamic world;

    public bool started_game = false;


    public Ship get_closest_ship(Vector2 from_global_pos, dynamic ships = Ship.ships)
    {
    if ships.Count == 0: return null;
    dynamic closest = ships[0];
    foreach (var ship in ships)
    {
        }
    if (closest.get_closest_point(from_global_pos).distance_to(from_global_pos) > ship.get_closest_point(from_global_pos).distance_to(from_global_pos))
    {
        }
    closest = ship;
    return closest

    }

    public override void _Ready()
    {
    Item.load_items();
    world = World.instance;

    }

    public void get_saveable_items()
    {
    Array list = new Array();
    list.append_array(Ship.ships);
    list.append_array(NPC.npcs);
    list.append_array(Item.items);
    return list
    }

}