extends Node2D

@onready var main = get_tree().get_root()
@onready var projectile_scene = load("res://Ship/Walls/Canon/Projectile.tscn")

func _ready() -> void:
	pass

func _physics_process(_delta: float) -> void:
	rotation_degrees += 1

func shoot():
	var instance = projectile_scene.instantiate()
	instance.dir = global_rotation
	instance.spawn_position = global_position
	instance.spawn_rotation = deg_to_rad(global_rotation_degrees - 90)
	main.add_child.call_deferred(instance)

func _input(event: InputEvent) -> void:
	if event is InputEventMouseButton:
		shoot()