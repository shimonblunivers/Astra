class_name Editor extends Node2D


@onready var console: Console = $HUD/Console/ConsoleLog
@onready var ship_editor: ShipEditor = $Ship
@onready var savemenu := $HUD/SavemenuUI

@onready var camera := $Camera2D

@onready var ship_name_label: LineEdit = $HUD/SavemenuUI/ShipName

@onready var ship_list := $HUD/SavemenuUI/Control/ShipList

@onready var inventory: Inventory = $HUD/Inventory

@onready var direction_label = $HUD/DirectionLabel
@onready var limit_rect = $LimitRect

var ships = []

static var instance

var inventory_open = false
var inventory_positions = Vector2(160, -165) # open, closed
func _unhandled_input(event: InputEvent) -> void:
	if event.is_action_pressed("editor_toggle_toolmenu"):
		inventory_open = !inventory_open
		var tween = create_tween()
		var duration = 0.5
		if inventory_open: tween.tween_property(inventory, "position", Vector2(inventory_positions.x, 0), duration).set_ease(Tween.EASE_OUT)
		else: tween.tween_property(inventory, "position", Vector2(inventory_positions.y, 0), duration).set_ease(Tween.EASE_IN)
	
	if event.is_action_pressed("game_toggle_menu"):
		_exit()

func _process(_delta):
	limit_rect.position.y = camera.position.y - limit_rect.size.y / 2

func center_camera():
	camera.position = Vector2(ShipEditor.starting_block_coords.x, ShipEditor.starting_block_coords.y)
	camera.zoom = Vector2(1, 1)

func _exit():
	self.queue_free()
	Player.main_player.camera.make_current()
	
	get_tree().paused = false
	#get_tree().set_deferred("paused", false)
	
	World.instance.visible = true
	World.instance.ui_node.visible = true
	World.used_builder = null

func _ready():
	Editor.instance = self

	limit_rect.visible = !Options.DEVELOPMENT_MODE
	
	_update_ship_list()

	ship_editor.inventory = inventory
	
	inventory.load_grid()
	
	if Player.main_player.owned_ship == null:
		ship_editor.load_ship("_start_ship", false)
	else:
		ship_editor.load_ship(Player.main_player.owned_ship.path, false)

	camera.make_current()

	center_camera()

	
func _on_save_pressed() -> void:
	if !ShipValidator.check_validity(ship_editor.wall_tile_map):
		ship_editor.console.print_out("[color=red]Ship does not meet the requirements for saving![/color]\nPlease check that you have a core in your ship.\nAlso check that all blocks are connected.")
		return
	if (ship_name_label.text == ""):
		ship_editor.save_ship()
		_update_ship_list()
		return
	ship_editor.save_ship(ship_name_label.text)
	_update_ship_list()

		
func _update_ship_list():
	ship_editor.evide_tiles()
	var ship_text = "[center][table=3]"
	var dir = DirAccess.open("user://saves/ships")
	if dir:
		dir.list_dir_begin()
		var file_name = dir.get_next()
		while file_name != "":
			if dir.current_is_dir():
				if !(!Options.DEVELOPMENT_MODE && file_name.begins_with('_') || !Options.DEVELOPMENT_MODE && file_name.begins_with('%')):
					ship_text += "[cell=1][left][url=" + file_name + "]" + file_name + "[/url][/left][/cell]"
					if !file_name.begins_with('_') && FileAccess.file_exists("user://saves/ships/" + file_name + "/details.dat"):
						var details = FileAccess.open("user://saves/ships/" + file_name + "/details.dat", FileAccess.READ)
						var price = details.get_16()
						ship_text += "[cell=1]      ->      [/cell][cell=1][right]"
						if price > ship_editor.current_ship_price + inventory.currency: ship_text += "[color=red]"
						ship_text += str(price)
						if price > ship_editor.current_ship_price + inventory.currency: ship_text += "[/color]"
						ship_text += " [img]res://UI/currency.png[/img]" + "[/right][/cell]"
						details.close()
					else:
						ship_text += "[cell=1][/cell][cell=1][/cell]"
					# ship_text += "[/url]"
			file_name = dir.get_next()
		dir.list_dir_end()
	
	if Options.DEVELOPMENT_MODE:
		dir = DirAccess.open("res://DefaultSave/ships")
		if dir:
			dir.list_dir_begin()
			var file_name = dir.get_next()
			while file_name != "":
				if dir.current_is_dir():
					ship_text += "[cell=1][left][url=" + file_name + "]" + file_name + "[/url][/left][/cell]"
					if !file_name.begins_with('_') && FileAccess.file_exists("user://saves/ships/" + file_name + "/details.dat"):
						var details = FileAccess.open("user://saves/ships/" + file_name + "/details.dat", FileAccess.READ)
						var price = details.get_16()
						ship_text += "[cell=1]      ->      [/cell][cell=1][right]"
						if price > ship_editor.current_ship_price + inventory.currency: ship_text += "[color=red]"
						ship_text += str(price)
						if price > ship_editor.current_ship_price + inventory.currency: ship_text += "[/color]"
						ship_text += " [img]res://UI/currency.png[/img]" + "[/right][/cell]"
						details.close()
					else:
						ship_text += "[cell=1][/cell][cell=1][/cell]"
				file_name = dir.get_next()
			dir.list_dir_end()
	
	ship_text += "[/table][/center]"
	ship_list.text = ship_text


