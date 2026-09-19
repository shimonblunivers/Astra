class_name Character extends CharacterBody2D


@onready var speed = Vector2.ZERO

@onready var legs = $LegHitbox

@export var godmode = false

const SPEED = 500.0
const RUN_SPEED_MODIFIER = 0.0
const TURN_SPEED = 1.0

var legs_offset = Vector2.ZERO

var max_health : float = 100
var health : = max_health
signal health_updated_signal
signal died_signal
var alive : bool = false
var spawned : bool = false

var spawn_point : Vector2 = Vector2.ZERO

var nickname = ""

var max_impact_velocity : float = 40

func _ready() -> void:
	legs_offset = legs.position


func _physics_process(delta: float) -> void:
	if (global_position - Player.main_player.global_position).length() > Player.main_player.update_range: return
	_in_physics(delta)
	
func _in_physics(_delta: float) -> void:
	pass


func set_health(amount : float):
	health = amount
	damage(0)

func damage(amount : float):
	if godmode: return
	health = max(health - amount, 0)
	if health == 0: kill()
	else: health_updated_signal.emit()


func kill():
	if !alive: return
	health = 0
	alive = false

	health_updated_signal.emit()
	died_signal.emit()
	
	
func spawn():
	alive = true
	spawned = true
	health = max_health
	position = spawn_point
	health_updated_signal.emit()
