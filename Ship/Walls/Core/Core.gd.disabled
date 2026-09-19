class_name Core extends InteractableShipPart

@onready var sprite : AnimatedSprite2D = $Sprite2D

var layer : int = 0

func init(_ship, _coords : Vector2i, _durability : float = 200, _mass : float = 5):
	super(_ship, _coords, _durability, _mass)

func _interact():
	World.save_file.save_world(false)

func _on_area_2d_area_entered(area:Area2D) -> void:
	if area.is_in_group("PlayerArea"):
		player_in_range = area.get_owner()
		player_in_range.hovering_controllables.append(self)

func _on_area_2d_area_exited(area:Area2D) -> void:
	if area.is_in_group("PlayerArea"):
		player_in_range.hovering_controllables.erase(self)
		player_in_range = null
