using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class ObjectTileMap : TileMapLayer
{
    private dynamic N(string path) => GetNode(path);



    public static readonly PackedScene builder_scene = GD.Load<PackedScene>("res://Ship/Objects/Builder/Builder.tscn");
    public static readonly PackedScene helm_scene = GD.Load<PackedScene>("res://Ship/Objects/Helm/Helm.tscn");
    public static readonly PackedScene NPC_scene = GD.Load<PackedScene>("res://Character/NPC/NPC.tscn");

    public Ship ship = null;



    public bool load_ship(dynamic _ship, string path, CustomObjectSpawn custom_object_spawn)
    {
    ship = _ship;

    clear();

    FileAccess save_file;

    if (not FileAccess.file_exists("user://saves/ships/" + path + "/objects.dat"))
    {
        }
    if (not FileAccess.file_exists("res://DefaultSave/ships/" + path + "/objects.dat"))
    {
        }
    return false
    }
    else
    {
        }
    save_file = FileAccess.open("res://DefaultSave/ships/" + path + "/objects.dat", FileAccess.READ);

    }
    else
    {
        }
    save_file = FileAccess.open("user://saves/ships/" + path + "/objects.dat", FileAccess.READ);

    contents := [];

    while (save_file.get_position() != save_file.get_length())
    {
        }
    contents = [save_file.get_float(), save_file.get_float(), save_file.get_16(), save_file.get_float(), save_file.get_float(), save_file.get_16()];
    tile:= Vector2();
    tile.x = contents[0];
    tile.y = contents[1];
    set_cell(tile, contents[2], Vector2I(contents[3], contents[4]), contents[5]);

    save_file.close();

    // ship.original_object_tile_map = self

    return _replace_interactive_tiles(custom_object_spawn, _from_save)


    }

    public bool _replace_interactive_tiles(CustomObjectSpawn custom_object_spawn, bool _from_save)
    {

    int npc_index = 0;
    int item_index = 0;

    int item_slot = 0;

    foreach (var cellpos in get_used_cells())
    {
        }
    dynamic cell = get_cell_tile_data(cellpos);

    dynamic tile_position = map_to_local(cellpos) * Limits.TILE_SCALE;

    switch (cell.get_custom_data("type"))
    {
        }
    case "helm":
        dynamic helm_object = helm_scene.Instantiate();
        helm_object.init(ship, cellpos);
        helm_object.Position = tile_position;

        ship.object_tiles.add_child(helm_object);


        }
    case "builder":
        dynamic builder_object = builder_scene.Instantiate();
        builder_object.init(ship, cellpos);
        builder_object.Position = tile_position;

        ship.object_tiles.add_child(builder_object);


        }
    case "npc":
        dynamic NPC_object = NPC_scene.Instantiate();
        NPC_object.spawn_point = tile_position;
        NPC_object.spawn();

        if (custom_object_spawn != null && custom_object_spawn.npc_presets.Count && npc_index < custom_object_spawn.npc_presets.Count)
        {
            }
        NPC_object.init(custom_object_spawn.npc_presets[npc_index].id, custom_object_spawn.npc_presets[npc_index].nickname, custom_object_spawn.npc_presets[npc_index].roles, custom_object_spawn.npc_presets[npc_index].colors, custom_object_spawn.npc_presets[npc_index].hair);
        npc_index += 1;
        }
        else if (!_from_save)
        {
            }
        NPC_object.init();

        NPC_object.ship = ship;
        ship.passengers_node.add_child(NPC_object);
        ship.passengers.append(NPC_object);


        }
    case "item":

        random := new RandomNumberGenerator();
        dynamic scaling = 4 * Limits.TILE_SCALE;
        dynamic offset = Vector2(scaling - random.randf() * scaling * 2, scaling - random.randf() * scaling * 2);
        if (custom_object_spawn != null && custom_object_spawn.item_presets.Count && item_index < custom_object_spawn.item_presets.Count)
        {
            }
        dynamic _in = item_index;
        foreach (var i in custom_object_spawn.item_presets.Count)
        {
            }
        if (custom_object_spawn.item_presets[i].ship_slot_id == item_slot)
        {
            }
        dynamic _new_item = Item.spawn(custom_object_spawn.item_presets[item_index].type, to_global(tile_position) + offset, custom_object_spawn.item_presets[item_index].id, ship, item_slot);
        item_index += 1;
        if (_in == item_index)
        {
            }
        foreach (var i in custom_object_spawn.item_presets.Count)
        {
            }
        if (custom_object_spawn.item_presets[i].ship_slot_id < 0)
        {
            dynamic _new_item = Item.spawn(custom_object_spawn.item_presets[item_index].type, to_global(tile_position) + offset, custom_object_spawn.item_presets[item_index].id, ship, item_slot);
            item_index += 1;
            }
        else if (!_from_save)
        {
            }
        Item.spawn(Item.random_item(), to_global(tile_position) + offset, -1, ship, item_slot);

        item_slot += 1;

        }
    return true
    }

}