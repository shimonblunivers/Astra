extends CanvasLayer

@onready var sound_slider = $MarginContainer/VBoxContainer/SoundSlider
@onready var soundtrack_slider = $MarginContainer/VBoxContainer/SoundtrackSlider

@onready var sfx_sound_text = $SFXTestSound

func _ready():
	sound_slider.value = (AudioServer.get_bus_volume_db(AudioServer.get_bus_index("SFX")) + 30) / 36 * 100 
	soundtrack_slider.value = (AudioServer.get_bus_volume_db(AudioServer.get_bus_index("Music")) + 30) / 36 * 100


func _on_sound_slider_drag_ended(value:float) -> void:
	var _value = -30 + 36 * sound_slider.value / 100
	AudioServer.set_bus_volume_db(AudioServer.get_bus_index("SFX"), _value)

	if _value <= -29:	
		AudioServer.set_bus_mute(AudioServer.get_bus_index("SFX"), true)
	else:
		AudioServer.set_bus_mute(AudioServer.get_bus_index("SFX"), false)

	sfx_sound_text.play()

func _on_soundtrack_slider_value_changed(value:float) -> void:
	var _value = -30 + 36 * value / 100

	AudioServer.set_bus_volume_db(AudioServer.get_bus_index("Music"), _value)
	if _value <= -29:
		AudioServer.set_bus_mute(AudioServer.get_bus_index("Music"), true)
	else:
		AudioServer.set_bus_mute(AudioServer.get_bus_index("Music"), false)

func _on_back_pressed():
	Menu.instance.visible = true
	self.queue_free()




