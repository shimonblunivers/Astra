class_name EditorCamera extends Camera2D

const SPEED : float = 400
const RUN_SPEED_MODIFIER : float = 800

const ZOOM_SPEED : float = 0.1
const MAX_ZOOM : float = 10
const MIN_ZOOM : float = 0.2

const LEFT_LIMIT = -350

var locked := false

func _physics_process(delta):
	if locked: return
	var velocity := Vector2()
	var direction := Input.get_vector("ui_left", "ui_right", "ui_up", "ui_down")
	var running = Input.get_action_strength("game_run")

	if direction != Vector2.ZERO:
		velocity.x = direction.x * (SPEED + RUN_SPEED_MODIFIER * running) * (1 / zoom.x)
		velocity.y = direction.y * (SPEED + RUN_SPEED_MODIFIER * running) * (1 / zoom.y)

		if position.x + (float)(velocity.x * delta) < LEFT_LIMIT && !Options.DEVELOPMENT_MODE: velocity.x = 0

		position += Vector2((float)(velocity.x * delta), (float)(velocity.y * delta))

func _unhandled_input(event: InputEvent) -> void:
	if locked: return
	if event is InputEventMouseButton && event.is_pressed():
		var zoom_modifier : float = 0

		if event.button_index == MOUSE_BUTTON_WHEEL_DOWN:
			if zoom.x > 1: zoom_modifier = -1
			else: zoom_modifier = -0.25

		if event.button_index == MOUSE_BUTTON_WHEEL_UP:
			zoom_modifier = 1
		var difference = zoom_modifier * ZOOM_SPEED
		var new_value = clampf(difference + zoom.x, MIN_ZOOM, MAX_ZOOM)
		zoom = Vector2(new_value, new_value)



