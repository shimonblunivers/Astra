extends TileMapLayer


const door_scene = preload("res://Ship/Walls/Door/Door.tscn")
const wall_scene = preload("res://Ship/Walls/Wall/Wall.tscn")
const floor_scene = preload("res://Ship/Walls/Floor/Floor.tscn")
const core_scene = preload("res://Ship/Walls/Core/Core.tscn")
const thruster_scene = preload("res://Ship/Walls/Thruster/Thruster.tscn")
const connector_scene = preload("res://Ship/Walls/Connector/Connector.tscn")

var ship : Ship = null

var _first_rect : Rect2i

var test_polygon = null

func get_rect():
	return _first_rect

func _load_hitbox():
	var shape : PackedVector2Array= _to_shape(_delete_edges(_create_edges()))

	ship.polygon = shape
	ship.hitbox.polygon = shape
	ship.visual.polygon = shape
	ship.area.polygon = shape


func _get_points(tile: Vector2i):
	# 1   2
	#  
	# 0   3  

	var tile_size = tile_set.tile_size
	
	return [ 
		Vector2(tile.x * tile_size.x, tile.y * tile_size.y + tile_size.y), # 0
		Vector2(tile.x * tile_size.x, tile.y * tile_size.y), # 1
		Vector2(tile.x * tile_size.x + tile_size.x, tile.y * tile_size.y), # 2
		Vector2(tile.x * tile_size.x + tile_size.x, tile.y * tile_size.y + tile_size.y) # 3
	]

func _get_points_rect(rect: Rect2):

	# 1   2
	#  
	# 0   3  

	var rect_position = rect.position * Vector2(tile_set.tile_size)
	var rect_size = rect.size * Vector2(tile_set.tile_size)
	return [ 
		Vector2(rect_position.x, rect_position.y + rect_size.y), # 0
		Vector2(rect_position.x, rect_position.y), # 1
		Vector2(rect_position.x + rect_size.x, rect_position.y), # 2
		Vector2(rect_position.x + rect_size.x, rect_position.y + rect_size.y) # 3
	]

func _get_lines(points, _scale = Limits.TILE_SCALE):
	return [
		[points[0] * _scale, points[1] * _scale],
		[points[1] * _scale, points[2] * _scale],
		[points[2] * _scale, points[3] * _scale],
		[points[3] * _scale, points[0] * _scale]
	]

func _create_edges():
	var edges = []
	var grid = get_used_cells()
	for tile in grid:
		for line in _get_lines(_get_points(tile)):
			edges.append(line)
	return edges

func _delete_edges(edges):
	var seen_edges = {}
	var marked_for_deletion = []
	
	for current_line in edges:
		var current_line_inverted = [current_line[1], current_line[0]]
		
		if seen_edges.has(current_line) or seen_edges.has(current_line_inverted):
			marked_for_deletion.append(current_line)
			marked_for_deletion.append(current_line_inverted)
		else:
			seen_edges[current_line] = true
			seen_edges[current_line_inverted] = true
	
	for line in marked_for_deletion:
		edges.erase(line)
	
	return edges

func _to_shape(edges):
	var result = PackedVector2Array()
	var next_line = edges[0]
	for index in range(edges.size()):
		for other_line in edges:
			if other_line == next_line: continue
			if next_line[1] == other_line[0]:
				next_line = other_line
				break
			elif next_line[1] == other_line[1]:
				next_line = [other_line[1], other_line[0]]
		
		result.append(next_line[0])
	return result

func update_center_of_mass():
	var grid = get_used_cells()
	var leftmost_point : float = grid[0].x
	var rightmost_point : float = grid[0].x
	var highest_point : float = grid[0].y
	var lowest_point : float = grid[0].y

	for point in grid:
		if point.x < leftmost_point: leftmost_point = point.x
		if point.x > rightmost_point: rightmost_point = point.x
		if point.y > lowest_point: lowest_point = point.y
		if point.y < highest_point: highest_point = point.y

	ship.center_of_mass = Vector2(5 * tile_set.tile_size.x * (0.5 + (leftmost_point + rightmost_point) / 2), 5 * tile_set.tile_size.y * (0.5 + (highest_point + lowest_point) / 2))

