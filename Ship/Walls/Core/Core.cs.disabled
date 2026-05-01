using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class Core : InteractableShipPart
{
    private dynamic N(string path) => GetNode(path);


    public AnimatedSprite2D sprite;
    // onready: sprite = GetNode<AnimatedSprite2D>("Sprite2D");

    public int layer = 0;


    public void init(dynamic _ship, Vector2I _coords, float _durability = 200, float _mass = 5)
    {
    super(_ship, _coords, _durability, _mass);

    }

    public void _interact()
    {
    World.save_file.save_world(false);

    }

    public void _on_area_2d_area_entered(Area2D area)
    {
    if (area.is_in_group("PlayerArea"))
    {
        }
    player_in_range = area.get_owner();
    player_in_range.hovering_controllables.append(this);

    }

    public void _on_area_2d_area_exited(Area2D area)
    {
    if (area.is_in_group("PlayerArea"))
    {
        }
    player_in_range.hovering_controllables.erase(this);
    player_in_range = null;
    }

}