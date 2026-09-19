class_name InteractableShipPart extends ShipPart

var direction := "horizontal"

var player_in_range = null

var hitboxes_to_shift = []

var button_indicator : ButtonIndicator
const scene_button_indicator = preload("res://UI/ButtonIndicator.tscn")

var button_indicator_visible = false

func init(_ship, _coords : Vector2i, _durability : float = 10, _mass : float = 1):
	super(_ship, _coords, _durability, _mass)
	button_indicator = scene_button_indicator.instantiate()
	call_deferred("add_child", button_indicator)
	button_indicator.init(self)
	button_indicator.visible = button_indicator_visible

func _physics_process(_delta: float) -> void:
	if button_indicator_visible == (player_in_range == null):
		button_indicator_visible = !player_in_range == null
		button_indicator.visible = button_indicator_visible

func _process(_delta):
	for hitbox in hitboxes_to_shift:
		hitbox.position = (- ship.difference_in_position).rotated(-global_rotation)
		# if direction == "horizontal":
		# 	hitbox.position = (- ship.difference_in_position).rotated(-global_rotation)
		# elif direction == "vertical":
		# 	hitbox.position = (- ship.difference_in_position).rotated(-global_rotation)

func interact():
	_interact()

func _interact(): # VIRTUAL METHOD
	pass

