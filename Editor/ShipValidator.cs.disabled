using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class ShipValidator : Node
{
    private dynamic N(string path) => GetNode(path);

    // TODO: class_name ShipValidator

    public static layer := 0;
    public static fill_atlas := Vector2I(0, 1);
    public static Array walls = new Array {"wall", "door", "thruster", "connector"};
    public static Array floors = new Array {"floor"};


    public static void autofill_floor(TileMapLayer tilemap)
    {
    if tilemap.get_used_cells().Count == 0: return;
    Array edges = new Array {_find_bottom_left_edge(tilemap), _find_top_right_edge(tilemap)};
    _bucket(tilemap, Vector4i(edges[0].x - 1, edges[1].x + 1, edges[1].y - 1, edges[0].y + 1));
    _invert_floor_tiles(tilemap, Vector4i(edges[0].x - 1, edges[1].x + 1, edges[1].y - 1, edges[0].y + 1));

    }

    public static bool check_validity(TileMapLayer tilemap)
    {
    if tilemap.get_used_cells().Count == 0: return false;
    Array edges = new Array {_find_bottom_left_edge(tilemap), _find_top_right_edge(tilemap)};
    return _validate(tilemap, Vector4i(edges[0].x, edges[1].x, edges[1].y, edges[0].y))

    }

    public static Vector2I _find_bottom_left_edge(TileMapLayer tilemap)
    {
    dynamic edge = tilemap.get_used_cells()[0];
    foreach (var cell in tilemap.get_used_cells())
    {
        }
    if cell.x < edge.x: edge.x = cell.x;
    if cell.y > edge.y: edge.y = cell.y;
    return edge

    }

    public static Vector2I _find_top_right_edge(TileMapLayer tilemap)
    {
    dynamic edge = tilemap.get_used_cells()[0];
    foreach (var cell in tilemap.get_used_cells())
    {
        }
    if cell.x > edge.x: edge.x = cell.x;
    if cell.y < edge.y: edge.y = cell.y;
    return edge

    }

    public static void _bucket(TileMapLayer tilemap, Vector4i limits)
    {
    Array checked_points = new Array();
    Array points_to_check = new Array {Vector2I(limits.x, limits.w)};
    while (points_to_check.Count != 0)
    {
        }
    foreach (var point in points_to_check)
    {
        }
    if (!_is_wall(tilemap, point))
    {
        ShipEditor.sell_tile(tilemap, point, true, true);
        tilemap.set_cell(point, 0, fill_atlas);
        foreach (var cell in _get_surrounding_cells(point, limits))
        {
            }
        if !cell in checked_points: points_to_check.append(cell);
        }
    points_to_check.erase(point);
    checked_points.append(point);

    }

    public static bool _is_wall(TileMapLayer tilemap, Vector2I coords)
    {
    return get_tile_type(tilemap, coords, layer) in walls


    }

    public static string get_tile_type(TileMapLayer tilemap, Vector2I coords)
    {
    if tilemap == null: return "";
    dynamic source_id = tilemap.get_cell_source_id(coords);
    if source_id == null || source_id == -1: return "";
    dynamic atlas_coord = tilemap.get_cell_atlas_coords(coords);
    if atlas_coord == null || atlas_coord == Vector2I(-1, -1): return "";
    dynamic tile_data = tilemap.tile_set.get_source(source_id).get_tile_data(atlas_coord, 0);
    if tile_data == null: return "";
    dynamic custom_data = tile_data.get_custom_data("type");
    return custom_data

    }

    public static void _get_surrounding_cells(Vector2I coords, Vector4i limits)
    {
    Array cells = new Array();

    if coords.x - 1 >= limits.x: cells.append(coords - Vector2I(1, 0));
    if coords.x + 1 <= limits.y: cells.append(coords + Vector2I(1, 0));
    if coords.y - 1 >= limits.z: cells.append(coords - Vector2I(0, 1));
    if coords.y + 1 <= limits.w: cells.append(coords + Vector2I(0, 1));

    return cells

    }

    public static void _invert_floor_tiles(TileMapLayer tilemap, Vector4i limits)
    {
    for (int x = limits.x; x < limits.y + 1; x++)
    {
        }
    for (int y = limits.z; y < limits.w + 1; y++)
    {
        }
    if tilemap.get_cell_atlas_coords(Vector2I(x, y)) == Vector2I(-1, -1): tilemap.set_cell(Vector2I(x, y), 0, atlas_coords_fill);
    if tilemap.get_cell_atlas_coords(Vector2I(x, y)) == fill_atlas: tilemap.set_cell(Vector2I(x, y), 0, Vector2I(-1, -1));


    }

    public static bool _validate(TileMapLayer tilemap, Vector4i limits)
    {

    Array checked_points = new Array();
    Array points_to_check = new Array();
    Array connected_tiles = new Array();

    bool _has_core = false;

    foreach (var cell in tilemap.get_used_cells())
    {
        }
    if get_tile_type(tilemap, cell) == "core": _has_core = true;
    if (get_tile_type(tilemap, cell) == "connector")
    {
        }
    connected_tiles.append(cell);
    foreach (var point in _get_surrounding_cells(cell, limits))
    {
        if !point in checked_points: points_to_check.append(point);

        }
    if !_has_core: return false;
    if connected_tiles.Count == 0: return false;


    while (points_to_check.Count != 0)
    {
        }
    foreach (var point in points_to_check)
    {
        }
    if (_is_wall(tilemap, point) || get_tile_type(tilemap, point) in floors)
    {
        connected_tiles.append(point);
        foreach (var cell in _get_surrounding_cells(point, limits))
        {
            }
        if !cell in checked_points: points_to_check.append(cell);
        }
    points_to_check.erase(point);
    checked_points.append(point);

    for (int x = limits.x; x < limits.y + 1; x++)
    {
        }
    for (int y = limits.z; y < limits.w + 1; y++)
    {
        }
    dynamic point = Vector2I(x, y);
    if (_is_wall(tilemap, point) || get_tile_type(tilemap, point) in floors)
    {
        if (!point in connected_tiles)
        {
            }
        return false
        }
    return true
    }

}