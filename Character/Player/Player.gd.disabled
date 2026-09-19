class_name Player extends Character

@onready var animated_sprite : AnimatedSprite2D = $AnimatedSprite2D

@onready var walk_sound : AudioStreamPlayer2D = $Sounds/Walk
@onready var camera : Camera2D = $Camera2D
@onready var vision : PointLight2D = $Vision/Light

@onready var interact_area = $InteractArea

@onready var respawn_timer = $RespawnTimer

@onready var pickup = $Pickup

static var main_player : Player

var currency : float :
	set(value):
		currency = value
		currency_updated_signal.emit()

var _sprite_dir := 69

var ship_controlled = null

var normal_zoom : float = 1

var normal_vision : float = 4
var driving_vision : float = 0.4

var suit = true

var use_range : float = 1000

var acceleration = Vector2(0, 0)
var _old_position = Vector2(0, 0)

var hovering_controllables := []

var controllables_in_use := []

var passenger_on := []
var parent_ship : Ship = null

var dim_acceleration_for_frames = 0

var camera_difference = Vector2.ZERO
signal currency_updated_signal

var _damage_timer : float = 0
var _regen_timer : float = 0

var owned_ship : Ship :
	set(value):
		if value != null:
			value.linear_damp = 0
		owned_ship = value

var invincible := false

var _locked_rotating = false

var update_range = 10000

# TODO: ✅ Make player controling zoom out so it's in the center of ship and is scalable with the ship size

# TODO: ✅ Add floating velocity

# TODO: ✅ Edit player vision so object that are in the dark cannot be seen (Using lights as mask)

# TODO: ✅ Add Damage & Death

# TODO: ✅ Fix wrong hitbox while ship moving

# TODO: ✅ Fix Michael Jackson walking

# TODO: ✅ Change sounds according to walking terrain

# TODO: ✅ Add load/save

# TODO: ✅ Add animations


var turn_tween : Tween

func add_currency(amount : int):
	currency += amount
	UIManager.currency_change_effect(amount)

func floating():
	return passenger_on.size() == 0

func get_in(ship):
	dim_acceleration_for_frames = 5
	if (ship in passenger_on): return
	passenger_on.append(ship)
	if passenger_on.size() == 1: rotate_to_ship()
	# print(acceleration, " ; ", ship.difference_in_position)
	if max_impact_velocity < (acceleration - ship.difference_in_position).length():
		kill()

func rotate_to_ship():
	if _locked_rotating: return
	if turn_tween: turn_tween.kill()

	var turn_speed = abs(rotation_degrees / 150)
	turn_tween = create_tween()

	if rotation_degrees > 180:
		turn_tween.tween_property(self, "rotation_degrees", 360, turn_speed)
	else:
		turn_tween.tween_property(self, "rotation_degrees", 0, turn_speed)


func get_off(ship):
	if turn_tween: turn_tween.kill()
	passenger_on.erase(ship)

func change_ship(ship):		
	if ship == null: return
	parent_ship = ship
	# parent_ship.hitbox.position = (parent_ship.difference_in_position).rotated(parent_ship.global_rotation)
	call_deferred("reparent", ship.passengers_node)
	if !floating(): 
		_change_ship_rotate.call_deferred()
	# parent_ship.make_invulnerable()

func _change_ship_rotate():
	rotate_to_ship()
	_locked_rotating = true
	$LockRotationTimer.start()

func _unhandled_input(event: InputEvent):
	if Options.DEVELOPMENT_MODE:
		if event.is_action_pressed("teleport_to_quest"):
			if QuestManager.get_quest(QuestManager.highlighted_quest_id):
				if QuestManager.get_quest(QuestManager.highlighted_quest_id).get_target():
					godmode = true
					teleport(QuestManager.get_quest(QuestManager.highlighted_quest_id).get_target().global_position);
			else:
				godmode = true
				teleport(ShipManager.main_station.global_position)
		if event.is_action_pressed("teleport_to_ship_editor"):
			teleport(Vector2(3300, -3100))

		if event.is_action_pressed("debug_die"):
			World.save_file.save_world(true)
			# parent_ship.delete()

		if event.is_action_pressed("debug_spawn"):
			# spawn()
			# Item.spawn(Item.types["Chip"], get_global_mouse_position())
			# ShipManager.spawn_ship(get_global_mouse_position(), "small_shuttle")
			add_currency(1500)
	

	
	if event.is_action_pressed("development_mode"):
		Options.DEVELOPMENT_MODE = !Options.DEVELOPMENT_MODE
		UIManager.instance.loading_screen_node.visible = !Options.DEVELOPMENT_MODE
		UIManager.instance.floating.visible = Options.DEVELOPMENT_MODE
		UIManager.instance.player_position.visible = Options.DEVELOPMENT_MODE

	if alive:
		if event.is_action_pressed("game_control"):
			for controllable in hovering_controllables:
				controllable.interact()
				return
			for controllable in controllables_in_use:
				if controllable in hovering_controllables: continue
				controllable.player_in_range = self
				controllable.interact()
				controllable.player_in_range = null
				return

