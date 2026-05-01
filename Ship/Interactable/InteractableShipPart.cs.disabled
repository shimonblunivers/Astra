using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class InteractableShipPart : ShipPart
{
    private dynamic N(string path) => GetNode(path);


    public direction := "horizontal";

    public dynamic player_in_range = null;

    public Array hitboxes_to_shift = new Array();

    public ButtonIndicator button_indicator;
    public static readonly PackedScene scene_button_indicator = GD.Load<PackedScene>("res://UI/ButtonIndicator.tscn");

    public bool button_indicator_visible = false;


    public void init(dynamic _ship, Vector2I _coords, float _durability = 10, float _mass = 1)
    {
    super(_ship, _coords, _durability, _mass);
    button_indicator = scene_button_indicator.Instantiate();
    CallDeferred("add_child", button_indicator);
    button_indicator.init(this);
    button_indicator.Visible = button_indicator_visible;

    }

    public override void _PhysicsProcess(double delta)
    {
    if (button_indicator_visible == (player_in_range == null))
    {
        }
    button_indicator_visible = !player_in_range == null;
    button_indicator.Visible = button_indicator_visible;

    }

    public override void _Process(double delta)
    {
    foreach (var hitbox in hitboxes_to_shift)
    {
        }
    hitbox.Position = (- ship.difference_in_position).rotated(-global_rotation);
    // if direction == "horizontal":
    // hitbox.position = (- ship.difference_in_position).rotated(-global_rotation)
    // elif direction == "vertical":
    // hitbox.position = (- ship.difference_in_position).rotated(-global_rotation)

    }

    public void interact()
    {
    _interact();

    }

    public void _interact()
    {
    // pass

    }

}