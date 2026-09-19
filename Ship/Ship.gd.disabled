class_name Ship extends RigidBody2D


@onready var wall_tile_map := $WallTileMap
@onready var object_tile_map := $ObjectTileMap

@onready var wall_tiles := $WallTiles
@onready var object_tiles := $ObjectTiles
@onready var items := $Items

@onready var hitbox := $Hitbox
@onready var visual := $Visual
@onready var area := $Area/AreaHitbox
@onready var passengers_node := $Passengers
@onready var timer := $Timer

# var original_wall_tile_map : TileMap
# var original_object_tile_map : TileMap

static var ships = []

var id : int

var polygon

var dock_position : Vector2 = Vector2(100, 100)

var passengers := []

var used_item_slots = 0

var controlled_by = null

var acceleration := Vector2.ZERO

var rotation_speed : float = 0 

var thrust_power : Vector4 = Vector4(0, 0, 0, 0) # LEFT UP RIGHT DOWN

var thrusters := [[], [], [], []] # LEFT UP RIGHT DOWN

var interactables := []

var path : String

var destroyed_walls := []
var opened_doors := []
var pickedup_items := []

var spawning = true

var comfortable_rotation_degrees : float = 0

var connectors = []

var from_save = false

# TODO: ✅ Fix bugging when the player exits at high speed

# TODO: ✅ Fix infinity position while moving too fast

# TODO: ✅ Fix player moving into walls when encountering moving ship

# TODO: ✅ Fix Ship center

# TODO: ✅ Add Thrusters

# TODO: ✅ Add Ship Connector

# TODO: Make Area2Ds for each room

# TODO: Make ships destroyable by collisions

# TODO: Add Canons

# TODO: Add Radar

# TODO: Create planets/moons/asteroids

var _old_position = position
var difference_in_position := Vector2.ZERO

static func get_ship(_id : int) -> Ship:
	for ship in ships:
		if ship.id == _id: return ship
	return null

func set_angle(angle : int):
	comfortable_rotation_degrees = angle
	for npc in NPC.npcs:
		if npc.ship == self:
			npc.rotation_degrees = angle

func _ready() -> void:
	Ship.ships.append(self)

func load_ship(_position : Vector2, _path : String, custom_object_spawn : CustomObjectSpawn, _lock_rotation : bool = false, _from_save := false) -> void:
	global_position = _position
	_old_position = _position
	path = _path
	name = path + "-" + str(id)

	print_debug("Loading ship: " + name)

	mass = 1

	from_save = _from_save

	print_debug("Loading walls..")
	wall_tile_map.load_ship(self, _path)

	print_debug("Loading objects..")
	object_tile_map.load_ship(self, _path, custom_object_spawn, _from_save)
	mass -= 1

	id = ShipManager.number_of_ships
	ShipManager.number_of_ships += 1

	for direction in thrusters: for thruster in direction: thruster.set_status(false)

	if !_lock_rotation:
		var rng = RandomNumberGenerator.new()
		rotation = rng.randf_range(0, 2 * PI)

func get_tile(coords : Vector2i):
	for tile in wall_tiles.get_children():
		if (tile is ShipPart):
			if (coords == tile.tilemap_coords):
				return tile
	return null

func get_closest_point(point1 : Vector2) -> Vector2:
	if polygon == null: return global_position
	var closest = polygon[0].rotated(global_rotation) + global_position
	for point2 in polygon:
		point2 = point2.rotated(global_rotation) + global_position
		if closest.distance_to(point1) > point2.distance_to(point1):
			closest = point2
	return closest

func set_connector(connector_index : int, connector : Connector) -> void:
	connectors[connector_index].connect_to(connector)

func _integrate_forces(state: PhysicsDirectBodyState2D) -> void:
	if (global_position - Player.main_player.global_position).length() > Player.main_player.update_range: return
	acceleration = Vector2.ZERO
	rotation_speed = 0

	if controlled_by != null: 
		control()

	update_thrusters()
	update_side_trusters()

	if controlled_by != null: 
		acceleration = acceleration.rotated(global_rotation)
		if acceleration != Vector2.ZERO: state.apply_central_impulse(acceleration)
		if rotation_speed != 0: state.apply_torque_impulse(rotation_speed)

	# if abs(get_linear_velocity().x) > Limits.VELOCITY_MAX or abs(get_linear_velocity().y) > Limits.VELOCITY_MAX:
	# 	var new_speed = get_linear_velocity().normalized()
	# 	new_speed *= Limits.VELOCITY_MAX
	# 	set_linear_velocity(new_speed)


