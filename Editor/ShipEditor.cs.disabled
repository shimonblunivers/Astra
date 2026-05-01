using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class ShipEditor : Node2D
{
    private dynamic N(string path) => GetNode(path);



    public Console console;
    // onready: console = GetNode<Console>("../HUD/Console/ConsoleLog");

    public static dynamic instance;
    public static dynamic tool_preview;
    public Inventory inventory;

    public static Dictionary directions = {;
    // TODO: 0 : "doprava",
    // TODO: 1 : "dolů",
    // TODO: 2 : "doleva",
    // TODO: 3 : "nahoru",
    // TODO: }

    public static direction := 0;

    public static bool autoflooring = false;

    public static tools := {};

    public static Tool tool = null;

    public static int current_ship_price = 0;

    public static dynamic starting_block_coords = Vector2.Zero;

    // TODO: ✅ Make placing tiles smoother

    // TODO: ✅ Create menu for tools

    // TODO: ✅ Make money (energy) system

    // TODO: ✅ Add autoflooring

    // TODO: Add Interactables


    public void evide_tiles()
    {
    starting_block_coords = Vector2.Zero;
    foreach (var key in tools.keys())
    {
        }
    tools[key].number_of_instances = 0;

    current_ship_price = 0;

    foreach (var coords in wall_tile_map.get_used_cells())
    {
        }
    dynamic type = ShipValidator.get_tile_type(wall_tile_map, coords);
    if type == "connector": starting_block_coords = Vector2(coords) * 32 + Vector2(16, 16) + global_position;
    if (type in tools.keys())
    {
        }
    tools[type].number_of_instances += 1;
    current_ship_price += tools[type].price;
    foreach (var coords in object_tile_map.get_used_cells())
    {
        }
    dynamic type = ShipValidator.get_tile_type(object_tile_map, coords);
    if (type in tools.keys())
    {
        }
    tools[type].number_of_instances += 1;
    current_ship_price += tools[type].price;

    }

    public override void _Ready()
    {
    load_tools();

    instance = this;
    tool_preview = N("../HUD/ToolPreview");
    ShipEditor.change_tool("wall");

    // ShipValidator.autofill_floor(wall_tile_map)

    update_preview.CallDeferred();

    }

    public void update_preview()
    {
    Editor.instance.direction_label.Text = "Směr: " + directions[direction];
    ShipEditor.update_preview_rotation();


    }

    public void load_tools()
    {
    string path = "res://Editor/Tools";
    dynamic dir = DirAccess.open(path);
    if (dir)
    {
        }
    dir.ListDirBegin();
    dynamic file_name = dir.GetNext();
    while (file_name != "")
    {
        }
    if (!dir.CurrentIsDir())
    {
        if ('.tres.remap' in file_name)
        {
            }
        file_name = file_name.TrimSuffix('.remap');
        if (".tres" in file_name)
        {
            }
        load(path + "/" + file_name).create();
        }
    file_name = dir.GetNext();

    }

    public static Vector2I get_mouse_tile(TileMapLayer tilemap)
    {
    return tilemap.local_to_map(instance.to_local(instance.get_global_mouse_position()))

    }

    public override void _UnhandledInput(InputEvent @event)
    {
    if (event is InputEventMouseMotion || event is InputEventMouseButton)
    {
        }
    if (event.ButtonMask == 1)
    {
        }
    use_tool();
    }
    else if (event.ButtonMask == 2 && (!ShipValidator.get_tile_type(wall_tile_map, ShipEditor.get_mouse_tile(wall_tile_map)) == "connector" || Options.DEVELOPMENT_MODE))
    {
        }
    if (ShipValidator.get_tile_type(object_tile_map, ShipEditor.get_mouse_tile(object_tile_map)) in tools && tools[ShipValidator.get_tile_type(object_tile_map, ShipEditor.get_mouse_tile(object_tile_map))].object)
    {
        ShipEditor.sell_tile(object_tile_map, ShipEditor.get_mouse_tile(object_tile_map));
        }
    else
    {
        ShipEditor.sell_tile(wall_tile_map, ShipEditor.get_mouse_tile(wall_tile_map));


        }
    if (event.is_action_pressed("editor_change_direction"))
    {
        }
    ShipEditor.direction = (ShipEditor.direction + 1) % 4;
    update_preview();
    }

    public void use_tool()
    {
    if tool == null: return;
    if !Options.DEVELOPMENT_MODE && !(tool.number_of_instances < tool.world_limit || tool.world_limit < 0): return;
    TileMapLayer tilemap = object_tile_map if tool.object else wall_tile_map;
    Vector2I tile = ShipEditor.get_mouse_tile(tilemap);
    if ShipValidator.get_tile_type(tilemap, tile) == tool.name: return;
    bool placing_on_something = false;
    if (tool.placeable_on_atlas_choords != Vector2I(-1, -1))
    {
        }
    placing_on_something = true;
    if (tool.placeable_on_atlas_choords != wall_tile_map.get_cell_atlas_coords(ShipEditor.get_mouse_tile(wall_tile_map)))
    {
        }
    return

    if ShipValidator.get_tile_type(tilemap, tile) == "connector": return;
    ShipEditor.sell_tile(tilemap, tile, false);

    if (tool.price != 0 && !Inventory.add_currency(-tool.price) && !Options.DEVELOPMENT_MODE)
    {
        }
    return

    tool.number_of_instances += 1;

    if (!placing_on_something)
    {
        }
    tilemap.set_cells_terrain_connect([tile], 0, -1, false);

    if (tool.terrain_id != -1)
    {
        }
    tilemap.set_cells_terrain_connect([tile], 0, tool.terrain_id);
    }
    else if (tool.atlas_coords != Vector2I(-1, -1))
    {
        }
    tilemap.set_cell(tile, 0, tool.atlas_coords, direction if tool.rotatable else 0);

    if (tool.name in ShipValidator.walls && autoflooring)
    {
        }
    ShipValidator.autofill_floor(tilemap);

    }

    public static bool sell_tile(TileMapLayer tilemap, Vector2I coords)
    {
    bool sold = false;
    dynamic type = ShipValidator.get_tile_type(tilemap, coords);

    if (type in tools.keys())
    {
        }
    tools[type].number_of_instances -= 1;
    Inventory.add_currency(tools[type].price, delete_tile);
    sold = true;

    if delete_tile: tilemap.set_cells_terrain_connect([coords], 0, -1, false);

    if (autoflooring && !react_autofill)
    {
        }
    ShipValidator.autofill_floor(tilemap);

    return sold

    }

    public static void change_tool(string key)
    {
    tool = tools[key];
    tool_preview.Texture = tool.Texture;
    update_preview_rotation();

    }

    public static void update_preview_rotation()
    {
    if (tool.rotatable)
    {
        }
    tool_preview.RotationDegrees = 90 * direction;
    }
    else
    {
        }
    tool_preview.RotationDegrees = 0;

    }

    public void save_ship(string path = "_default_ship")
    {
    evide_tiles();

    string location;

    if path.BeginsWith("_"): location = "res://DefaultSave/ships/";
    }
    else: location = "user://saves/ships/";

    DirAccess.make_dir_recursive_absolute(location + path + "/");

    walls_save_file := FileAccess.open(location + path + "/walls.dat", FileAccess.WRITE);
    objects_save_file := FileAccess.open(location + path + "/objects.dat", FileAccess.WRITE);
    details_save_file := FileAccess.open(location + path + "/details.dat", FileAccess.WRITE);

    foreach (var cell in wall_tile_map.get_used_cells())
    {
        }
    walls_save_file.store_float(cell.x)	# 0;
    walls_save_file.store_float(cell.y)	# 1;
    walls_save_file.store_16(wall_tile_map.get_cell_source_id(Vector2I(cell.x, cell.y)))	# 2;
    walls_save_file.store_float(wall_tile_map.get_cell_atlas_coords(Vector2I(cell.x, cell.y)).x)	# 3;
    walls_save_file.store_float(wall_tile_map.get_cell_atlas_coords(Vector2I(cell.x, cell.y)).y)	# 4;
    walls_save_file.store_16(wall_tile_map.get_cell_alternative_tile(Vector2I(cell.x, cell.y)))	# 5;

    foreach (var cell in object_tile_map.get_used_cells())
    {
        }
    objects_save_file.store_float(cell.x)	# 0;
    objects_save_file.store_float(cell.y)	# 1;
    objects_save_file.store_16(object_tile_map.get_cell_source_id(Vector2I(cell.x, cell.y)))	# 2;
    objects_save_file.store_float(object_tile_map.get_cell_atlas_coords(Vector2I(cell.x, cell.y)).x)	# 3;
    objects_save_file.store_float(object_tile_map.get_cell_atlas_coords(Vector2I(cell.x, cell.y)).y)	# 4;
    objects_save_file.store_16(object_tile_map.get_cell_alternative_tile(Vector2I(cell.x, cell.y)))	# 5;

    details_save_file.store_16(current_ship_price);

    walls_save_file.close();
    objects_save_file.close();
    details_save_file.close();

    console.print_out("Uložena loď s názvem: " + path);

    }

    public bool load_ship(string path = "_default_ship")
    {

    string location;

    if path.BeginsWith("_"): location = "res://DefaultSave/ships/";
    }
    else: location = "user://saves/ships/";

    if (not FileAccess.file_exists(location + path + "/walls.dat"))
    {
        }
    return false
    if (not FileAccess.file_exists(location + path + "/objects.dat"))
    {
        }
    return false

    Editor.instance.inventory.currency += current_ship_price;

    // if !path.begins_with('_') && FileAccess.file_exists(location + path + "/details.dat"):
    // var details = FileAccess.open(location + path + "/details.dat", FileAccess.READ)
    // var price = details.get_16()



    wall_tile_map.clear();
    object_tile_map.clear();

    walls_save_file := FileAccess.open(location + path + "/walls.dat", FileAccess.READ);
    objects_save_file := FileAccess.open(location + path + "/objects.dat", FileAccess.READ);

    contents := [];

    while (walls_save_file.get_position() != walls_save_file.get_length())
    {
        }
    contents = [walls_save_file.get_float(), walls_save_file.get_float(), walls_save_file.get_16(), walls_save_file.get_float(), walls_save_file.get_float(), walls_save_file.get_16()];
    tile:= Vector2();
    tile.x = contents[0];
    tile.y = contents[1];
    wall_tile_map.set_cell(tile, contents[2], Vector2I(contents[3], contents[4]), contents[5]);

    while (objects_save_file.get_position() != objects_save_file.get_length())
    {
        }
    contents = [objects_save_file.get_float(), objects_save_file.get_float(), objects_save_file.get_16(), objects_save_file.get_float(), objects_save_file.get_float(), objects_save_file.get_16()];
    tile:= Vector2();
    tile.x = contents[0];
    tile.y = contents[1];
    object_tile_map.set_cell(tile, contents[2], Vector2I(contents[3], contents[4]), contents[5]);

    walls_save_file.close();
    objects_save_file.close();

    evide_tiles();

    if (charge)
    {
        }
    inventory.currency -= current_ship_price;
    inventory.currency_value.Text = str(inventory.currency);

    console.print_out("Načtena loď s názvem: " + path);
    return true
    }

}