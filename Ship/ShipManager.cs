using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class ShipManager : Node2D
{
    private dynamic N(string path) => GetNode(path);



    public static dynamic ship_scene = GD.Load<PackedScene>("res://Ship/Ship.tscn");

    public static ShipManager instance;
    public static int number_of_ships = 0;
    public static Ship : main_station;
    // TODO: set(value):
    // TODO: if value != null: value.freeze = true
    // TODO: main_station = value


    public override void _Ready()
    {
    instance = this;

    }

    public static void randomly_generate_ships()
    {
    Player.main_player.owned_ship = ShipManager.spawn_ship(Vector2(-10000, -10000), "_start_ship");
    Player.main_player.owned_ship.linear_damp = 0;

    GD.Print("Spawning main station..");
    main_station = ShipManager.spawn_ship(Vector2(0, 0), "_station", null, false, true);

    // var ship_percentage = 25

    // var random = RandomNumberGenerator.new()

    // var ship_counter = 4

    // for x in range(-5, 6):
    // for y in range(-5, 6):
    // if x == 0 && y == 0: continue
    // if random.randi_range(0, ship_percentage) == 0:
    // if ship_counter <= 0: break
    // ship_counter -= 1
    // ShipManager.spawn_ship(Vector2(x * 200000, y * 200000), random_ship())


    }

    public static string random_ship()
    {
    random := new RandomNumberGenerator();
    return "_small_shuttle_" + str(random.randi_range(0, 4))

    }

    public static string get_quest_ship_path(int _mission_id)
    {
    return random_ship()

    }

    public static Ship spawn_ship(Vector2 _position, string path = "_station", CustomObjectSpawn custom_object_spawn = null)
    {
    dynamic _ship = ship_scene.Instantiate();
    _ship.name = "Ship-" + str(_ship.id);
    instance.add_child(_ship);

    GD.Print("Spawning ship at " + str(_position));
    _ship.load_ship(_position, path, custom_object_spawn, lock_rotation, _from_save);
    return _ship

    }

    public static Ship build_ship(Builder _builder, bool for_player, string path = "_station")
    {
    Ship _ship = ship_scene.Instantiate();
    _ship.name = "Ship-" + str(_ship.id);
    _builder.play_sound();
    instance.add_child(_ship);
    _ship.load_ship(_builder.get_spawn_position(), path, null, true, true);
    _ship.linear_velocity = _builder.ship.linear_velocity;
    _ship.rotation = _builder.get_ship_rotation();


    if (for_player)
    {
        }
    if (Player.main_player.owned_ship != null)
    {
        }
    Player.main_player.deleting_ship(Player.main_player.owned_ship);
    Player.main_player.owned_ship.delete();
    Player.main_player.owned_ship = _ship;
    _ship.connectors[0].connect_to(_builder.connector);

    return _ship
    }

}