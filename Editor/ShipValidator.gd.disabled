class_name ShipValidator

static var layer := 0
static var fill_atlas := Vector2i(0, 1)
static var walls = ["wall", "door", "thruster", "connector"]
static var floors = ["floor"]

static func autofill_floor(tilemap : TileMapLayer):
	if tilemap.get_used_cells().size() == 0: return
	var edges = [_find_bottom_left_edge(tilemap), _find_top_right_edge(tilemap)]
	_bucket(tilemap, Vector4i(edges[0].x - 1, edges[1].x + 1, edges[1].y - 1, edges[0].y + 1))
	_invert_floor_tiles(tilemap, Vector4i(edges[0].x - 1, edges[1].x + 1, edges[1].y - 1, edges[0].y + 1))

static func check_validity(tilemap : TileMapLayer) -> bool:
	if tilemap.get_used_cells().size() == 0: return false
	var edges = [_find_bottom_left_edge(tilemap), _find_top_right_edge(tilemap)]
	return _validate(tilemap, Vector4i(edges[0].x, edges[1].x, edges[1].y, edges[0].y))

static func _find_bottom_left_edge(tilemap : TileMapLayer) -> Vector2i:
	var edge = tilemap.get_used_cells()[0]
	for cell in tilemap.get_used_cells():
		if cell.x < edge.x: edge.x = cell.x
		if cell.y > edge.y: edge.y = cell.y
	return edge

static func _find_top_right_edge(tilemap : TileMapLayer) -> Vector2i:
	var edge = tilemap.get_used_cells()[0]
	for cell in tilemap.get_used_cells():
		if cell.x > edge.x: edge.x = cell.x
		if cell.y < edge.y: edge.y = cell.y
	return edge

static func _bucket(tilemap : TileMapLayer, limits : Vector4i): # left, right, top, bottom
	var checked_points = []
	var points_to_check = [Vector2i(limits.x, limits.w)]
	while points_to_check.size() != 0:
		for point in points_to_check:
			if !_is_wall(tilemap, point):
				ShipEditor.sell_tile(tilemap, point, true, true)
				tilemap.set_cell(point, 0, fill_atlas)
				for cell in _get_surrounding_cells(point, limits):
					if !cell in checked_points: points_to_check.append(cell)
			points_to_check.erase(point)
			checked_points.append(point)

static func _is_wall(tilemap : TileMapLayer, coords : Vector2i) -> bool:
	return get_tile_type(tilemap, coords, layer) in walls


static func get_tile_type(tilemap : TileMapLayer, coords : Vector2i, _layer := 0) -> String:
	if tilemap == null: return ""
	var source_id = tilemap.get_cell_source_id(coords)
	if source_id == null || source_id == -1: return ""
	var atlas_coord =  tilemap.get_cell_atlas_coords(coords)
	if atlas_coord == null || atlas_coord == Vector2i(-1, -1): return ""
	var tile_data =  tilemap.tile_set.get_source(source_id).get_tile_data(atlas_coord, 0)
	if tile_data == null: return ""
	var custom_data = tile_data.get_custom_data("type")
	return custom_data

static func _get_surrounding_cells(coords : Vector2i, limits : Vector4i):
	var cells = []

	if coords.x - 1 >= limits.x: cells.append(coords - Vector2i(1, 0))
	if coords.x + 1 <= limits.y: cells.append(coords + Vector2i(1, 0))
	if coords.y - 1 >= limits.z: cells.append(coords - Vector2i(0, 1))
	if coords.y + 1 <= limits.w: cells.append(coords + Vector2i(0, 1))

	return cells

static func _invert_floor_tiles(tilemap : TileMapLayer, limits : Vector4i, atlas_coords_fill := Vector2i(0, 0)):
	for x in range(limits.x, limits.y + 1):
		for y in range(limits.z, limits.w + 1):
			if tilemap.get_cell_atlas_coords(Vector2i(x, y)) == Vector2i(-1, -1): tilemap.set_cell(Vector2i(x, y), 0, atlas_coords_fill)
			if tilemap.get_cell_atlas_coords(Vector2i(x, y)) == fill_atlas: tilemap.set_cell(Vector2i(x, y), 0, Vector2i(-1, -1))


static func _validate(tilemap : TileMapLayer, limits : Vector4i) -> bool:

	var checked_points = []
	var points_to_check = []
	var connected_tiles = []

	var _has_core = false

	for cell in tilemap.get_used_cells():
		if get_tile_type(tilemap, cell) == "core": _has_core = true 
		if get_tile_type(tilemap, cell) == "connector": 
			connected_tiles.append(cell)
			for point in _get_surrounding_cells(cell, limits):
				if !point in checked_points: points_to_check.append(point)
				
	if !_has_core: return false
	if connected_tiles.size() == 0: return false


	while points_to_check.size() != 0:
		for point in points_to_check:
			if _is_wall(tilemap, point) || get_tile_type(tilemap, point) in floors:
				connected_tiles.append(point)
				for cell in _get_surrounding_cells(point, limits):
					if !cell in checked_points: points_to_check.append(cell)
			points_to_check.erase(point)
			checked_points.append(point)

	for x in range(limits.x, limits.y + 1):
		for y in range(limits.z, limits.w + 1):
			var point = Vector2i(x, y)
			if _is_wall(tilemap, point) || get_tile_type(tilemap, point) in floors:
				if !point in connected_tiles: 
					return false
	return true