func teleport(pos : Vector2):
	global_position = pos
	_old_position = World.instance.get_distance_from_center(global_position)

func spawn(pos := spawn_point, _acceleration := Vector2.ZERO, _rotation = null):
	if !alive: animated_sprite.play("Idle")
	alive = true
	spawned = true
	health = max_health
	if _rotation != null: global_rotation = _rotation
	change_ship(ObjectList.get_closest_ship(global_position))
	_locked_rotating = false
	global_position = pos - World.instance._center_of_universe
	_old_position = World.instance.get_distance_from_center(global_position)

	invincible = true
	$InvincibilityTimer.start()
	health_updated_signal.emit()

func _ready():
	super()
	main_player = self

	spawn_point = Vector2(7777, -69)
	
	nickname = "Samuel"
	await get_tree().process_frame # WAIT FOR THE WORLD TO LOAD AND THE POSITION TO UPDATE // WAIT FOR NEXT FRAME
	animated_sprite.play("Idle")
	

	World.save_file.load_world()


func kill():
	if !alive || !spawned || invincible || godmode: return
	health = 0
	alive = false

	if ship_controlled != null: control_ship(ship_controlled)

	animated_sprite.play("Death")
	health_updated_signal.emit()
	died_signal.emit()
	respawn_timer.start()

func _process(delta):
	if alive:
		if floating():
			_regen_timer = 0
			_damage_timer += delta * 2
			if _damage_timer >= 1:
				_damage_timer = 0
				damage(5)

		else:
			_damage_timer = 0
			if health != max_health:
				_regen_timer += delta * 3
				if _regen_timer >= 1:
					damage(-1)
					_regen_timer = 0

	pickup.position = (- acceleration).rotated(-global_rotation)
	

func _in_physics(delta: float) -> void:
	# print("Player position: ", position)
	if passenger_on.size() == 1 && passenger_on[0] != parent_ship:
		change_ship(passenger_on[0])

	if ship_controlled == null: 
		_move(delta)
	else:
		camera.offset = camera_difference.rotated(global_rotation)


	# if parent_ship != null: World.instance.shift_origin(-parent_ship.global_transform.origin) # Moving the world origin to remove flickering bugs

func control_ship(ship):
	if ship != null:
		ship_controlled = ship
		walk_sound.stop()
		_sprite_dir = 0
		animated_sprite.flip_h = false
		animated_sprite.play("Idle")
		
		ship.start_controlling(self)
		change_view(0)
	else:
		change_view(1)
		if ship_controlled != null: ship_controlled.stop_controlling()
		ship_controlled = null



