class_name TaskSaveFile extends Resource


@export var id : int
@export var times_activated : int

static func save():
	var files = []

	for task_key in QuestManager.tasks.keys():
		var task = QuestManager.tasks[task_key]
		var file = TaskSaveFile.new()

		file.id = task.id
		file.times_activated = task.times_activated

		files.append(file)

	return files

func load():
	if QuestManager.tasks.has(id):
		QuestManager.tasks[id].times_activated = times_activated
	else:
		print_debug("Warning: Task with ID " + str(id) + " not found")
	
