using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class ShipSaveFile : Resource
{
    private dynamic N(string path) => GetNode(path);


    [Export] int id;
    [Export] string path;
    [Export] Vector2 position;
    [Export] Vector2 old_position;
    [Export] Vector2 velocity;
    [Export] float rotation;

    [Export] destroyed_walls := [];
    [Export] opened_doors := [];
    [Export] pickedup_items := [];


    public static void save()
    {
    Array files = new Array();

    foreach (var ship in Ship.ships)
    {
        }
    dynamic file = new ShipSaveFile();
    file.id = ship.id;

    file.path = ship.path if ship.path != "%player_ship_new" else "%player_ship_old";
    file.Position = World.instance.get_distance_from_center(ship.GlobalPosition);
    file.old_position = ship._old_position;
    file.velocity = ship.linear_velocity;
    file.rotation = ship.rotation;

    file.destroyed_walls = ship.destroyed_walls;
    file.opened_doors = ship.opened_doors;
    file.pickedup_items = ship.pickedup_items;

    files.append(file);

    return files

    }

    public void load(Array _npcs = new Array(), Array _items = new Array())
    {

    Array _item_presets = new Array();
    Array _npc_presets = new Array();

    for npc in _npcs: if id == npc.ship_id: _npc_presets.append(NPCPreset.new(npc.id, npc.nickname, npc.roles, npc.skin, npc.hair));
    for item in _items: if id == item.ship_id: _item_presets.append(ItemPreset.new(item.id, item.type, item.ship_slot_id));

    dynamic custom_object_spawn = CustomObjectSpawn.create(_npc_presets, _item_presets);

    dynamic ship = ShipManager.spawn_ship(position, path, custom_object_spawn, true);
    ship.id = id;
    ship.rotation = rotation;
    ship.linear_velocity = velocity;
    ship.CallDeferred("apply_changes", destroyed_walls, opened_doors);
    ship._old_position = old_position;

    // spawn_ship(_position : Vector2, path : String = "station", custom_object_spawn : CustomObjectSpawn = null) -> void:
    }

}