func _move(_delta: float) -> void:
	var direction := Input.get_vector("ui_left", "ui_right", "ui_up", "ui_down") # Get input keys
	var rotation_direction := Input.get_axis("game_turn_left","game_turn_right")
	var running := Input.get_action_strength("game_run")

	var _sound_pitch_range := [0.9, 1.1] # Sound variation
	
	if !alive: 
		direction = Vector2.ZERO # Death check
		rotation_direction = 0
	
	acceleration = World.instance.get_distance_from_center(global_position) - _old_position # Acceleration get by the difference of the position
	_old_position = World.instance.get_distance_from_center(global_position)

	# print("f: ", floating(), "ACC: ", acceleration)

	# if abs(acceleration.x) > Limits.VELOCITY_MAX or abs(acceleration.y) > Limits.VELOCITY_MAX: # Speed limitation, need to redo that tho
	# 	var new_speed = acceleration.normalized()
	# 	new_speed *= Limits.VELOCITY_MAX
	# 	acceleration = new_speed

	velocity = (direction * (SPEED + RUN_SPEED_MODIFIER * running)).rotated(global_rotation) # velocity // the _fix_position is help variable made to remove bug

	if parent_ship != null:
		if floating(): 	# If outside of the ship
			rotate(deg_to_rad(TURN_SPEED * rotation_direction))
			legs.position = legs_offset - (acceleration).rotated(-global_rotation) # Counter steering the bug, where every hitbox of the ship shifts
			if suit == false: 
				velocity = Vector2(0, 0) # No control over the direction u r flying if you don't have a suit
			else: 
				velocity *= .01 # Taking the velocity and dividing it by 100, so the player isn't so fast in the space like in ship

			if dim_acceleration_for_frames <= 0:
				velocity += (acceleration - parent_ship.difference_in_position) / _delta # Removing the parent_ship (ship he is attached to) velocity, so the acceleration won't throw him into deep space
			else:
				var _dim_factor = 10
				velocity += (acceleration - parent_ship.difference_in_position) / (_delta * _dim_factor)
		else:
			legs.position = legs_offset - (passenger_on[0].difference_in_position).rotated(-global_rotation) # Again the counter steering against the bug

	if dim_acceleration_for_frames > 0:
		dim_acceleration_for_frames -= 1

	# if get_last_slide_collision():
	# velocity = velocity.move_toward(Vector2(0, 0), 40)
	# print("dampening now  velocity:", velocity)
	# print(direction)
	move_and_slide()

	# animation things down here..

	# elif _fix_position == position && passenger_on[0].linear_velocity != Vector2.ZERO:
	# 	position += acceleration

	if !floating() && direction.x < 0:
		if !walk_sound.playing && !floating(): 
			walk_sound.pitch_scale = randf_range(_sound_pitch_range[0], _sound_pitch_range[1])
			walk_sound.play()
		if _sprite_dir != 1:
			_sprite_dir = 1
			animated_sprite.flip_h = true
			animated_sprite.play("WalkToSide")
		
	elif !floating() && direction.x > 0:
		if !walk_sound.playing && !floating(): 
			walk_sound.pitch_scale = randf_range(_sound_pitch_range[0], _sound_pitch_range[1])
			walk_sound.play()
		if _sprite_dir != 2:
			_sprite_dir = 2
			animated_sprite.flip_h = false
			animated_sprite.play("WalkToSide")
		
	elif !floating() && direction.y > 0: 
		if !walk_sound.playing && !floating(): 
			walk_sound.pitch_scale = randf_range(_sound_pitch_range[0], _sound_pitch_range[1])
			walk_sound.play()
		if _sprite_dir != 3:
			_sprite_dir = 3
			animated_sprite.flip_h = false
			animated_sprite.play("WalkDown")
			
	elif !floating() && direction.y < 0: 
		if !walk_sound.playing && !floating(): 
			walk_sound.pitch_scale = randf_range(_sound_pitch_range[0], _sound_pitch_range[1])
			walk_sound.play()
		if _sprite_dir != 4:
			_sprite_dir = 4
			animated_sprite.flip_h = false
			animated_sprite.play("WalkUp")
		
	else:
		if alive && _sprite_dir != 0:
			_sprite_dir = 0
			animated_sprite.flip_h = false
			animated_sprite.play("Idle")
			
var collisionpos = Vector2.ZERO

func change_view(view: int) -> void:
	var tween = create_tween()
	var ship_rect : Rect2 = Rect2(ship_controlled.get_rect().position.x * ship_controlled.get_tile_size().x * 5, ship_controlled.get_rect().position.y * ship_controlled.get_tile_size().y * 5, ship_controlled.get_rect().size.x * ship_controlled.get_tile_size().x * 5, ship_controlled.get_rect().size.y * ship_controlled.get_tile_size().y * 5)
	var ship_center : Vector2 = ship_rect.size / 2 + ship_rect.position
	camera_difference = ship_center - position
	var duration = 1

	var ship_size = (max(ship_rect.size.x, ship_rect.size.y) + 2000) * 1.666
	var cam_size = get_viewport_rect().size.y * 1.25
	var ship_zoom = 1 / (ship_size / cam_size)
	
	match view:
		0: 
			tween.parallel().tween_property(camera, "zoom", Vector2(ship_zoom, ship_zoom), duration).set_ease(Tween.EASE_OUT)
			tween.parallel().tween_property(camera, "offset", camera_difference.rotated(global_rotation), duration).set_ease(Tween.EASE_OUT)
			tween.parallel().tween_property(vision, "texture_scale", driving_vision, duration).set_ease(Tween.EASE_OUT)
		1: 
			camera_difference = Vector2.ZERO
			tween.parallel().tween_property(camera, "zoom", Vector2(normal_zoom, normal_zoom), duration).set_ease(Tween.EASE_OUT)
			tween.parallel().tween_property(camera, "offset", camera_difference, duration).set_ease(Tween.EASE_OUT)
			tween.parallel().tween_property(vision, "texture_scale", normal_vision, duration).set_ease(Tween.EASE_OUT)

func _on_pickup_area_entered(area:Area2D) -> void:
	if !area.is_in_group("CharacterInteractArea"):
		area.get_parent().can_pickup = true

func _on_pickup_area_exited(area:Area2D) -> void:
	if !area.is_in_group("CharacterInteractArea"):
		area.get_parent().can_pickup = false

func deleting_ship(_ship : Ship):
	if _ship == parent_ship:
		call_deferred("reparent", World.instance)

func _on_health_updated_signal() -> void:
	UIManager.instance.player_health_updated_signal()


func _on_respawn_timer_timeout() -> void:
	World.save_file.load_world()

func _on_invincibility_timer_timeout() -> void:
	invincible = false


func _on_lock_rotation_timer_timeout() -> void:
	_locked_rotating = false
