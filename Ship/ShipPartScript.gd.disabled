class_name ShipPart extends Node2D

var _hitbox

var durability_max: float: 
	get: return durability_max
	set(value):
		durability_max = value
		durability_current = min(durability_current, durability_max)

var durability_current : float :
	get: return durability_current if durability_max >= 0 else 6942069.
	set(value): durability_current = min(value, durability_max)


var mass : float

var ship : Ship

var tilemap_coords : Vector2i


func init(_ship, _coords : Vector2i, _durability : float = 60, _mass : float = 1):
	ship = _ship
	tilemap_coords = _coords
	durability_max = _durability
	durability_current = _durability
	mass = _mass

	ship.mass += mass

	_reset_hitbox.call_deferred()

func destroy():
	remove()

func remove() -> void:
	ship.mass -= mass
	queue_free()

func _reset_hitbox():
	_hitbox = get_node_or_null("Hitbox")
	if (_hitbox != null): _hitbox.position = Vector2.ZERO
