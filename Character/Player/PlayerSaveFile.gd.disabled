class_name PlayerSaveFile extends Resource

@export var position : Vector2
@export var rotation : float
@export var acceleration : Vector2
@export var currency : int
@export var suit : bool
@export var health : int
@export var owned_ship_id : int

@export var highlighted_quest_id : int

static func save() -> PlayerSaveFile:
	var file = PlayerSaveFile.new()

	file.position = World.instance.get_distance_from_center(Player.main_player.global_position)
	file.rotation = Player.main_player.global_rotation
	file.acceleration = Player.main_player.acceleration
	
	file.health = Player.main_player.health
	file.currency = Player.main_player.currency
	file.suit = Player.main_player.suit

	file.owned_ship_id = Player.main_player.owned_ship.id

	file.highlighted_quest_id = QuestManager.highlighted_quest_id

	return file

func load():

	var player = Player.main_player
	player.spawn(position, acceleration, rotation)
	 
	player.set_health(health)
	player.suit = suit
	player.currency = currency
	player.currency_updated_signal.emit()

	player.owned_ship = Ship.get_ship(owned_ship_id)

	QuestManager.highlighted_quest_id = highlighted_quest_id
