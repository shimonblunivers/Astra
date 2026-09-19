extends Node

@export var DEVELOPMENT_MODE : bool = true if OS.has_feature("editor") else false
@export var FULLSCREEN : bool = false


func _unhandled_input(event: InputEvent):
	if event.is_action_pressed("toggle_fullscreen"):
		FULLSCREEN = !FULLSCREEN
		if FULLSCREEN:
			DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_EXCLUSIVE_FULLSCREEN)
		else:
			DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_WINDOWED)
