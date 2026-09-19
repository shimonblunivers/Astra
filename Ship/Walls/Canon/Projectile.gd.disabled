extends CharacterBody2D

@export var SPEED = 500

var dir : float
var spawn_position : Vector2
var spawn_rotation : float
var damage := 10

func _ready():
	global_position = spawn_position
	global_rotation = spawn_rotation

func _physics_process(_delta: float) -> void:
	velocity = Vector2(0, -SPEED).rotated(dir)
	move_and_slide()


func _on_area_2d_body_entered(body:Node2D) -> void:
	if body.is_in_group("Wall"):
		body.get_parent().damage(damage)
		queue_free()
