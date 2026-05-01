## Global singleton that manages all quests in the game.
extends Node

## Dictionary of all tasks (possible quests), identified by their ID.
var tasks: Dictionary = {}

## Dictionary of quests that are currently active, identified by their ID.
var active_quests: Dictionary = {}
var active_task_ids: Array = []

## List of all quest IDs that have been used in this save.
var quest_id_history: Array = []

var highlighted_quest_id := -1
var highlight_main_station = false

func get_uid() -> int:
	var _id = quest_id_history.size()
	while true:
		if !(_id in quest_id_history):
			return _id
		_id += 1
	return 0

func _ready():
	var dir = DirAccess.open("res://Quests/Tasks")
	if dir:
		dir.list_dir_begin()
		var file_name = dir.get_next()
		while file_name != "":
			if !dir.current_is_dir():
				if '.tres.remap' in file_name:
					file_name = file_name.trim_suffix('.remap')
				if ".tres" in file_name:
					var task: Task = load("res://Quests/Tasks/" + file_name)
					if task:
						print("Loaded task: " + task.title)
						tasks[task.id] = task
					else: printerr("Error: Failed to load quest resource " + file_name)
			file_name = dir.get_next()
		dir.list_dir_end()

func _process(_delta):
	if UIManager.instance:
		var show_arrow = active_quests.size() > 0 && highlighted_quest_id != -1 || highlight_main_station
		UIManager.instance.quest_arrow.visible = show_arrow
		UIManager.instance.quest_arrow_distance_label.visible = show_arrow
		if show_arrow:
			var distance := 0.0
			if highlight_main_station: distance = (ShipManager.main_station.global_position - Player.main_player.global_position).length()
			elif highlighted_quest_id != -1:
				if get_quest(highlighted_quest_id):
					if get_quest(highlighted_quest_id).get_target(): distance = (get_quest(highlighted_quest_id).get_target().global_position - Player.main_player.global_position).length()
				else:
					print_debug("Warning: Quest with ID " + str(highlighted_quest_id) + " not found.")
			var minimal_range = 150
			var maximal_range = 250

			if distance < 999999 && distance > maximal_range:
				UIManager.instance.quest_arrow_distance_label.rotation = - UIManager.instance.quest_arrow.rotation
				UIManager.instance.quest_arrow_distance_label.text = str(round(distance / 100)) + "m"
			else:
				UIManager.instance.quest_arrow_distance_label.text = ""

			if distance < minimal_range:
				UIManager.instance.quest_arrow.visible = false
				UIManager.instance.quest_arrow.modulate.a = 0.75
			else:
				UIManager.instance.quest_arrow.visible = true
				var normalized_distance = clamp((distance - minimal_range) / (maximal_range - minimal_range), 0, 0.75)
				UIManager.instance.quest_arrow.modulate.a = normalized_distance
			if highlight_main_station: UIManager.instance.quest_arrow.rotation = (ShipManager.main_station.global_position - Player.main_player.global_position).angle() - Player.main_player.global_rotation
			elif highlighted_quest_id != -1:
				if get_quest(highlighted_quest_id):
					if get_quest(highlighted_quest_id).get_target(): UIManager.instance.quest_arrow.rotation = (get_quest(highlighted_quest_id).get_target().global_position - Player.main_player.global_position).angle() - Player.main_player.global_rotation
				else:
					print_debug("Warning: Quest with ID " + str(highlighted_quest_id) + " not found.")
					  
func update_quest_log():
	var string_to_add = ""
	for quest_key in active_quests.keys():
		var quest = active_quests[quest_key]
		if quest.id == highlighted_quest_id: string_to_add += "[u]"
		string_to_add += "\n[url=" + str(quest.id) + "][b]" + quest.task.title
		if quest.status > 0: string_to_add += " [" + str(quest.status) + "/2]"
		string_to_add += "[/b][/url]"
		if quest.id == highlighted_quest_id:
			string_to_add += "[/u] [url=cancel" + str(quest.id) + "](X)[/url]"
			string_to_add += "\n" + quest.task.description
	UIManager.quest_label.text = string_to_add

	string_to_add = "[center][b]"
	if highlight_main_station: string_to_add += "[u]"
	string_to_add += "[url=main_ship]Main Station"
	UIManager.main_station_label.text = string_to_add

func finished_quest_objective(quest: Quest):
	quest.progress()
	QuestManager.update_quest_log()

func get_task(id: int) -> Task:
	if tasks.has(id): return tasks[id]
	print_debug("Warning: No task found with id " + str(id))
	return null

func get_quest(id: int) -> Quest:
	if active_quests.has(id): return active_quests[id]
	print_debug("Warning: No quest found with id " + str(id))
	return null

func get_quest_by_target(target: Node2D) -> Quest:
	for quest_key in active_quests.keys():
		var quest = active_quests[quest_key]
		if quest.get_target() == target: return quest
	return null

func is_objective(target: Node2D) -> bool:
	for quest_key in active_quests.keys():
		var quest = active_quests[quest_key]
		if quest.get_target() == target: return true
	return false
