class_name SaveFile extends Resource

const SAVE_GAME_PATH := "user://saves/worlds/"
const FIRST_SAVE_GAME_PATH := "res://DefaultSave/worlds/"

@export var player_save_file : PlayerSaveFile
@export var main_station_id : int

@export var NPC_save_files = []
@export var item_save_files = []
@export var quest_save_files = []
@export var task_save_files = []
@export var ship_save_files = []

static var save_name : String = "last_save"

func initialize_files():
	DirAccess.make_dir_recursive_absolute("user://saves/ships")
	DirAccess.make_dir_recursive_absolute("user://saves/worlds")

static func get_save_path(path := SAVE_GAME_PATH + save_name) -> String:
	return path + "/save_file.tres"

func save_world(dev := false):
	
	print_debug("Saving...")
	
	UIManager.instance.saving_screen()

	player_save_file = PlayerSaveFile.save()
	main_station_id = ShipManager.main_station.id
	ship_save_files = ShipSaveFile.save()
	NPC_save_files = NPCSaveFile.save()
	item_save_files = ItemSaveFile.save()
	quest_save_files = QuestSaveFile.save()
	task_save_files = TaskSaveFile.save()

	if DirAccess.dir_exists_absolute("user://saves/ships/%player_ship_new"):
		if DirAccess.dir_exists_absolute("user://saves/ships/%player_ship_old"): delete_directory("user://saves/ships/%player_ship_old")
		DirAccess.rename_absolute("user://saves/ships/%player_ship_new", "user://saves/ships/%player_ship_old")
		if DirAccess.dir_exists_absolute("user://saves/ships/%player_ship_old"): Player.main_player.owned_ship.path = "%player_ship_old"
	if !dev:
		DirAccess.make_dir_recursive_absolute("user://saves/worlds/" + save_name + "/")
		return ResourceSaver.save(self, SaveFile.get_save_path())
	else:
		return ResourceSaver.save(self, SaveFile.get_save_path(FIRST_SAVE_GAME_PATH))

func load_world():
	if FileAccess.file_exists(SaveFile.get_save_path()):
		World.save_file = ResourceLoader.load(SaveFile.get_save_path())
		World.save_file.call_deferred("_load")
	else:
		World.instance.new_world()
		# World.save_file = ResourceLoader.load(get_save_path(FIRST_SAVE_GAME_PATH))
		# World.save_file.call_deferred("_load")

	
func _load(): # deferred
	World.reset_values()
	
	print_debug("Loading...")
	
	for ship in ship_save_files: ship.load(NPC_save_files, item_save_files) 
	ShipManager.main_station = Ship.get_ship(main_station_id)
	ShipManager.main_station.freeze = true

	player_save_file.load()
	Player.main_player.owned_ship.linear_damp = 0

	for quest : QuestSaveFile in quest_save_files: quest.load() 
	for task : TaskSaveFile in task_save_files: task.load()
	
func delete_directory(path: String) -> bool:
	var dir = DirAccess.open(path)
	if dir:
		dir.list_dir_begin()
		var file_name = dir.get_next()
		while file_name != "":
			if dir.current_is_dir():
				if !delete_directory(path + "/" + file_name): return false
			else:
				dir.remove(path + "/" + file_name)
			file_name = dir.get_next()
		dir.remove(path)
		return true
	else: return false
