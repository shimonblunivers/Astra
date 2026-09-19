extends Node

static var world

var started_game = false

func get_closest_ship(from_global_pos : Vector2, ships = Ship.ships) -> Ship:
	if ships.size() == 0: return null
	var closest = ships[0]
	for ship in ships:
		if closest.get_closest_point(from_global_pos).distance_to(from_global_pos) > ship.get_closest_point(from_global_pos).distance_to(from_global_pos): 
			closest = ship
	return closest

func _ready() -> void:
	Item.load_items()
	world = World.instance

func get_saveable_items():
	var list = []
	list.append_array(Ship.ships)
	list.append_array(NPC.npcs)
	list.append_array(Item.items)
	return list
