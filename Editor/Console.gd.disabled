class_name Console extends RichTextLabel

@onready var timer : Timer = $ConsoleTimer

var text_timeout = 5

func print_out(string: String):
	modulate = Color.WHITE
	text += "\n" + string

	timer.start(text_timeout)

	await timer.timeout

	var tween = create_tween()
	
	tween.tween_property(self, "modulate", Color(1, 1, 1, 0), text_timeout / 2)

	tween.connect("finished", _clear)

func _clear():
	text = ""