func _physics_process(_delta: float) -> void:
	if spawning && !from_save:
		for exception in get_collision_exceptions():
			remove_collision_exception_with(exception)
		var collision = move_and_collide(Vector2.ZERO)
		if collision == null: spawning = false
		else: 
			add_collision_exception_with(collision.get_collider())
			move_and_collide(Vector2(collision.get_depth() * cos(collision.get_angle()) * 10,  10 * collision.get_depth() * sin(collision.get_angle())))

	difference_in_position = position - _old_position
	# print("Ship moved by: ", _difference_in_position)
	# for passenger in passengers: 
	# 	if passenger.is_in_group("NPC"):
	# 		passenger.legs.position = passenger.legs_offset + difference_in_position
	# area.position = -difference_in_position
	_old_position = position

	# if Player.main_player.parent_ship == self: hitbox.position = (-difference_in_position).rotated(-global_rotation) # Hitbox counter bug
	# else: hitbox.position = Vector2.ZERO

func control():
	
	var direction := Input.get_vector("ui_left", "ui_right", "ui_up", "ui_down")
	var rotation_direction := Input.get_axis("game_turn_left","game_turn_right")

	var _rotation_power = 4000 * mass

	if !controlled_by.alive: direction = Vector2.ZERO

	if direction.x < 0: acceleration.x -= thrust_power.x
	elif direction.x > 0: acceleration.x += thrust_power.z

	if direction.y < 0: acceleration.y -= thrust_power.y
	elif direction.y > 0: acceleration.y += thrust_power.w

	if ((thrusters[0].size() + thrusters[1].size() + thrusters[2].size() + thrusters[3].size()) == 0): rotation_speed = 0
	else: rotation_speed = _rotation_power * rotation_direction + rotation_direction * (thrusters[0].size() + thrusters[1].size() + thrusters[2].size() + thrusters[3].size()) * _rotation_power / 2

func update_side_trusters():
	for thruster_list in thrusters:
		for thruster in thruster_list:
			thruster.side_thrusters(rotation_speed)

func apply_changes(_destroyed_walls = [], _opened_doors = []):

	for coords in _opened_doors:
		var tile = get_tile(coords)
		if tile is Door: tile.open()

	for coords in _destroyed_walls:
		var tile = get_tile(coords)
		if tile is Wall: tile.destroy()

	# for item_id in _pickedup_items:
	# 	var item = Item.get_item(item_id)
	# 	print(item.name)
	# 	if item != null: item.delete()

func update_thrusters():
	for thruster in thrusters[0]: if thruster.running != (acceleration.x < 0): 
		thruster.set_status(acceleration.x < 0)
	for thruster in thrusters[2]: if thruster.running != (acceleration.x > 0): 
		thruster.set_status(acceleration.x > 0)
	for thruster in thrusters[1]: if thruster.running != (acceleration.y < 0): 
		thruster.set_status(acceleration.y < 0)
	for thruster in thrusters[3]: if thruster.running != (acceleration.y > 0): 
		thruster.set_status(acceleration.y > 0)

func get_rect():
	return wall_tile_map.get_rect()

func get_tile_size():
	return Vector2(wall_tile_map.tile_set.tile_size) * wall_tile_map.scale

func start_controlling(player):
	controlled_by = player

func stop_controlling():
	controlled_by = null

func _on_area_area_entered(_area:Area2D) -> void:
	if _area.is_in_group("PlayerInteractArea"):
		var body = _area.get_parent()
		if (!body.spawned): 				return
		if (body in passengers): 			return
		passengers.append(body)
		body.get_in(self)

	# if body.is_in_group("Player"):
		# if body.max_impact_velocity < (body.acceleration - _difference_in_position).length(): body.kill()   TODO: OPRAVIT

func _on_area_area_exited(_area:Area2D) -> void:
	if _area.is_in_group("PlayerInteractArea"):
		var body = _area.get_parent()
		if (!body.spawned): 				return
		if !(body in passengers): 			return
		passengers.erase(body)
		body.get_off(self)

func delete():
	hitbox.disabled = true
	for connector in connectors: connector.connect_to(null)
	if self == Player.main_player.parent_ship: Player.main_player.reparent(World.instance)
	ShipManager.number_of_ships -= 1
	Ship.ships.erase(self)
	queue_free()

func _unhandled_input(event: InputEvent):
	if controlled_by == null: return
	if event.is_action_pressed("game_dock_ship"):
		var connected = false
		for connector in connectors:
			if connector.connected_to != null:
				connector.connect_to(null)
				connected = true
		if !connected && connectors[0].connectors_in_range.size() > 0:
			connectors[0].connect_to(connectors[0].connectors_in_range[0])

func _draw() -> void:
	if !visual.visible: return
	if polygon: 
		for point in polygon:
			draw_circle(point.rotated(global_rotation) + global_position, 16, Color.RED)


	if wall_tile_map.test_polygon:
		for point in wall_tile_map.test_polygon:
			draw_circle(Vector2(point) + global_position + Vector2(0, -200), 16, Color.BLUE)