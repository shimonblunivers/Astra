using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class Helm : InteractableShipPart
{
    private dynamic N(string path) => GetNode(path);


    public bool controlled = false;


    public void init(dynamic _ship, Vector2I _coords, float _durability = 10, float _mass = 1)
    {
    _coords = Vector2I(_coords.x / 4, _coords.y / 4)                                   # MAYBE DOESNT WORK!!;
    super(_ship, _coords, _durability, _mass);

    }

    public void _interact()
    {
    if (controlled)
    {
        }
    player_in_range.control_ship(null);
    player_in_range.controllables_in_use.erase(this);
    }
    else
    {
        }
    player_in_range.control_ship(ship);
    player_in_range.controllables_in_use.append(this);
    controlled = !controlled;

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

}