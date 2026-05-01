class_name ShipEditor extends Node2D


@onready var console: Console = $"../HUD/Console/ConsoleLog"
@onready var wall_tile_map := $WallTileMap
@onready var object_tile_map := $ObjectTileMap

static var instance
static var tool_preview
var inventory: Inventory

static var directions = {
	0: "right",
	1: "down",
	2: "left",
	3: "up",
}

static var direction := 0

static var autoflooring = false

static var tools := {}

static var tool: Tool = null

static var current_ship_price: int = 0

static var starting_block_coords = Vector2.ZERO

# TODO: ✅ Make placing tiles smoother

# TODO: ✅ Create menu for tools

# TODO: ✅ Make money (energy) system

# TODO: ✅ Add autoflooring

# TODO: Add Interactables

func evide_tiles():
	starting_block_coords = Vector2.ZERO
	for key in tools.keys():
		tools[key].number_of_instances = 0
		
	current_ship_price = 0

	for coords in wall_tile_map.get_used_cells():
		var type = ShipValidator.get_tile_type(wall_tile_map, coords)
		if type == "connector": starting_block_coords = Vector2(coords) * 32 + Vector2(16, 16) + global_position
		if type in tools.keys():
			tools[type].number_of_instances += 1
			current_ship_price += tools[type].price
	for coords in object_tile_map.get_used_cells():
		var type = ShipValidator.get_tile_type(object_tile_map, coords)
		if type in tools.keys():
			tools[type].number_of_instances += 1
			current_ship_price += tools[type].price

func _ready() -> void:
	load_tools()

	instance = self
	tool_preview = $"../HUD/ToolPreview"
	ShipEditor.change_tool("wall")

	# ShipValidator.autofill_floor(wall_tile_map)

	update_preview.call_deferred()
	
func update_preview():
	Editor.instance.direction_label.text = "Direction: " + directions[direction]
	ShipEditor.update_preview_rotation()


func load_tools():
	var path = "res://Editor/Tools"
	var dir = DirAccess.open(path)
	if dir:
		dir.list_dir_begin()
		var file_name = dir.get_next()
		while file_name != "":
			if !dir.current_is_dir():
				if '.tres.remap' in file_name:
					file_name = file_name.trim_suffix('.remap')
				if ".tres" in file_name:
					load(path + "/" + file_name).create()
			file_name = dir.get_next()

static func get_mouse_tile(tilemap: TileMapLayer) -> Vector2i:
	return tilemap.local_to_map(instance.to_local(instance.get_global_mouse_position()))

func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventMouseMotion || event is InputEventMouseButton:
		if event.button_mask == 1:
			use_tool()
		elif event.button_mask == 2 && (!ShipValidator.get_tile_type(wall_tile_map, ShipEditor.get_mouse_tile(wall_tile_map)) == "connector" || Options.DEVELOPMENT_MODE):
			if ShipValidator.get_tile_type(object_tile_map, ShipEditor.get_mouse_tile(object_tile_map)) in tools && tools[ShipValidator.get_tile_type(object_tile_map, ShipEditor.get_mouse_tile(object_tile_map))].object:
				ShipEditor.sell_tile(object_tile_map, ShipEditor.get_mouse_tile(object_tile_map))
			else:
				ShipEditor.sell_tile(wall_tile_map, ShipEditor.get_mouse_tile(wall_tile_map))

	
	if event.is_action_pressed("editor_change_direction"):
		ShipEditor.direction = (ShipEditor.direction + 1) % 4
		update_preview()
func use_tool() -> void:
	if tool == null: return
	if !Options.DEVELOPMENT_MODE && !(tool.number_of_instances < tool.world_limit || tool.world_limit < 0): return
	var tilemap: TileMapLayer = object_tile_map if tool.object else wall_tile_map
	var tile: Vector2i = ShipEditor.get_mouse_tile(tilemap)
	if ShipValidator.get_tile_type(tilemap, tile) == tool.name: return
	var placing_on_something = false
	if tool.placeable_on_atlas_choords != Vector2i(-1, -1):
		placing_on_something = true
		if tool.placeable_on_atlas_choords != wall_tile_map.get_cell_atlas_coords(ShipEditor.get_mouse_tile(wall_tile_map)):
			return

	if ShipValidator.get_tile_type(tilemap, tile) == "connector": return
	ShipEditor.sell_tile(tilemap, tile, false)

	if tool.price != 0 && !Inventory.add_currency(-tool.price) && !Options.DEVELOPMENT_MODE:
		return

	tool.number_of_instances += 1

	if !placing_on_something:
		tilemap.set_cells_terrain_connect([tile], 0, -1, false)

	if tool.terrain_id != -1:
		tilemap.set_cells_terrain_connect([tile], 0, tool.terrain_id)
	elif tool.atlas_coords != Vector2i(-1, -1):
		tilemap.set_cell(tile, 0, tool.atlas_coords, direction if tool.rotatable else 0)

	if tool.name in ShipValidator.walls && autoflooring:
		ShipValidator.autofill_floor(tilemap)

