class_name QuestSaveFile extends Resource

@export var task_id : int

@export var npc_id : int

@export var target_id : int
@export var target_type : int

@export var status : int

@export var id : int

static func save():
	var files = []

	for quest_key in QuestManager.active_quests.keys():
		var quest = QuestManager.active_quests[quest_key]
		var file = QuestSaveFile.new()

		file.task_id = quest.task.id

		file.npc_id = quest.npc_id
		file.target_id = quest.target_id
		file.target_type = quest.target_type
		file.status = quest.status
		files.append(file)

	return files

func load():
	var quest = Quest.new(task_id, NPC.get_npc(npc_id), target_id, id)

	quest.task.times_activated -= 1

	quest.status = status
	quest.target_type = target_type
	
	QuestManager.update_quest_log()
	




