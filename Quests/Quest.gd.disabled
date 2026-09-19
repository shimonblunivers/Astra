## Instance of a task.
class_name Quest

var task : Task

var id : int

var npc_id : int = -1
var target_id : int = -1
var target_type : Goal.Type

var status : int = 0

static var default_outline_color = Color.BLACK
static var active_quest_outline_color = Color.DARK_SEA_GREEN
static var objective_of_quest_outline_color = Color.DARK_GOLDENROD
static var talking_about_quest_outline_color = Color.MEDIUM_PURPLE

func get_npc() -> NPC:
	if (npc_id == -1): return null
	var npc =  NPC.get_npc(npc_id)
	if (is_instance_valid(npc)): return npc
	
	print_debug("Warning: NPC with ID " + str(npc_id) + " not found")
	return null

func get_target() -> Node2D:
	if (target_id == -1): return null
	var target : Node2D = null

	match target_type:
		Goal.Type.go_to_place:
			target = null
		Goal.Type.talk_to_npc:
			target = NPC.get_npc(target_id)
		Goal.Type.pick_up_item:
			target = Item.get_item(target_id)

	if (is_instance_valid(target)): return target

	print_debug("Warning: Target with ID " + str(target_id) + " and type " + str(target_type) + " not found")
	return null

func _init(_task_id: int, _npc : NPC, _target_id : int = -1, _id : int = -1):
	self.task = QuestManager.get_task(_task_id)
	self.npc_id = _npc.id

	if _id == -1: id = QuestManager.get_uid()
	else: id = _id
	
	QuestManager.active_quests[id] = self

	QuestManager.quest_id_history.append(id)

	QuestManager.active_task_ids.append(_task_id)

	target_type = task.goal.type

	task.times_activated += 1

	if _target_id != -1: target_id = _target_id
	else: spawn_quest_ship()

	if !(task.add_role_on_accept in _npc.roles): _npc.roles.append(task.add_role_on_accept)

	_npc.active_quest_id = id
	_npc.selected_quest_id = -1

	QuestManager.highlighted_quest_id = id
	
	QuestManager.update_quest_log()

func finish():
	Player.main_player.add_currency(task.reward)

	World.difficulty_multiplier += 0.2 # Increase difficulty for the next quests

	target_id = -1
	get_npc().quest_finished()

	delete()

func delete():
	if QuestManager.highlighted_quest_id == id: 
		QuestManager.highlighted_quest_id = -1
	
	get_npc().active_quest_id = -1
	get_npc().selected_quest_id = -1
	
	QuestManager.active_task_ids.erase(task.id)
	QuestManager.active_quests.erase(id)

	QuestManager.update_quest_log()

func progress():
	status += 1
	
	if !task.return_to_npc && status == 1 || task.return_to_npc && status > 1: finish() 
	else:
		target_id = npc_id
		target_type = Goal.Type.talk_to_npc
		get_npc().update_nametag_color()

func spawn_quest_ship():
	var distances = Vector2(50000 + 10000 * World.difficulty_multiplier * task.difficulty_multiplier, 200000 + 10000 * World.difficulty_multiplier * (task.difficulty_multiplier + 1)) #Vector2(10000, 50000)

	var rng = RandomNumberGenerator.new()

	var _distance = rng.randf_range(distances.x, distances.y)
	var _angle = rng.randf_range(0, 2 * PI)

	var new_ship_pos = Vector2(Player.main_player.global_position.x + _distance * cos(_angle), Player.main_player.global_position.y + _distance * sin(_angle))
	var npc_presets := [];
	var item_presets := [];

	match target_type:
		Goal.Type.go_to_place: # TODO
			pass
		Goal.Type.talk_to_npc:
			target_id = NPC.get_uid()
			npc_presets = [NPCPreset.new(target_id, NPC.names.pick_random(), [NPC.Roles.CIVILIAN])]
		Goal.Type.pick_up_item:
			target_id = Item.get_uid()
			item_presets = [ItemPreset.new(target_id, Item.types[task.goal.item_type], 0)]

	var _custom_object_spawn = CustomObjectSpawn.create(npc_presets, item_presets)

	ShipManager.spawn_ship(new_ship_pos, ShipManager.get_quest_ship_path(task.id), _custom_object_spawn)#.linear_velocity = Player.main_player.parent_ship.linear_velocity
