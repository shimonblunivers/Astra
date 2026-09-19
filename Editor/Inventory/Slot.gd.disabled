class_name Slot extends Control

@onready var texture_rect := $Panel/TextureRect
@onready var price_label := $Price
@onready var nickname_label := $Nickname

var tool : Tool

func set_tool(_tool : Tool):
	tool = _tool
	texture_rect.texture = tool.texture
	nickname_label.text = tool.nickname
	price_label.text = str(tool.price)

func _on_panel_gui_input(event: InputEvent) -> void:
	if event is InputEventMouseButton && event.button_mask == 1:
		ShipEditor.change_tool(tool.name)