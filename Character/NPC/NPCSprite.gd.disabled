class_name NPCSprite extends Node2D

@onready var skin_node = $Skin
@onready var eyes_node = $Eyes
@onready var hair_node = $Hair
@onready var torso_node = $Torso
@onready var legs_node = $Legs
@onready var boots_node = $Boots

var skin = []

func set_skin(a = _random_color(), b = _random_color(), c = _random_color(), d = _random_color(), e = _random_color()):
	skin_node.set_modulate(a)
	eyes_node.set_modulate(b)
	hair_node.set_modulate(c)
	torso_node.set_modulate(d)
	legs_node.set_modulate(e)
	boots_node.set_modulate(Color.BLACK)

	skin = [a, b, c, d, e]


func _ready():
	var random := RandomNumberGenerator.new()
	hair_node.frame = random.randi_range(0, 6)
	hair_node.flip_h = random.randi_range(0, 1) == 0
	set_skin()
	eyes_node.stop()
	$Timer.set_wait_time(random.randf_range(0, 2))
	$Timer.start()
	await $Timer.timeout
	eyes_node.play("default")

func _random_color() -> Color:
	var random := RandomNumberGenerator.new()
	return Color(random.randfn() * 0.75, random.randfn() * 0.75, random.randfn() * 0.75, 1)