func load_ship(_ship, path : String) -> bool:
	ship = _ship
	clear()
	
	var save_file : FileAccess
	
	print_debug("Loading ship file..")

	if not FileAccess.file_exists("user://saves/ships/" + path + "/walls.dat"):
		if not FileAccess.file_exists("res://DefaultSave/ships/" + path + "/walls.dat"):
			return false
		else:
			save_file = FileAccess.open("res://DefaultSave/ships/" + path + "/walls.dat", FileAccess.READ)
	else:
		save_file = FileAccess.open("user://saves/ships/" + path + "/walls.dat", FileAccess.READ)
	
	var contents := []
	
	while save_file.get_position() != save_file.get_length():
		contents = [save_file.get_float(), save_file.get_float(), save_file.get_16(), save_file.get_float(), save_file.get_float(), save_file.get_16()]
		var tile:= Vector2()
		tile.x = contents[0]
		tile.y = contents[1]
		set_cell(tile, contents[2], Vector2i(contents[3], contents[4]), contents[5])

	save_file.close()

	print_debug("Loading ship hitbox..")

	_load_hitbox()

	print_debug("Updating ship center of mass..")
	
	update_center_of_mass()


	_first_rect = get_used_rect()

	# ship.original_wall_tile_map = self

	print_debug("Replacing tiles..")
	_replace_tiles()	

	return true
	
func _replace_tiles() -> bool:
	var layer := 0

	var atlas := tile_set.get_source(0) as TileSetAtlasSource
	var atlas_image := atlas.texture.get_image()

	for cellpos in get_used_cells():
		var cell := get_cell_tile_data(cellpos)

		var object_direction = get_cell_alternative_tile(cellpos)
		
		var tile_position = map_to_local(cellpos) * Limits.TILE_SCALE

		if cell == null:
				var _connector_object = connector_scene.instantiate()
				_connector_object.init(ship, cellpos)
				_connector_object.position = tile_position

				ship.wall_tiles.add_child(_connector_object)
				
				_connector_object.rotation_degrees = object_direction * 90

				set_cell(cellpos, -1)
				continue

		match cell.get_custom_data("type"):

			"floor":
				var _floor_object = floor_scene.instantiate()
				_floor_object.init(ship, cellpos)
				_floor_object.position = tile_position
				ship.wall_tiles.add_child(_floor_object)

			"door":

				var _door_object = door_scene.instantiate()
				_door_object.init(ship, cellpos)
				_door_object.direction = cell.get_custom_data("direction")
				_door_object.position = tile_position
				ship.wall_tiles.add_child(_door_object)
				var _floor_object = floor_scene.instantiate()
				_floor_object.init(ship, cellpos)
				_floor_object.position = tile_position
				ship.wall_tiles.add_child(_floor_object)


			"wall":

				var _wall_object = wall_scene.instantiate()
				_wall_object.init(ship, cellpos)
				_wall_object.position = tile_position
				ship.wall_tiles.add_child(_wall_object)

				_wall_object.light_occluder.occluder = cell.get_occluder(layer)
				_wall_object.light_occluder.scale = Vector2(1, 1) * Limits.TILE_SCALE
				
				var tile_image := atlas_image.get_region(atlas.get_tile_texture_region(get_cell_atlas_coords(cellpos)))
				
				for i in range(object_direction): tile_image.rotate_90(CLOCKWISE)

				var tile_texture := ImageTexture.create_from_image(tile_image)

				tile_texture.set_size_override(Vector2i(32, 32))
				_wall_object.set_texture(tile_texture)

			"core":
			
				var _core_object = core_scene.instantiate()
				_core_object.init(ship, cellpos)
				_core_object.position = tile_position
				ship.wall_tiles.add_child(_core_object)
				var _floor_object = floor_scene.instantiate()
				_floor_object.init(ship, cellpos)
				_floor_object.position = tile_position
				ship.wall_tiles.add_child(_floor_object)
				
			
			"thruster":
				var _thruster_object = thruster_scene.instantiate()
				_thruster_object.init(ship, cellpos, 150, 5, object_direction)
				_thruster_object.position = tile_position
	
				ship.wall_tiles.add_child(_thruster_object)

				_thruster_object.rotation_degrees = object_direction * 90
				
			
			"connector":
				var _connector_object = connector_scene.instantiate()
				_connector_object.init(ship, cellpos)
				_connector_object.position = tile_position

				ship.wall_tiles.add_child(_connector_object)
				
				_connector_object.rotation_degrees = object_direction * 90

	return true