func _on_load_pressed() -> void:
	var success
	if !ship_name_label.text.begins_with('_') && FileAccess.file_exists("user://saves/ships/" + ship_name_label.text + "/details.dat"):
		var details = FileAccess.open("user://saves/ships/" + ship_name_label.text + "/details.dat", FileAccess.READ)
		var price = details.get_16()
		if price > ship_editor.current_ship_price + inventory.currency && !Options.DEVELOPMENT_MODE:
			console.print_out("[color=red]Insufficient funds![/color]")
			return
	if (ship_name_label.text == ""): success = ship_editor.load_ship()
	else: success = ship_editor.load_ship(ship_name_label.text)
	if !success: console.print_out("[color=red]Ship with name '" + ship_name_label.text + "' was not found![/color]")
	else:
		_on_exit_pressed()
		center_camera()

func _on_open_savemenu_pressed() -> void:
	ShipValidator.autofill_floor(ShipEditor.instance.wall_tile_map)
	savemenu.visible = true
	$HUD/Savemenu.visible = false
	camera.locked = true
	_update_ship_list()

func _on_exit_pressed() -> void:
	savemenu.visible = false
	$HUD/Savemenu.visible = true
	camera.locked = false

func _on_ship_list_meta_clicked(meta: Variant) -> void:
	ship_name_label.text = meta

func _on_autofloor_pressed():
	ShipValidator.autofill_floor(ShipEditor.instance.wall_tile_map)

func _on_autofloor_button_toggled(toggled_on: bool):
	ShipEditor.autoflooring = toggled_on
	if toggled_on: ShipValidator.autofill_floor(ShipEditor.instance.wall_tile_map)

func _on_deploy_pressed() -> void:
	ShipValidator.autofill_floor(ShipEditor.instance.wall_tile_map)
	
	
	if !ShipValidator.check_validity(ship_editor.wall_tile_map):
		ship_editor.console.print_out("[color=red]Ship does not meet the requirements for saving![/color]\nPlease check that you have a core in your ship.\nAlso check that all blocks are connected.")
		return

	ship_editor.save_ship("%player_ship_new")
	
	
	# var path = "user://saves/ships"
	# var dir = DirAccess.open(path)

	# var ship_names = []
	# if dir:
	# 	dir.list_dir_begin()
	# 	var file_name = dir.get_next()
	# 	while file_name != "":
	# 		if dir.current_is_dir(): 
	# 			ship_names.append(file_name)
	# 		file_name = dir.get_next()

	# var saved_file_name = ""

	# var ship_num = 0
	# while saved_file_name == "":
	# 	if "%player_ship_" + str(ship_num) in ship_names: ship_num += 1
	# 	else: saved_file_name = "%player_ship_" + str(ship_num)
		

	# 
	# dir = DirAccess.open("user://saves/ships")
	# if dir:
	# 	dir.list_dir_begin()
	# 	var file_name = dir.get_next()
	# 	while file_name != "":
	# 		if file_name != saved_file_name:
	# 			if compare_files(path + "/" + file_name + "/walls.dat", path + "/" + saved_file_name + "/walls.dat"): 
	# 				if compare_files(path + "/" + file_name + "/objects.dat", path + "/" + saved_file_name + "/objects.dat"):
	# 					delete_directory(path + "/" + file_name)
						
	# 		file_name = dir.get_next()

	if World.used_builder != null:
		ShipManager.build_ship(World.used_builder, true, "%player_ship_new")
		Player.main_player.currency = ship_editor.inventory.currency
		
	_exit()

# ## Returns true if the files are the same
# func compare_files(path1 : String, path2 : String) -> bool:
# 	var content1 = FileAccess.get_file_as_bytes(path1)
# 	var content2 = FileAccess.get_file_as_bytes(path2)
# 	return content1 == content2
