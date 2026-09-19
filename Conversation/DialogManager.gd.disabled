## Global singleton that manages all dialogs in the game.
extends Node

@onready var text_box_scene = preload("res://Conversation/TextBox.tscn")
@onready var parent = get_parent()

var dialog_lines := []
var current_line_index = 0

var text_box
var text_box_position: Vector2

var is_dialog_active = false
var can_advance_line = false

signal dialog_finished()

## [param position]: the offset of the text box from the parent node. [br]
## [param lines]: the lines of the dialog. [br]
func start_dialog(position: Vector2, lines):
	if is_dialog_active: 
		print_debug("Warning: Dialog is already active")
		return

	dialog_lines = lines
	text_box_position = position
	_show_text_box()

	is_dialog_active = true

func _show_text_box():	
	if dialog_lines.is_empty(): 
		print_debug("Warning: Attempted to show text box with dialog_lines empty")
		return
	if typeof(dialog_lines[current_line_index]) == TYPE_INT:
		var task = QuestManager.get_task(dialog_lines[current_line_index])
		if task:
			Quest.new(task.id, parent, -1);
			advance()
			return
		else:
			dialog_lines = [" . . . "]
			current_line_index = 0
			print_debug("Warning: Task " + str(dialog_lines[current_line_index]) + " not found in QuestManager.tasks")

	text_box = text_box_scene.instantiate()
	text_box.finished_displaying.connect(_on_text_box_finished_displaying)
	add_child(text_box)
	text_box.position = text_box_position

	text_box.display_text(dialog_lines[current_line_index])
	
	can_advance_line = false


func _on_text_box_finished_displaying():
	can_advance_line = true

## Advances the dialog to the next line. [br]
func advance():
	if is_dialog_active:
		text_box.queue_free()

		current_line_index += 1

		if current_line_index >= dialog_lines.size():
			dialog_finished.emit()
			is_dialog_active = false
			current_line_index = 0
			return
		
		_show_text_box()
	else:
		print_debug("Warning: Attempted to advance dialog when dialog is not active")

func end_dialog():
	if !is_dialog_active: return
	text_box.queue_free()
	dialog_finished.emit()
	is_dialog_active = false
	current_line_index = 0
