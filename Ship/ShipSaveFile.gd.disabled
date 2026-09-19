class_name ShipSaveFile extends Resource

@export var id : int
@export var path : String
@export var position : Vector2
@export var old_position : Vector2
@export var velocity : Vector2
@export var rotation : float

@export var destroyed_walls := []
@export var opened_doors := []
@export var pickedup_items := []

static func save():
	var files = []

	for ship in Ship.ships:
		var file = ShipSaveFile.new()
		file.id = ship.id

		file.path = ship.path if ship.path != "%player_ship_new" else "%player_ship_old"
		file.position = World.instance.get_distance_from_center(ship.global_position)
		file.old_position = ship._old_position
		file.velocity = ship.linear_velocity
		file.rotation = ship.rotation

		file.destroyed_walls = ship.destroyed_walls
		file.opened_doors = ship.opened_doors
		file.pickedup_items = ship.pickedup_items

		files.append(file)

	return files

func load(_npcs = [], _items = []):

	var _item_presets = []
	var _npc_presets = []

	for npc in _npcs: if id == npc.ship_id: _npc_presets.append(NPCPreset.new(npc.id, npc.nickname, npc.roles, npc.skin, npc.hair))
	for item in _items: if id == item.ship_id: _item_presets.append(ItemPreset.new(item.id, item.type, item.ship_slot_id))

	var custom_object_spawn = CustomObjectSpawn.create(_npc_presets, _item_presets)

	var ship = ShipManager.spawn_ship(position, path, custom_object_spawn, true)
	ship.id = id
	ship.rotation = rotation
	ship.linear_velocity = velocity
	ship.call_deferred("apply_changes", destroyed_walls, opened_doors)
	ship._old_position = old_position

	# spawn_ship(_position : Vector2, path : String = "station", custom_object_spawn : CustomObjectSpawn = null) -> void:
