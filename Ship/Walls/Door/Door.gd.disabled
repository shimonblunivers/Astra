class_name Door extends InteractableShipPart


@onready var walkway: StaticBody2D = $Hitbox/StaticBody2DWalkway
@onready var open_sound: AudioStreamPlayer2D = $Sound/DoorOpen
@onready var close_sound: AudioStreamPlayer2D = $Sound/DoorClose
@onready var animated_sprite: AnimatedSprite2D = $AnimatedSprite2D

@onready var mouse_hitbox: Node2D = $Hitbox/Area/MouseHitbox
@onready var door_area: Node2D = $Hitbox/Area/Area2D

var state := "closed"

var obstructed := false
var locked := false

var collision_layer = 1
var occluder_light_mask = 1

var interact_range = 300

var is_operating = false

func init(_ship, _coords: Vector2i, _durability: float = 100, _mass: float = 3):
	_ship.interactables.append(self )
	super (_ship, _coords, _durability, _mass)

func _ready() -> void:
	if direction == "vertical":
		rotation_degrees = 90
	animated_sprite.connect("frame_changed", _on_frame_changed)
	hitboxes_to_shift.append(mouse_hitbox)
	
func update_sprites():
	if state == "open":
		animated_sprite.play("open")
	else:
		animated_sprite.play_backwards("open")

func open():
	if locked:
		return
	is_operating = true
	state = "open"
	open_sound.pitch_scale = randf_range(0.9, 1.1)
	open_sound.play()
	update_sprites()
	ship.opened_doors.append(tilemap_coords)

	$AutocloseTimer.start()
	
func close():
	is_operating = true
	state = "closed"
	close_sound.pitch_scale = randf_range(0.9, 1.1)
	close_sound.play()
	update_sprites()
	walkway.set_collision_layer_value(collision_layer, true)
	ship.opened_doors.erase(tilemap_coords)

func _on_frame_changed():
	match animated_sprite.frame:
		3: # OPEN
			is_operating = state != "open"
			
			$"Hitbox/AnimatedOccluders/0left".occluder_light_mask = 0
			$"Hitbox/AnimatedOccluders/0right".occluder_light_mask = 0
			$"Hitbox/AnimatedOccluders/1left".occluder_light_mask = 0
			$"Hitbox/AnimatedOccluders/1right".occluder_light_mask = 0
			$"Hitbox/AnimatedOccluders/2center".occluder_light_mask = 0

			$"Hitbox/AnimatedHitbox/0left".set_collision_layer_value(collision_layer, false)
			$"Hitbox/AnimatedHitbox/0right".set_collision_layer_value(collision_layer, false)
			$"Hitbox/AnimatedHitbox/1left".set_collision_layer_value(collision_layer, false)
			$"Hitbox/AnimatedHitbox/1right".set_collision_layer_value(collision_layer, false)
		2:
			$"Hitbox/AnimatedOccluders/0left".occluder_light_mask = occluder_light_mask
			$"Hitbox/AnimatedOccluders/0right".occluder_light_mask = occluder_light_mask
			$"Hitbox/AnimatedOccluders/1left".occluder_light_mask = 0
			$"Hitbox/AnimatedOccluders/1right".occluder_light_mask = 0
			$"Hitbox/AnimatedOccluders/2center".occluder_light_mask = 0
			if state == "open":
				walkway.set_collision_layer_value(collision_layer, false)
				$"Hitbox/AnimatedHitbox/0left".set_collision_layer_value(collision_layer, true)
				$"Hitbox/AnimatedHitbox/0right".set_collision_layer_value(collision_layer, true)
				$"Hitbox/AnimatedHitbox/1left".set_collision_layer_value(collision_layer, false)
				$"Hitbox/AnimatedHitbox/1right".set_collision_layer_value(collision_layer, false)
		1:
			$"Hitbox/AnimatedOccluders/0left".occluder_light_mask = occluder_light_mask
			$"Hitbox/AnimatedOccluders/0right".occluder_light_mask = occluder_light_mask
			$"Hitbox/AnimatedOccluders/1left".occluder_light_mask = occluder_light_mask
			$"Hitbox/AnimatedOccluders/1right".occluder_light_mask = occluder_light_mask
			$"Hitbox/AnimatedOccluders/2center".occluder_light_mask = 0
			if state == "open":
				$"Hitbox/AnimatedHitbox/0left".set_collision_layer_value(collision_layer, true)
				$"Hitbox/AnimatedHitbox/0right".set_collision_layer_value(collision_layer, true)
				$"Hitbox/AnimatedHitbox/1left".set_collision_layer_value(collision_layer, true)
				$"Hitbox/AnimatedHitbox/1right".set_collision_layer_value(collision_layer, true)
		0: # CLOSED
			$"Hitbox/AnimatedOccluders/0left".occluder_light_mask = occluder_light_mask
			$"Hitbox/AnimatedOccluders/0right".occluder_light_mask = occluder_light_mask
			$"Hitbox/AnimatedOccluders/1left".occluder_light_mask = occluder_light_mask
			$"Hitbox/AnimatedOccluders/1right".occluder_light_mask = occluder_light_mask
			$"Hitbox/AnimatedOccluders/2center".occluder_light_mask = occluder_light_mask
			is_operating = state != "closed"


func _interact():
	if Player.main_player.global_position.distance_to(global_position) > interact_range:
		return
	if is_operating:
		return

	# var tile = ship.get_tile(tilemap_coords + Vector2i(1, 0))
	# print(tile)
	# if tile != null:
	# 	tile.destroy()
	# ship.wall_tile_map.set_cell(0, tilemap_coords + Vector2i(1, 0), -1)

	if state == "open":
		if obstructed:
			return
		close()
	else:
		open()

var obstructers = []

func _on_area_2d_area_entered(area: Area2D) -> void:
	if (area.is_in_group("CharacterInteractArea")):
		obstructed = true
		obstructers.append(area)


func _on_area_2d_area_exited(area: Area2D) -> void:
	if (area.is_in_group("CharacterInteractArea")):
		obstructers.erase(area)
		if obstructers.is_empty(): obstructed = false


func _on_mouse_hitbox_input_event(_viewport: Node, event: InputEvent, _shape_idx: int):
	if event is InputEventMouseButton && event.button_mask == 1 && Player.main_player.alive:
		interact()


func _on_autoclose_timer_timeout() -> void:
	if state == "open":
		if !obstructed:
			close()
		else:
			$AutocloseTimer.start()
