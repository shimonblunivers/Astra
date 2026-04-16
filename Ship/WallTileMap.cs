using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class WallTileMap : TileMapLayer
{
    private dynamic N(string path) => GetNode(path);



    public static readonly PackedScene door_scene = GD.Load<PackedScene>("res://Ship/Walls/Door/Door.tscn");
    public static readonly PackedScene wall_scene = GD.Load<PackedScene>("res://Ship/Walls/Wall/Wall.tscn");
    public static readonly PackedScene floor_scene = GD.Load<PackedScene>("res://Ship/Walls/Floor/Floor.tscn");
    public static readonly PackedScene core_scene = GD.Load<PackedScene>("res://Ship/Walls/Core/Core.tscn");
    public static readonly PackedScene thruster_scene = GD.Load<PackedScene>("res://Ship/Walls/Thruster/Thruster.tscn");
    public static readonly PackedScene connector_scene = GD.Load<PackedScene>("res://Ship/Walls/Connector/Connector.tscn");

    public Ship ship = null;

    public Rect2I _first_rect;

    public dynamic test_polygon = null;


    public void get_rect()
    {
    return _first_rect

    }

    public void _load_hitbox()
    {
    PackedVector2Array shape = _to_shape(_delete_edges(_create_edges()));

    ship.Polygon = shape;
    ship.hitbox.Polygon = shape;
    ship.visual.Polygon = shape;
    ship.area.Polygon = shape;


    }

    public void _get_points(Vector2I tile)
    {
    // 1   2
    // 
    // 0   3

    dynamic tile_size = tile_set.tile_size;

    return [
    Vector2(tile.x * tile_size.x, tile.y * tile_size.y + tile_size.y), # 0;
    Vector2(tile.x * tile_size.x, tile.y * tile_size.y), # 1;
    Vector2(tile.x * tile_size.x + tile_size.x, tile.y * tile_size.y), # 2;
    Vector2(tile.x * tile_size.x + tile_size.x, tile.y * tile_size.y + tile_size.y) # 3;
    ];

    }

    public void _get_points_rect(Rect2 rect)
    {

    // 1   2
    // 
    // 0   3

    dynamic rect_position = rect.Position * Vector2(tile_set.tile_size);
    dynamic rect_size = rect.size * Vector2(tile_set.tile_size);
    return [
    Vector2(rect_position.x, rect_position.y + rect_size.y), # 0;
    Vector2(rect_position.x, rect_position.y), # 1;
    Vector2(rect_position.x + rect_size.x, rect_position.y), # 2;
    Vector2(rect_position.x + rect_size.x, rect_position.y + rect_size.y) # 3;
    ];

    }

    public void _get_lines(dynamic points, dynamic _scale = Limits.TILE_SCALE)
    {
    return [
    [points[0] * _scale, points[1] * _scale],;
    [points[1] * _scale, points[2] * _scale],;
    [points[2] * _scale, points[3] * _scale],;
    new Array {points[3} * _scale, pointsnew Array {0} * _scale];
    ];

    }

    public void _create_edges()
    {
    Array edges = new Array();
    dynamic grid = get_used_cells();
    foreach (var tile in grid)
    {
        }
    foreach (var line in _get_lines(_get_points(tile)))
    {
        }
    edges.append(line);
    return edges

    }

    public void _delete_edges(dynamic edges)
    {
    Dictionary seen_edges = new Dictionary();
    Array marked_for_deletion = new Array();

    foreach (var current_line in edges)
    {
        }
    Array current_line_inverted = new Array {current_line[1}, current_linenew Array {0}];

    if (seen_edges.has(current_line) or seen_edges.has(current_line_inverted))
    {
        }
    marked_for_deletion.append(current_line);
    marked_for_deletion.append(current_line_inverted);
    }
    else
    {
        }
    seen_edges[current_line] = true;
    seen_edges[current_line_inverted] = true;

    foreach (var line in marked_for_deletion)
    {
        }
    edges.erase(line);

    return edges

    }

    public void _to_shape(dynamic edges)
    {
    dynamic result = PackedVector2Array();
    dynamic next_line = edges[0];
    for (int index = 0; index < edges.Count; index++)
    {
        }
    foreach (var other_line in edges)
    {
        }
    if other_line == next_line: continue;
    if (next_line[1] == other_line[0])
    {
        next_line = other_line;
        break;
        }
    else if (next_line[1] == other_line[1])
    {
        next_line = [other_line[1], other_line[0]];

        }
    result.append(next_line[0]);
    return result

    }

    public void update_center_of_mass()
    {
    dynamic grid = get_used_cells();
    float leftmost_point = grid[0].x;
    float rightmost_point = grid[0].x;
    float highest_point = grid[0].y;
    float lowest_point = grid[0].y;

    foreach (var point in grid)
    {
        }
    if point.x < leftmost_point: leftmost_point = point.x;
    if point.x > rightmost_point: rightmost_point = point.x;
    if point.y > lowest_point: lowest_point = point.y;
    if point.y < highest_point: highest_point = point.y;

    ship.center_of_mass = Vector2(5 * tile_set.tile_size.x * (0.5 + (leftmost_point + rightmost_point) / 2), 5 * tile_set.tile_size.y * (0.5 + (highest_point + lowest_point) / 2));

    }

    public bool load_ship(dynamic _ship, string path)
    {
    ship = _ship;
    clear();

    FileAccess save_file;

    GD.Print("Loading ship file..");

    if (not FileAccess.file_exists("user://saves/ships/" + path + "/walls.dat"))
    {
        }
    if (not FileAccess.file_exists("res://DefaultSave/ships/" + path + "/walls.dat"))
    {
        }
    return false
    }
    else
    {
        }
    save_file = FileAccess.open("res://DefaultSave/ships/" + path + "/walls.dat", FileAccess.READ);
    }
    else
    {
        }
    save_file = FileAccess.open("user://saves/ships/" + path + "/walls.dat", FileAccess.READ);

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

    GD.Print("Loading ship hitbox..");

    _load_hitbox();

    GD.Print("Updating ship center of mass..");

    update_center_of_mass();


    _first_rect = get_used_rect();

    // ship.original_wall_tile_map = self

    GD.Print("Replacing tiles..");
    _replace_tiles();

    return true

    }

    public bool _replace_tiles()
    {
    layer := 0;

    atlas := tile_set.get_source(0) as TileSetAtlasSource;
    atlas_image := atlas.Texture.get_image();

    foreach (var cellpos in get_used_cells())
    {
        }
    cell := get_cell_tile_data(cellpos);

    dynamic object_direction = get_cell_alternative_tile(cellpos);

    dynamic tile_position = map_to_local(cellpos) * Limits.TILE_SCALE;

    if (cell == null)
    {
        dynamic _connector_object = connector_scene.Instantiate();
        _connector_object.init(ship, cellpos);
        _connector_object.Position = tile_position;

        ship.wall_tiles.add_child(_connector_object);

        _connector_object.RotationDegrees = object_direction * 90;

        set_cell(cellpos, -1);
        continue;

        }
    switch (cell.get_custom_data("type"))
    {

        }
    case "floor":
        dynamic _floor_object = floor_scene.Instantiate();
        _floor_object.init(ship, cellpos);
        _floor_object.Position = tile_position;
        ship.wall_tiles.add_child(_floor_object);

        }
    case "door":

        dynamic _door_object = door_scene.Instantiate();
        _door_object.init(ship, cellpos);
        _door_object.direction = cell.get_custom_data("direction");
        _door_object.Position = tile_position;
        ship.wall_tiles.add_child(_door_object);
        dynamic _floor_object = floor_scene.Instantiate();
        _floor_object.init(ship, cellpos);
        _floor_object.Position = tile_position;
        ship.wall_tiles.add_child(_floor_object);


        }
    case "wall":

        dynamic _wall_object = wall_scene.Instantiate();
        _wall_object.init(ship, cellpos);
        _wall_object.Position = tile_position;
        ship.wall_tiles.add_child(_wall_object);

        _wall_object.light_occluder.occluder = cell.get_occluder(layer);
        _wall_object.light_occluder.Scale = Vector2(1, 1) * Limits.TILE_SCALE;

        tile_image := atlas_image.get_region(atlas.get_tile_texture_region(get_cell_atlas_coords(cellpos)));

        for i in range(object_direction): tile_image.rotate_90(CLOCKWISE);

        tile_texture := ImageTexture.create_from_image(tile_image);

        tile_texture.set_size_override(Vector2I(32, 32));
        _wall_object.set_texture(tile_texture);

        }
    case "core":

        dynamic _core_object = core_scene.Instantiate();
        _core_object.init(ship, cellpos);
        _core_object.Position = tile_position;
        ship.wall_tiles.add_child(_core_object);
        dynamic _floor_object = floor_scene.Instantiate();
        _floor_object.init(ship, cellpos);
        _floor_object.Position = tile_position;
        ship.wall_tiles.add_child(_floor_object);


        }
    case "thruster":
        dynamic _thruster_object = thruster_scene.Instantiate();
        _thruster_object.init(ship, cellpos, 150, 5, object_direction);
        _thruster_object.Position = tile_position;

        ship.wall_tiles.add_child(_thruster_object);

        _thruster_object.RotationDegrees = object_direction * 90;


        }
    case "connector":
        dynamic _connector_object = connector_scene.Instantiate();
        _connector_object.init(ship, cellpos);
        _connector_object.Position = tile_position;

        ship.wall_tiles.add_child(_connector_object);

        _connector_object.RotationDegrees = object_direction * 90;

        }
    return true









    }

}