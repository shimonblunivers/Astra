class_name Helm extends InteractableShipPart

var controlled : bool = false

func init(_ship, _coords : Vector2i, _durability : float = 10, _mass : float = 1):
	_coords = Vector2i(_coords.x / 4, _coords.y / 4)                                   # MAYBE DOESNT WORK!!
	super(_ship, _coords, _durability, _mass)

func _interact():
	if controlled:
		player_in_range.control_ship(null)
		player_in_range.controllables_in_use.erase(self)
	else:
		player_in_range.control_ship(ship)
		player_in_range.controllables_in_use.append(self)
	controlled = !controlled

func _on_area_area_entered(area: Area2D) -> void:
	if area.is_in_group("PlayerArea"):
		player_in_range = area.get_owner()
		player_in_range.hovering_controllables.append(self)

func _on_area_area_exited(area: Area2D) -> void:
	if area.is_in_group("PlayerArea"):
		player_in_range.hovering_controllables.erase(self)
		player_in_range = null