static func sell_tile(tilemap: TileMapLayer, coords: Vector2i, delete_tile := true, react_autofill := false) -> bool:
	var sold = false
	var type = ShipValidator.get_tile_type(tilemap, coords)

	if type in tools.keys():
		tools[type].number_of_instances -= 1
		Inventory.add_currency(tools[type].price, delete_tile)
		sold = true

	if delete_tile: tilemap.set_cells_terrain_connect([coords], 0, -1, false)

	if autoflooring && !react_autofill:
		ShipValidator.autofill_floor(tilemap)

	return sold

static func change_tool(key: String) -> void:
	tool = tools[key]
	tool_preview.texture = tool.texture
	update_preview_rotation()

static func update_preview_rotation():
	if tool.rotatable:
		tool_preview.rotation_degrees = 90 * direction
	else:
		tool_preview.rotation_degrees = 0
	
func save_ship(path: String = "_default_ship") -> void:
	evide_tiles()

	var location: String

	if path.begins_with("_"): location = "res://DefaultSave/ships/"
	else: location = "user://saves/ships/"

	DirAccess.make_dir_recursive_absolute(location + path + "/")
	
	var walls_save_file := FileAccess.open(location + path + "/walls.dat", FileAccess.WRITE)
	var objects_save_file := FileAccess.open(location + path + "/objects.dat", FileAccess.WRITE)
	var details_save_file := FileAccess.open(location + path + "/details.dat", FileAccess.WRITE)
	
	for cell in wall_tile_map.get_used_cells():
		walls_save_file.store_float(cell.x) # 0
		walls_save_file.store_float(cell.y) # 1
		walls_save_file.store_16(wall_tile_map.get_cell_source_id(Vector2i(cell.x, cell.y))) # 2
		walls_save_file.store_float(wall_tile_map.get_cell_atlas_coords(Vector2i(cell.x, cell.y)).x) # 3
		walls_save_file.store_float(wall_tile_map.get_cell_atlas_coords(Vector2i(cell.x, cell.y)).y) # 4
		walls_save_file.store_16(wall_tile_map.get_cell_alternative_tile(Vector2i(cell.x, cell.y))) # 5
		
	for cell in object_tile_map.get_used_cells():
		objects_save_file.store_float(cell.x) # 0
		objects_save_file.store_float(cell.y) # 1
		objects_save_file.store_16(object_tile_map.get_cell_source_id(Vector2i(cell.x, cell.y))) # 2
		objects_save_file.store_float(object_tile_map.get_cell_atlas_coords(Vector2i(cell.x, cell.y)).x) # 3
		objects_save_file.store_float(object_tile_map.get_cell_atlas_coords(Vector2i(cell.x, cell.y)).y) # 4
		objects_save_file.store_16(object_tile_map.get_cell_alternative_tile(Vector2i(cell.x, cell.y))) # 5

	details_save_file.store_16(current_ship_price)

	walls_save_file.close()
	objects_save_file.close()
	details_save_file.close()
	
	console.print_out("Ship saved as: " + path)
	
func load_ship(path: String = "_default_ship", charge := true) -> bool:
	var location: String

	if path.begins_with("_"): location = "res://DefaultSave/ships/"
	else: location = "user://saves/ships/"

	if not FileAccess.file_exists(location + path + "/walls.dat"):
		return false
	if not FileAccess.file_exists(location + path + "/objects.dat"):
		return false

	Editor.instance.inventory.currency += current_ship_price

	# if !path.begins_with('_') && FileAccess.file_exists(location + path + "/details.dat"):
	# 	var details = FileAccess.open(location + path + "/details.dat", FileAccess.READ)
	# 	var price = details.get_16()
		

	wall_tile_map.clear()
	object_tile_map.clear()
	
	var walls_save_file := FileAccess.open(location + path + "/walls.dat", FileAccess.READ)
	var objects_save_file := FileAccess.open(location + path + "/objects.dat", FileAccess.READ)
	
	var contents := []
	
	while walls_save_file.get_position() != walls_save_file.get_length():
		contents = [walls_save_file.get_float(), walls_save_file.get_float(), walls_save_file.get_16(), walls_save_file.get_float(), walls_save_file.get_float(), walls_save_file.get_16()]
		var tile := Vector2()
		tile.x = contents[0]
		tile.y = contents[1]
		wall_tile_map.set_cell(tile, contents[2], Vector2i(contents[3], contents[4]), contents[5])

	while objects_save_file.get_position() != objects_save_file.get_length():
		contents = [objects_save_file.get_float(), objects_save_file.get_float(), objects_save_file.get_16(), objects_save_file.get_float(), objects_save_file.get_float(), objects_save_file.get_16()]
		var tile := Vector2()
		tile.x = contents[0]
		tile.y = contents[1]
		object_tile_map.set_cell(tile, contents[2], Vector2i(contents[3], contents[4]), contents[5])

	walls_save_file.close()
	objects_save_file.close()

	evide_tiles()

	if charge:
		inventory.currency -= current_ship_price
	inventory.currency_value.text = str(inventory.currency)
	
	console.print_out("Ship loaded: " + path)
	return true
