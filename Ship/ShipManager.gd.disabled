class_name ShipManager extends Node2D


static var ship_scene = preload("res://Ship/Ship.tscn")

static var instance : ShipManager
static var number_of_ships = 0
static var main_station : Ship : 
	set(value):
		if value != null: value.freeze = true
		main_station = value

func _ready() -> void:
	instance = self

static func randomly_generate_ships():
	Player.main_player.owned_ship = ShipManager.spawn_ship(Vector2(-10000, -10000), "_start_ship")
	Player.main_player.owned_ship.linear_damp = 0

	print_debug("Spawning main station..")
	main_station = ShipManager.spawn_ship(Vector2(0, 0), "_station", null, false, true)

	# var ship_percentage = 25

	# var random = RandomNumberGenerator.new()

	# var ship_counter = 4

	# for x in range(-5, 6):
	# 	for y in range(-5, 6):
	# 		if x == 0 && y == 0: continue
	# 		if random.randi_range(0, ship_percentage) == 0:
	# 			if ship_counter <= 0: break
	# 			ship_counter -= 1
	# 			ShipManager.spawn_ship(Vector2(x * 200000, y * 200000), random_ship())


static func random_ship() -> String:
	var random := RandomNumberGenerator.new()
	return "_small_shuttle_" + str(random.randi_range(0, 4))

static func get_quest_ship_path(_mission_id : int) -> String:
	return random_ship()
	
static func spawn_ship(_position : Vector2, path : String = "_station", custom_object_spawn : CustomObjectSpawn = null, _from_save := false, lock_rotation := false) -> Ship:
	var _ship = ship_scene.instantiate()
	_ship.name = "Ship-" + str(_ship.id)
	instance.add_child(_ship)
	
	print_debug("Spawning ship at " + str(_position))
	_ship.load_ship(_position, path, custom_object_spawn, lock_rotation, _from_save)
	return _ship

static func build_ship(_builder : Builder, for_player : bool, path : String = "_station") -> Ship:
	var _ship : Ship = ship_scene.instantiate()
	_ship.name = "Ship-" + str(_ship.id)
	_builder.play_sound()
	instance.add_child(_ship)
	_ship.load_ship(_builder.get_spawn_position(), path, null, true, true)
	_ship.linear_velocity = _builder.ship.linear_velocity
	_ship.rotation = _builder.get_ship_rotation()


	if for_player: 
		if Player.main_player.owned_ship != null: 
			Player.main_player.deleting_ship(Player.main_player.owned_ship)
			Player.main_player.owned_ship.delete()
		Player.main_player.owned_ship = _ship
	_ship.connectors[0].connect_to(_builder.connector)
	
	return _ship
