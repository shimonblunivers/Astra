using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class Player : Character
{
    private dynamic N(string path) => GetNode(path);


    public AnimatedSprite2D animated_sprite;
    // onready: animated_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

    public AudioStreamPlayer2D walk_sound;
    // onready: walk_sound = GetNode<AudioStreamPlayer2D>("Sounds/Walk");
    public Camera2D camera;
    // onready: camera = GetNode<Camera2D>("Camera2D");
    public PointLight2D vision;
    // onready: vision = GetNode<PointLight2D>("Vision/Light");

    public dynamic interact_area;
    // onready: interact_area = GetNode<Node>("InteractArea");

    public dynamic respawn_timer;
    // onready: respawn_timer = GetNode<Node>("RespawnTimer");

    public dynamic pickup;
    // onready: pickup = GetNode<Node>("Pickup");

    public static Player main_player;

    public dynamic currency; // TODO: manual property translation

    public _sprite_dir := 69;

    public dynamic ship_controlled = null;

    public float normal_zoom = 1;

    public float normal_vision = 4;
    public float driving_vision = 0.4;

    public bool suit = true;

    public float use_range = 1000;

    public dynamic acceleration = Vector2(0, 0);
    public dynamic _old_position = Vector2(0, 0);

    public hovering_controllables := [];

    public controllables_in_use := [];

    public passenger_on := [];
    public Ship parent_ship = null;

    public int dim_acceleration_for_frames = 0;

    public dynamic camera_difference = Vector2.Zero;
    [Signal] public delegate void CurrencyUpdatedSignalEventHandler();

    public float _damage_timer = 0;
    public float _regen_timer = 0;

    public dynamic owned_ship; // TODO: manual property translation

    public invincible := false;

    public bool _locked_rotating = false;

    public int update_range = 10000;

    // TODO: ✅ Make player controling zoom out so it's in the center of ship and is scalable with the ship size

    // TODO: ✅ Add floating velocity

    // TODO: ✅ Edit player vision so object that are in the dark cannot be seen (Using lights as mask)

    // TODO: ✅ Add Damage & Death

    // TODO: ✅ Fix wrong hitbox while ship moving

    // TODO: ✅ Fix Michael Jackson walking

    // TODO: ✅ Change sounds according to walking terrain

    // TODO: ✅ Add load/save

    // TODO: ✅ Add animations


    public Tween turn_tween;

    public dynamic collisionpos = Vector2.Zero;


    public void add_currency(int amount)
    {
    currency += amount;
    UIManager.currency_change_effect(amount);

    }

    public void floating()
    {
    return passenger_on.Count == 0

    }

    public void get_in(dynamic ship)
    {
    dim_acceleration_for_frames = 5;
    if (ship in passenger_on): return;
    {
        }
    passenger_on.append(ship);
    if passenger_on.Count == 1: rotate_to_ship();
    // print(acceleration, " ; ", ship.difference_in_position)
    if (max_impact_velocity < (acceleration - ship.difference_in_position).length())
    {
        }
    kill();

    }

    public void rotate_to_ship()
    {
    if _locked_rotating: return;
    if turn_tween: turn_tween.kill();

    dynamic turn_speed = abs(rotation_degrees / 150);
    turn_tween = create_tween();

    if (rotation_degrees > 180)
    {
        }
    turn_tween.tween_property(this, "rotation_degrees", 360, turn_speed);
    }
    else
    {
        }
    turn_tween.tween_property(this, "rotation_degrees", 0, turn_speed);


    }

    public void get_off(dynamic ship)
    {
    if turn_tween: turn_tween.kill();
    passenger_on.erase(ship);

    }

    public void change_ship(dynamic ship)
    {
    if ship == null: return;
    parent_ship = ship;
    // parent_ship.hitbox.position = (parent_ship.difference_in_position).rotated(parent_ship.global_rotation)
    CallDeferred("reparent", ship.passengers_node);
    if (!floating())
    {
        }
    _change_ship_rotate.CallDeferred();
    // parent_ship.make_invulnerable()

    }

    public void _change_ship_rotate()
    {
    rotate_to_ship();
    _locked_rotating = true;
    N("LockRotationTimer").start();

    }

    public override void _UnhandledInput(InputEvent @event)
    {
    if (Options.DEVELOPMENT_MODE)
    {
        }
    if (event.is_action_pressed("teleport_to_quest"))
    {
        }
    if (QuestManager.get_quest(QuestManager.highlighted_quest_id))
    {
        if (QuestManager.get_quest(QuestManager.highlighted_quest_id).get_target())
        {
            }
        godmode = true;
        teleport(QuestManager.get_quest(QuestManager.highlighted_quest_id).get_target().GlobalPosition);;
        }
    else
    {
        godmode = true;
        teleport(ShipManager.main_station.GlobalPosition);
        }
    if (event.is_action_pressed("teleport_to_ship_editor"))
    {
        }
    teleport(Vector2(3300, -3100));

    if (event.is_action_pressed("debug_die"))
    {
        }
    World.save_file.save_world(true);
    // parent_ship.delete()

    if (event.is_action_pressed("debug_spawn"))
    {
        }
    // spawn()
    // Item.spawn(Item.types["Chip"], get_global_mouse_position())
    // ShipManager.spawn_ship(get_global_mouse_position(), "small_shuttle")
    add_currency(1500);



    if (event.is_action_pressed("development_mode"))
    {
        }
    Options.DEVELOPMENT_MODE = !Options.DEVELOPMENT_MODE;
    UIManager.instance.loading_screen_node.Visible = !Options.DEVELOPMENT_MODE;
    UIManager.instance.floating.Visible = Options.DEVELOPMENT_MODE;
    UIManager.instance.player_position.Visible = Options.DEVELOPMENT_MODE;

    if (alive)
    {
        }
    if (event.is_action_pressed("game_control"))
    {
        }
    foreach (var controllable in hovering_controllables)
    {
        controllable.interact();
        return
        }
    foreach (var controllable in controllables_in_use)
    {
        if controllable in hovering_controllables: continue;
        controllable.player_in_range = this;
        controllable.interact();
        controllable.player_in_range = null;
        return

        }
    }

    public void teleport(Vector2 pos)
    {
    global_position = pos;
    _old_position = World.instance.get_distance_from_center(global_position);

    }

    public void spawn(dynamic _rotation = null)
    {
    if !alive: animated_sprite.play("Idle");
    alive = true;
    spawned = true;
    health = max_health;
    if _rotation != null: global_rotation = _rotation;
    change_ship(ObjectList.get_closest_ship(global_position));
    _locked_rotating = false;
    global_position = pos - World.instance._center_of_universe;
    _old_position = World.instance.get_distance_from_center(global_position);

    invincible = true;
    N("InvincibilityTimer").start();
    health_updated_signal.emit();

    }

    public override async Task _Ready()
    {
    super();
    main_player = this;

    spawn_point = Vector2(7777, -69);

    nickname = "Samuel";
    await get_tree().process_frame # WAIT FOR THE WORLD TO LOAD AND THE POSITION TO UPDATE // WAIT FOR NEXT FRAME;
    animated_sprite.play("Idle");


    World.save_file.load_world();


    }

    public void kill()
    {
    if !alive || !spawned || invincible || godmode: return;
    health = 0;
    alive = false;

    if ship_controlled != null: control_ship(ship_controlled);

    animated_sprite.play("Death");
    health_updated_signal.emit();
    died_signal.emit();
    respawn_timer.start();

    }

    public override void _Process(double delta)
    {
    if (alive)
    {
        }
    if (floating())
    {
        }
    _regen_timer = 0;
    _damage_timer += delta * 2;
    if (_damage_timer >= 1)
    {
        _damage_timer = 0;
        damage(5);

        }
    else
    {
        }
    _damage_timer = 0;
    if (health != max_health)
    {
        _regen_timer += delta * 3;
        if (_regen_timer >= 1)
        {
            }
        damage(-1);
        _regen_timer = 0;

        }
    pickup.Position = (- acceleration).rotated(-global_rotation);


    }

    public void _in_physics(float delta)
    {
    // print("Player position: ", position)
    if (passenger_on.Count == 1 && passenger_on[0] != parent_ship)
    {
        }
    change_ship(passenger_on[0]);

    if (ship_controlled == null)
    {
        }
    _move(delta);
    }
    else
    {
        }
    camera.offset = camera_difference.rotated(global_rotation);


    // if parent_ship != null: World.instance.shift_origin(-parent_ship.global_transform.origin) # Moving the world origin to remove flickering bugs

    }

    public void control_ship(dynamic ship)
    {
    if (ship != null)
    {
        }
    ship_controlled = ship;
    walk_sound.stop();
    _sprite_dir = 0;
    animated_sprite.FlipH = false;
    animated_sprite.play("Idle");

    ship.start_controlling(this);
    change_view(0);
    }
    else
    {
        }
    change_view(1);
    if ship_controlled != null: ship_controlled.stop_controlling();
    ship_controlled = null;



    }

    public void _move(float _delta)
    {
    direction := Input.get_vector("ui_left", "ui_right", "ui_up", "ui_down") # Get input keys;
    rotation_direction := Input.get_axis("game_turn_left","game_turn_right");
    running := Input.get_action_strength("game_run");

    _sound_pitch_range := [0.9, 1.1] # Sound variation;

    if (!alive)
    {
        }
    direction = Vector2.Zero # Death check;
    rotation_direction = 0;

    acceleration = World.instance.get_distance_from_center(global_position) - _old_position # Acceleration get by the difference of the position;
    _old_position = World.instance.get_distance_from_center(global_position);

    // print("f: ", floating(), "ACC: ", acceleration)

    // if abs(acceleration.x) > Limits.VELOCITY_MAX or abs(acceleration.y) > Limits.VELOCITY_MAX: # Speed limitation, need to redo that tho
    // var new_speed = acceleration.normalized()
    // new_speed *= Limits.VELOCITY_MAX
    // acceleration = new_speed

    velocity = (direction * (SPEED + RUN_SPEED_MODIFIER * running)).rotated(global_rotation) # velocity // the _fix_position is help variable made to remove bug;

    if (parent_ship != null)
    {
        }
    if floating(): 	# If outside of the ship;
    rotate(Mathf.DegToRad(TURN_SPEED * rotation_direction));
    legs.Position = legs_offset - (acceleration).rotated(-global_rotation) # Counter steering the bug, where every hitbox of the ship shifts;
    if (suit == false)
    {
        velocity = Vector2(0, 0) # No control over the direction u r flying if you don't have a suit;
        }
    else
    {
        velocity *= .01 # Taking the velocity and dividing it by 100, so the player isn't so fast in the space like in ship;

        }
    if (dim_acceleration_for_frames <= 0)
    {
        velocity += (acceleration - parent_ship.difference_in_position) / _delta # Removing the parent_ship (ship he is attached to) velocity, so the acceleration won't throw him into deep space;
        }
    else
    {
        int _dim_factor = 10;
        velocity += (acceleration - parent_ship.difference_in_position) / (_delta * _dim_factor);
        }
    else
    {
        }
    legs.Position = legs_offset - (passenger_on[0].difference_in_position).rotated(-global_rotation) # Again the counter steering against the bug;

    if (dim_acceleration_for_frames > 0)
    {
        }
    dim_acceleration_for_frames -= 1;

    // if get_last_slide_collision():
    // velocity = velocity.move_toward(Vector2(0, 0), 40)
    // print("dampening now  velocity:", velocity)
    // print(direction)
    move_and_slide();

    // animation things down here..

    // elif _fix_position == position && passenger_on[0].linear_velocity != Vector2.ZERO:
    // position += acceleration

    if (!floating() && direction.x < 0)
    {
        }
    if (!walk_sound.Playing && !floating())
    {
        }
    walk_sound.PitchScale = GD.RandRange(_sound_pitch_range[0], _sound_pitch_range[1]);
    walk_sound.play();
    if (_sprite_dir != 1)
    {
        }
    _sprite_dir = 1;
    animated_sprite.FlipH = true;
    animated_sprite.play("WalkToSide");

    }
    else if (!floating() && direction.x > 0)
    {
        }
    if (!walk_sound.Playing && !floating())
    {
        }
    walk_sound.PitchScale = GD.RandRange(_sound_pitch_range[0], _sound_pitch_range[1]);
    walk_sound.play();
    if (_sprite_dir != 2)
    {
        }
    _sprite_dir = 2;
    animated_sprite.FlipH = false;
    animated_sprite.play("WalkToSide");

    }
    else if (!floating() && direction.y > 0)
    {
        }
    if (!walk_sound.Playing && !floating())
    {
        }
    walk_sound.PitchScale = GD.RandRange(_sound_pitch_range[0], _sound_pitch_range[1]);
    walk_sound.play();
    if (_sprite_dir != 3)
    {
        }
    _sprite_dir = 3;
    animated_sprite.FlipH = false;
    animated_sprite.play("WalkDown");

    }
    else if (!floating() && direction.y < 0)
    {
        }
    if (!walk_sound.Playing && !floating())
    {
        }
    walk_sound.PitchScale = GD.RandRange(_sound_pitch_range[0], _sound_pitch_range[1]);
    walk_sound.play();
    if (_sprite_dir != 4)
    {
        }
    _sprite_dir = 4;
    animated_sprite.FlipH = false;
    animated_sprite.play("WalkUp");

    }
    else
    {
        }
    if (alive && _sprite_dir != 0)
    {
        }
    _sprite_dir = 0;
    animated_sprite.FlipH = false;
    animated_sprite.play("Idle");

    }

    public void change_view(int view)
    {
    dynamic tween = create_tween();
    Rect2 ship_rect = Rect2(ship_controlled.get_rect().Position.x * ship_controlled.get_tile_size().x * 5, ship_controlled.get_rect().Position.y * ship_controlled.get_tile_size().y * 5, ship_controlled.get_rect().size.x * ship_controlled.get_tile_size().x * 5, ship_controlled.get_rect().size.y * ship_controlled.get_tile_size().y * 5);
    Vector2 ship_center = ship_rect.size / 2 + ship_rect.Position;
    camera_difference = ship_center - position;
    int duration = 1;

    dynamic ship_size = (max(ship_rect.size.x, ship_rect.size.y) + 2000) * 1.666;
    dynamic cam_size = get_viewport_rect().size.y * 1.25;
    dynamic ship_zoom = 1 / (ship_size / cam_size);

    switch (view)
    {
        }
    case 0:
        }
    tween.parallel().tween_property(camera, "zoom", Vector2(ship_zoom, ship_zoom), duration).set_ease(Tween.EASE_OUT);
    tween.parallel().tween_property(camera, "offset", camera_difference.rotated(global_rotation), duration).set_ease(Tween.EASE_OUT);
    tween.parallel().tween_property(vision, "texture_scale", driving_vision, duration).set_ease(Tween.EASE_OUT);
    case 1:
        }
    camera_difference = Vector2.Zero;
    tween.parallel().tween_property(camera, "zoom", Vector2(normal_zoom, normal_zoom), duration).set_ease(Tween.EASE_OUT);
    tween.parallel().tween_property(camera, "offset", camera_difference, duration).set_ease(Tween.EASE_OUT);
    tween.parallel().tween_property(vision, "texture_scale", normal_vision, duration).set_ease(Tween.EASE_OUT);

    }

    public void _on_pickup_area_entered(Area2D area)
    {
    if (!area.is_in_group("CharacterInteractArea"))
    {
        }
    area.get_parent().can_pickup = true;

    }

    public void _on_pickup_area_exited(Area2D area)
    {
    if (!area.is_in_group("CharacterInteractArea"))
    {
        }
    area.get_parent().can_pickup = false;

    }

    public void deleting_ship(Ship _ship)
    {
    if (_ship == parent_ship)
    {
        }
    CallDeferred("reparent", World.instance);

    }

    public void _on_health_updated_signal()
    {
    UIManager.instance.player_health_updated_signal();


    }

    public void _on_respawn_timer_timeout()
    {
    World.save_file.load_world();

    }

    public void _on_invincibility_timer_timeout()
    {
    invincible = false;


    }

    public void _on_lock_rotation_timer_timeout()
    {
    _locked_rotating = false;
    }

}