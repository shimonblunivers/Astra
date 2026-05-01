class_name NPC extends Character

var difference = Vector2.ZERO

@onready var dialog_manager := $DialogManager
@onready var sprites := $Sprite
@onready var timer := $Timer


var ship: Ship

var roles = []

enum Roles {
	CIVILIAN,
	NONE,
	TRUSTED,
	CAPTAIN,
	ADDICTED,
}

static var names = [
	"Kevin",
	"Lukáš",
	"Tomáš",
	"Jan",
	"Pavel",
	"Martin",
	"Jakub",
	"Michal",
	"Jiří",
	"Adam",
	"David",
	"Marek",
	"Petr",
	"Ondřej",
	"Filip",
	"Richard",
	"Robert",
	"Václav",
	"Matěj",
	"Aleš",
	"Daniel",
	"Josef",
	"Karel",
	"Vojtěch",
	"František",
	"Eduard",
	"Viktor",
	"Igor",
	"Radim",
	"Radek",
	"Lukas",
	"Dominik",
	"Jakub",
	"Rudolf",
	"Lukáš",
	"Emanuel",
	"Štěpán",
	"Jaroslav",
	"Michael",
	"Zdeněk",
	"Aleš",
	"Patrik",
	"Tom",
	"Albert",
	"Viktor",
	"Jiří",
	"Denis",
	"Pavel",
	"Igor",
	"Eduard",
	"Jan",
	"Luboš",
	"Šimon",
	"Teo",
	"Honza",
	"Vašek",
]

static var npcs = []

var interactable = false

var hovering = false

var active_quest_id: int = -1:
	set(value):
		if value == -1:
			reload_missions()
		else:
			for npc in NPC.npcs:
				if npc.selected_quest_id == value && npc != self:
					npc.reload_missions()
		active_quest_id = value
		update_nametag_color()

var selected_quest_id = -1: # IS TALKING ABOUT QUEST?
	set(value):
		if (active_quest_id != -1 || QuestManager.is_objective(self )):
			selected_quest_id = -1
		else:
			selected_quest_id = value
		update_nametag_color()
 
var id: int

# TODO: ✅ Add dialog

# TODO: ✅ Add missions

static func get_uid() -> int:
	var _id = 0
	while true:
		if NPC.get_npc(_id) == null:
			return _id
		_id += 1
	return 0

static func get_npc(_id: int) -> NPC:
	for npc in npcs: if npc.id == _id: return npc
	return null

var skin = null
var hair = null

func init(_id: int = -1, _nickname: String = names.pick_random(), _roles := [Roles.CIVILIAN], _skin = null, _hair = null):
	roles = _roles
	skin = _skin
	hair = _hair
	
	if _id != -1 && NPC.get_npc(_id) == null:
		id = _id
	else:
		id = NPC.get_uid()
		
	if id == 0 && _skin == null:
		_nickname = "Captain " + _nickname
		roles.append(Roles.CAPTAIN)
	nickname = _nickname
	
	if OS.is_debug_build(): nickname += " " + str(id)
	
	$Nametag.text = nickname
	name = "NPC_" + nickname + "_" + str(id)

	npcs.append(self )
	

func update_nametag_color():
	if QuestManager.is_objective(self ):
		$Nametag.add_theme_color_override("font_outline_color", Quest.objective_of_quest_outline_color)
		return
	if active_quest_id > -1:
		$Nametag.add_theme_color_override("font_outline_color", Quest.active_quest_outline_color)
		return
	if selected_quest_id > -1:
		$Nametag.add_theme_color_override("font_outline_color", Quest.talking_about_quest_outline_color)
		return
	$Nametag.add_theme_color_override("font_outline_color", Quest.default_outline_color)


func quest_finished():
	active_quest_id = -1
	$FinishedQuest.play()

## Updates [member selected_quest_id] with a new mission. [br]
## [forced]: the mission will be updated even if the NPC is currently talking about a quest. [br]
func reload_missions(forced := false):
	if !forced && active_quest_id != -1: return

	var mission = Dialogs.random_task_id(roles, true)

	if mission < 0:
		selected_quest_id = -1
	else:
		selected_quest_id = mission
		
	var random := RandomNumberGenerator.new()
	timer.start(120 + 60 * random.randi_range(0, 8))

	update_nametag_color()

func _ready() -> void:
	legs_offset = legs.position

	if skin != null:
		sprites.set_skin(skin[0], skin[1], skin[2], skin[3], skin[4])

	if hair != null:
		sprites.hair_node.frame = hair[0]
		sprites.hair_node.flip_h = hair[1]

	skin = sprites.skin
	reload_missions()
	

func _in_physics(_delta):
	$Area.position = Vector2(0, -42.5) + (-ship.difference_in_position).rotated(-global_rotation)
 
func _on_interaction_area_area_entered(area: Area2D) -> void:
	if area.is_in_group("PlayerInteractArea"):
		interactable = true

func _on_interaction_area_area_exited(area: Area2D):
	if area.is_in_group("PlayerInteractArea"):
		interactable = false
		dialog_manager.end_dialog()

func _on_area_mouse_entered() -> void:
	hovering = true
func _on_area_mouse_exited() -> void:
	hovering = false

func _process(_delta):
	$Nametag.visible = hovering && interactable

func _on_area_input_event(_viewport: Node, event: InputEvent, _shape_idx: int) -> void:
	if event is InputEventMouseButton && event.button_mask == 1:
		if interactable:
			if dialog_manager.is_dialog_active:
				dialog_manager.advance()
			else:
				var dialog_position = Vector2(0, -105)

				if QuestManager.is_objective(self ):
					if QuestManager.get_quest_by_target(self ).task.id in Dialogs.conversations["mission_finished"].keys():
						dialog_manager.start_dialog(dialog_position, Dialogs.conversations["mission_finished"][QuestManager.get_quest_by_target(self ).task.id])
					else:
						dialog_manager.start_dialog(dialog_position, Dialogs.conversations["mission_finished"][-1])

					QuestManager.finished_quest_objective(QuestManager.get_quest_by_target(self ))

				elif selected_quest_id >= 0:
					if selected_quest_id in QuestManager.active_task_ids:
						print_debug("Warning: Task " + str(selected_quest_id) + " is selected, but already active.")
					else:
						dialog_manager.start_dialog(dialog_position, Dialogs.conversations["mission"][selected_quest_id])
				elif selected_quest_id == -1:
					dialog_manager.start_dialog(dialog_position, [Dialogs.conversations["greeting"].pick_random()])
				
func delete():
	npcs.erase(self )
	queue_free()

func _on_timer_timeout() -> void:
	reload_missions()
