extends TileMapLayer


const builder_scene = preload("res://Ship/Objects/Builder/Builder.tscn")
const helm_scene = preload("res://Ship/Objects/Helm/Helm.tscn")
const NPC_scene = preload("res://Character/NPC/NPC.tscn")

var ship : Ship = null
	 

func load_ship(_ship, path : String, custom_object_spawn : CustomObjectSpawn, _from_save := false) -> bool:
	ship = _ship
	
	clear()
	
	var save_file : FileAccess

	if not FileAccess.file_exists("user://saves/ships/" + path + "/objects.dat"):
		if not FileAccess.file_exists("res://DefaultSave/ships/" + path + "/objects.dat"):
			return false
		else:
			save_file = FileAccess.open("res://DefaultSave/ships/" + path + "/objects.dat", FileAccess.READ)
			
	else:
		save_file = FileAccess.open("user://saves/ships/" + path + "/objects.dat", FileAccess.READ)
	
	var contents := []
	
	while save_file.get_position() != save_file.get_length():
		contents = [save_file.get_float(), save_file.get_float(), save_file.get_16(), save_file.get_float(), save_file.get_float(), save_file.get_16()]
		var tile:= Vector2()
		tile.x = contents[0]
		tile.y = contents[1]
		set_cell(tile, contents[2], Vector2i(contents[3], contents[4]), contents[5])

	save_file.close()
	
	# ship.original_object_tile_map = self

	return _replace_interactive_tiles(custom_object_spawn, _from_save)
	

func _replace_interactive_tiles(custom_object_spawn : CustomObjectSpawn, _from_save : bool) -> bool:

	var npc_index = 0
	var item_index = 0

	var item_slot = 0

	for cellpos in get_used_cells():
		var cell = get_cell_tile_data(cellpos)

		var tile_position = map_to_local(cellpos) * Limits.TILE_SCALE

		match cell.get_custom_data("type"):
			"helm":
				var helm_object = helm_scene.instantiate()
				helm_object.init(ship, cellpos)
				helm_object.position = tile_position

				ship.object_tiles.add_child(helm_object)


			"builder":
				var builder_object = builder_scene.instantiate()
				builder_object.init(ship, cellpos)
				builder_object.position = tile_position

				ship.object_tiles.add_child(builder_object)

				
			"npc":
				var NPC_object = NPC_scene.instantiate()
				NPC_object.spawn_point = tile_position
				NPC_object.spawn()

				if custom_object_spawn != null && custom_object_spawn.npc_presets.size() && npc_index < custom_object_spawn.npc_presets.size():
					NPC_object.init(custom_object_spawn.npc_presets[npc_index].id, custom_object_spawn.npc_presets[npc_index].nickname, custom_object_spawn.npc_presets[npc_index].roles, custom_object_spawn.npc_presets[npc_index].colors, custom_object_spawn.npc_presets[npc_index].hair)
					npc_index += 1
				elif !_from_save:
					NPC_object.init()

				NPC_object.ship = ship
				ship.passengers_node.add_child(NPC_object)
				ship.passengers.append(NPC_object)
				

			"item":
					
				var random := RandomNumberGenerator.new()
				var scaling = 4 * Limits.TILE_SCALE
				var offset = Vector2(scaling - random.randf() * scaling * 2, scaling - random.randf() * scaling * 2)
				if custom_object_spawn != null && custom_object_spawn.item_presets.size() && item_index < custom_object_spawn.item_presets.size():
					var _in = item_index
					for i in custom_object_spawn.item_presets.size():
						if custom_object_spawn.item_presets[i].ship_slot_id == item_slot:
							var _new_item = Item.spawn(custom_object_spawn.item_presets[item_index].type, to_global(tile_position) + offset, custom_object_spawn.item_presets[item_index].id, ship, item_slot)
							item_index += 1
					if _in == item_index:
						for i in custom_object_spawn.item_presets.size():
							if custom_object_spawn.item_presets[i].ship_slot_id < 0:
								var _new_item = Item.spawn(custom_object_spawn.item_presets[item_index].type, to_global(tile_position) + offset, custom_object_spawn.item_presets[item_index].id, ship, item_slot)
								item_index += 1
				elif !_from_save:
					Item.spawn(Item.random_item(), to_global(tile_position) + offset, -1, ship, item_slot)

				item_slot += 1

	return true
