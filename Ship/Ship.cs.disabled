using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class Ship : RigidBody2D
{
    private dynamic N(string path) => GetNode(path);






    // var original_wall_tile_map : TileMap
    // var original_object_tile_map : TileMap

    public static Array ships = new Array();

    public int id;

    public dynamic polygon;

    public Vector2 dock_position = Vector2(100, 100);

    public passengers := [];

    public int used_item_slots = 0;

    public dynamic controlled_by = null;

    public acceleration := Vector2.Zero;

    public float rotation_speed = 0;

    public Vector4 thrust_power = Vector4(0, 0, 0, 0) # LEFT UP RIGHT DOWN;

    public thrusters := [[], [], [], []] # LEFT UP RIGHT DOWN;

    public interactables := [];

    public string path;

    public destroyed_walls := [];
    public opened_doors := [];
    public pickedup_items := [];

    public bool spawning = true;

    public float comfortable_rotation_degrees = 0;

    public Array connectors = new Array();

    public bool from_save = false;

    // TODO: ✅ Fix bugging when the player exits at high speed

    // TODO: ✅ Fix infinity position while moving too fast

    // TODO: ✅ Fix player moving into walls when encountering moving ship

    // TODO: ✅ Fix Ship center

    // TODO: ✅ Add Thrusters

    // TODO: ✅ Add Ship Connector

    // TODO: Make Area2Ds for each room

    // TODO: Make ships destroyable by collisions

    // TODO: Add Canons

    // TODO: Add Radar

    // TODO: Create planets/moons/asteroids

    public dynamic _old_position = position;
    public difference_in_position := Vector2.Zero;


    public static Ship get_ship(int _id)
    {
    foreach (var ship in ships)
    {
        }
    if ship.id == _id: return ship;
    return null

    }

    public void set_angle(int angle)
    {
    comfortable_rotation_degrees = angle;
    foreach (var npc in NPC.npcs)
    {
        }
    if (npc.ship == this)
    {
        }
    npc.RotationDegrees = angle;

    }

    public override void _Ready()
    {
    Ship.ships.append(this);

    }

    public void load_ship(Vector2 _position, string _path, CustomObjectSpawn custom_object_spawn, bool _lock_rotation = false)
    {
    global_position = _position;
    _old_position = _position;
    path = _path;
    name = path + "-" + str(id);

    GD.Print("Loading ship: " + name);

    mass = 1;

    from_save = _from_save;

    GD.Print("Loading walls..");
    wall_tile_map.load_ship(this, _path);

    GD.Print("Loading objects..");
    object_tile_map.load_ship(this, _path, custom_object_spawn, _from_save);
    mass -= 1;

    id = ShipManager.number_of_ships;
    ShipManager.number_of_ships += 1;

    for direction in thrusters: for thruster in direction: thruster.set_status(false);

    if (!_lock_rotation)
    {
        }
    dynamic rng = new RandomNumberGenerator();
    rotation = rng.GD.RandRange(0, 2 * PI);

    }

    public void get_tile(Vector2I coords)
    {
    foreach (var tile in wall_tiles.get_children())
    {
        }
    if ((tile is ShipPart))
    {
        }
    if ((coords == tile.tilemap_coords))
    {
        return tile
        }
    return null

    }

    public Vector2 get_closest_point(Vector2 point1)
    {
    if polygon == null: return global_position;
    dynamic closest = polygon[0].rotated(global_rotation) + global_position;
    foreach (var point2 in polygon)
    {
        }
    point2 = point2.rotated(global_rotation) + global_position;
    if (closest.distance_to(point1) > point2.distance_to(point1))
    {
        }
    closest = point2;
    return closest

    }

    public void set_connector(int connector_index, Connector connector)
    {
    connectors[connector_index].connect_to(connector);

    }

    public void _integrate_forces(PhysicsDirectBodyState2D state)
    {
    if (global_position - Player.main_player.GlobalPosition).length() > Player.main_player.update_range: return;
    {
        }
    acceleration = Vector2.Zero;
    rotation_speed = 0;

    if (controlled_by != null)
    {
        }
    control();

    update_thrusters();
    update_side_trusters();

    if (controlled_by != null)
    {
        }
    acceleration = acceleration.rotated(global_rotation);
    if acceleration != Vector2.Zero: state.apply_central_impulse(acceleration);
    if rotation_speed != 0: state.apply_torque_impulse(rotation_speed);

    // if abs(get_linear_velocity().x) > Limits.VELOCITY_MAX or abs(get_linear_velocity().y) > Limits.VELOCITY_MAX:
    // var new_speed = get_linear_velocity().normalized()
    // new_speed *= Limits.VELOCITY_MAX
    // set_linear_velocity(new_speed)


    }

    public override void _PhysicsProcess(double delta)
    {
    if (spawning && !from_save)
    {
        }
    foreach (var exception in get_collision_exceptions())
    {
        }
    remove_collision_exception_with(exception);
    dynamic collision = move_and_collide(Vector2.Zero);
    if collision == null: spawning = false;
    }
    else
    {
        }
    add_collision_exception_with(collision.get_collider());
    move_and_collide(Vector2(collision.get_depth() * cos(collision.get_angle()) * 10,  10 * collision.get_depth() * sin(collision.get_angle())));

    difference_in_position = position - _old_position;
    // print("Ship moved by: ", _difference_in_position)
    // for passenger in passengers:
    // if passenger.is_in_group("NPC"):
    // passenger.legs.position = passenger.legs_offset + difference_in_position
    // area.position = -difference_in_position
    _old_position = position;

    // if Player.main_player.parent_ship == self: hitbox.position = (-difference_in_position).rotated(-global_rotation) # Hitbox counter bug
    // else: hitbox.position = Vector2.ZERO

    }

    public void control()
    {

    direction := Input.get_vector("ui_left", "ui_right", "ui_up", "ui_down");
    rotation_direction := Input.get_axis("game_turn_left","game_turn_right");

    dynamic _rotation_power = 4000 * mass;

    if !controlled_by.alive: direction = Vector2.Zero;

    if direction.x < 0: acceleration.x -= thrust_power.x;
    elif direction.x > 0: acceleration.x += thrust_power.z;

    if direction.y < 0: acceleration.y -= thrust_power.y;
    elif direction.y > 0: acceleration.y += thrust_power.w;

    if ((thrusters[0].Count + thrusters[1].Count + thrusters[2].Count + thrusters[3].Count) == 0): rotation_speed = 0;
    {
        }
    else: rotation_speed = _rotation_power * rotation_direction + rotation_direction * (thrusters[0].Count + thrusters[1].Count + thrusters[2].Count + thrusters[3].Count) * _rotation_power / 2;

    }

    public void update_side_trusters()
    {
    foreach (var thruster_list in thrusters)
    {
        }
    foreach (var thruster in thruster_list)
    {
        }
    thruster.side_thrusters(rotation_speed);

    }

    public void apply_changes(Array _destroyed_walls = new Array(), Array _opened_doors = new Array())
    {

    foreach (var coords in _opened_doors)
    {
        }
    dynamic tile = get_tile(coords);
    if tile is Door: tile.open();

    foreach (var coords in _destroyed_walls)
    {
        }
    dynamic tile = get_tile(coords);
    if tile is Wall: tile.destroy();

    // for item_id in _pickedup_items:
    // var item = Item.get_item(item_id)
    // print(item.name)
    // if item != null: item.delete()

    }

    public void update_thrusters()
    {
    foreach (var thruster in thrusters[0]: if thruster.running != (acceleration.x < 0))
    {
        }
    thruster.set_status(acceleration.x < 0);
    foreach (var thruster in thrusters[2]: if thruster.running != (acceleration.x > 0))
    {
        }
    thruster.set_status(acceleration.x > 0);
    foreach (var thruster in thrusters[1]: if thruster.running != (acceleration.y < 0))
    {
        }
    thruster.set_status(acceleration.y < 0);
    foreach (var thruster in thrusters[3]: if thruster.running != (acceleration.y > 0))
    {
        }
    thruster.set_status(acceleration.y > 0);

    }

    public void get_rect()
    {
    return wall_tile_map.get_rect()

    }

    public void get_tile_size()
    {
    return Vector2(wall_tile_map.tile_set.tile_size) * wall_tile_map.Scale

    }

    public void start_controlling(dynamic player)
    {
    controlled_by = player;

    }

    public void stop_controlling()
    {
    controlled_by = null;

    }

    public void _on_area_area_entered(Area2D _area)
    {
    if (_area.is_in_group("PlayerInteractArea"))
    {
        }
    dynamic body = _area.get_parent();
    if (!body.spawned): 				return;
    {
        }
    if (body in passengers): 			return;
    {
        }
    passengers.append(body);
    body.get_in(this);

    // if body.is_in_group("Player"):
    // if body.max_impact_velocity < (body.acceleration - _difference_in_position).length(): body.kill()   TODO: OPRAVIT

    }

    public void _on_area_area_exited(Area2D _area)
    {
    if (_area.is_in_group("PlayerInteractArea"))
    {
        }
    dynamic body = _area.get_parent();
    if (!body.spawned): 				return;
    {
        }
    if !(body in passengers): 			return;
    passengers.erase(body);
    body.get_off(this);

    }

    public void delete()
    {
    hitbox.disabled = true;
    for connector in connectors: connector.connect_to(null);
    if this == Player.main_player.parent_ship: Player.main_player.reparent(World.instance);
    ShipManager.number_of_ships -= 1;
    Ship.ships.erase(this);
    queue_free();

    }

    public override void _UnhandledInput(InputEvent @event)
    {
    if controlled_by == null: return;
    if (event.is_action_pressed("game_dock_ship"))
    {
        }
    bool connected = false;
    foreach (var connector in connectors)
    {
        }
    if (connector.connected_to != null)
    {
        connector.connect_to(null);
        connected = true;
        }
    if (!connected && connectors[0].connectors_in_range.Count > 0)
    {
        }
    connectors[0].connect_to(connectors[0].connectors_in_range[0]);

    }

    public override void _Draw()
    {
    if !visual.Visible: return;
    if (polygon)
    {
        }
    foreach (var point in polygon)
    {
        }
    draw_circle(point.rotated(global_rotation) + global_position, 16, Color.RED);


    if (wall_tile_map.test_polygon)
    {
        }
    foreach (var point in wall_tile_map.test_polygon)
    {
        }
    draw_circle(Vector2(point) + global_position + Vector2(0, -200), 16, Color.BLUE);
    }

}