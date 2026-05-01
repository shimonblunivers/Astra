using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class Door : InteractableShipPart
{
    private dynamic N(string path) => GetNode(path);



    public StaticBody2D walkway;
    // onready: walkway = GetNode<StaticBody2D>("Hitbox/StaticBody2DWalkway");
    public AudioStreamPlayer2D open_sound;
    // onready: open_sound = GetNode<AudioStreamPlayer2D>("Sound/DoorOpen");
    public AudioStreamPlayer2D close_sound;
    // onready: close_sound = GetNode<AudioStreamPlayer2D>("Sound/DoorClose");
    public AnimatedSprite2D animated_sprite;
    // onready: animated_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

    public Node2D mouse_hitbox;
    // onready: mouse_hitbox = GetNode<Node2D>("Hitbox/Area/MouseHitbox");
    public Node2D door_area;
    // onready: door_area = GetNode<Node2D>("Hitbox/Area/Area2D");

    public state := "closed";

    public obstructed := false;
    public locked := false;

    public int collision_layer = 1;
    public int occluder_light_mask = 1;

    public int interact_range = 300;

    public bool is_operating = false;

    public Array obstructers = new Array();


    public void init(dynamic _ship, Vector2I _coords, float _durability = 100, float _mass = 3)
    {
    _ship.interactables.append(this);
    super(_ship, _coords, _durability, _mass);

    }

    public override void _Ready()
    {
    if (direction == "vertical")
    {
        }
    rotation_degrees = 90;
    animated_sprite.connect("frame_changed", _on_frame_changed);
    hitboxes_to_shift.append(mouse_hitbox);

    }

    public void update_sprites()
    {
    if (state == "open")
    {
        }
    animated_sprite.play("open");
    }
    else
    {
        }
    animated_sprite.play_backwards("open");

    }

    public void open()
    {
    if (locked)
    {
        }
    return
    is_operating = true;
    state = "open";
    open_sound.PitchScale = GD.RandRange(0.9, 1.1);
    open_sound.play();
    update_sprites();
    ship.opened_doors.append(tilemap_coords);

    N("AutocloseTimer").start();

    }

    public void close()
    {
    is_operating = true;
    state = "closed";
    close_sound.PitchScale = GD.RandRange(0.9, 1.1);
    close_sound.play();
    update_sprites();
    walkway.set_collision_layer_value(collision_layer, true);
    ship.opened_doors.erase(tilemap_coords);

    }

    public void _on_frame_changed()
    {

    switch (animated_sprite.Frame)
    {
        }
    3: #  OTEVŘENO;
    is_operating = state != "open";

    N("Hitbox/AnimatedOccluders/0left").occluder_light_mask = 0;
    N("Hitbox/AnimatedOccluders/0right").occluder_light_mask = 0;
    N("Hitbox/AnimatedOccluders/1left").occluder_light_mask = 0;
    N("Hitbox/AnimatedOccluders/1right").occluder_light_mask = 0;
    N("Hitbox/AnimatedOccluders/2center").occluder_light_mask = 0;

    N("Hitbox/AnimatedHitbox/0left").set_collision_layer_value(collision_layer, false);
    N("Hitbox/AnimatedHitbox/0right").set_collision_layer_value(collision_layer, false);
    N("Hitbox/AnimatedHitbox/1left").set_collision_layer_value(collision_layer, false);
    N("Hitbox/AnimatedHitbox/1right").set_collision_layer_value(collision_layer, false);
    case 2:
        }
    N("Hitbox/AnimatedOccluders/0left").occluder_light_mask = occluder_light_mask;
    N("Hitbox/AnimatedOccluders/0right").occluder_light_mask = occluder_light_mask;
    N("Hitbox/AnimatedOccluders/1left").occluder_light_mask = 0;
    N("Hitbox/AnimatedOccluders/1right").occluder_light_mask = 0;
    N("Hitbox/AnimatedOccluders/2center").occluder_light_mask = 0;
    if (state == "open")
    {
        walkway.set_collision_layer_value(collision_layer, false);
        N("Hitbox/AnimatedHitbox/0left").set_collision_layer_value(collision_layer, true);
        N("Hitbox/AnimatedHitbox/0right").set_collision_layer_value(collision_layer, true);
        N("Hitbox/AnimatedHitbox/1left").set_collision_layer_value(collision_layer, false);
        N("Hitbox/AnimatedHitbox/1right").set_collision_layer_value(collision_layer, false);
        }
    case 1:
        }
    N("Hitbox/AnimatedOccluders/0left").occluder_light_mask = occluder_light_mask;
    N("Hitbox/AnimatedOccluders/0right").occluder_light_mask = occluder_light_mask;
    N("Hitbox/AnimatedOccluders/1left").occluder_light_mask = occluder_light_mask;
    N("Hitbox/AnimatedOccluders/1right").occluder_light_mask = occluder_light_mask;
    N("Hitbox/AnimatedOccluders/2center").occluder_light_mask = 0;
    if (state == "open")
    {
        N("Hitbox/AnimatedHitbox/0left").set_collision_layer_value(collision_layer, true);
        N("Hitbox/AnimatedHitbox/0right").set_collision_layer_value(collision_layer, true);
        N("Hitbox/AnimatedHitbox/1left").set_collision_layer_value(collision_layer, true);
        N("Hitbox/AnimatedHitbox/1right").set_collision_layer_value(collision_layer, true);
        }
    0: # ZAVŘENO;
    N("Hitbox/AnimatedOccluders/0left").occluder_light_mask = occluder_light_mask;
    N("Hitbox/AnimatedOccluders/0right").occluder_light_mask = occluder_light_mask;
    N("Hitbox/AnimatedOccluders/1left").occluder_light_mask = occluder_light_mask;
    N("Hitbox/AnimatedOccluders/1right").occluder_light_mask = occluder_light_mask;
    N("Hitbox/AnimatedOccluders/2center").occluder_light_mask = occluder_light_mask;
    is_operating = state != "closed";


    }

    public void _interact()
    {
    if (Player.main_player.GlobalPosition.distance_to(global_position) > interact_range)
    {
        }
    return
    if (is_operating)
    {
        }
    return

    // var tile = ship.get_tile(tilemap_coords + Vector2i(1, 0))
    // print(tile)
    // if tile != null:
    // tile.destroy()
    // ship.wall_tile_map.set_cell(0, tilemap_coords + Vector2i(1, 0), -1)

    if (state == "open")
    {
        }
    if (obstructed)
    {
        }
    return
    close();
    }
    else
    {
        }
    open();

    }

    public void _on_area_2d_area_entered(Area2D area)
    {
    if ((area.is_in_group("CharacterInteractArea")))
    {
        }
    obstructed = true;
    obstructers.append(area);


    }

    public void _on_area_2d_area_exited(Area2D area)
    {
    if ((area.is_in_group("CharacterInteractArea")))
    {
        }
    obstructers.erase(area);
    if obstructers.Count == 0: obstructed = false;


    }

    public void _on_mouse_hitbox_input_event(Node _viewport, InputEvent event, int _shape_idx)
    {
    if (event is InputEventMouseButton && event.ButtonMask == 1 && Player.main_player.alive)
    {
        }
    interact();


    }

    public void _on_autoclose_timer_timeout()
    {
    if (state == "open")
    {
        }
    if (!obstructed)
    {
        }
    close();
    }
    else
    {
        }
    N("AutocloseTimer").start();

    }

}