class_name Inventory extends Node2D

@onready var grid := $GridContainer

static var currency_node
static var currency_value
static var add_currency_label
static var remove_currency_label
static var currency_timer

const slot_scene := preload("res://Editor/Inventory/Slot.tscn")

static var currency = 0

static func add_currency(amount : int, visual := true) -> bool:
	if amount == 0: return false
	if currency + amount < 0: 
		if visual: Inventory.currency_change_effect(0)
		return false
	if visual: Inventory.currency_change_effect(amount)
	currency += amount
	currency_value.text = str(currency)
	return true

func _ready():
	currency_node = $Currency
	currency_value = $Currency/Value
	add_currency_label = $Currency/AddCurrencyLabel
	remove_currency_label = $Currency/RemoveCurrencyLabel  
	currency_timer = $Currency/Timer  
	currency_value.text = str(currency)
	
	currency = Player.main_player.currency
	currency_value.text = str(currency)

func load_grid():
	for key in ShipEditor.tools.keys():
		if ShipEditor.tools[key].debug && !Options.DEVELOPMENT_MODE: continue
		var new_slot = slot_scene.instantiate()
		grid.add_child(new_slot)
		new_slot.set_tool(ShipEditor.tools[key])

static func currency_change_effect(amount : int):
	if amount == 0:
		currency_timer.start(1)
		currency_value.set("theme_override_colors/font_color", Color.RED)
		await currency_timer.timeout
		currency_value.set("theme_override_colors/font_color", Color.WHITE)
		return

	var label
	if amount > 0:
		label = add_currency_label.duplicate()
		label.text = "+" + str(amount)	
	elif amount < 0:
		label = remove_currency_label.duplicate()
		label.text = str(amount)	
	label.visible = true
	currency_node.add_child(label)
	var _start_position = label.position
	var tween = label.create_tween()
	var duration = 1
	label.modulate = Color.WHITE
	tween.parallel().tween_property(label, "position", _start_position + Vector2(0, -50), duration)
	tween.parallel().tween_property(label, "modulate", Color(1, 1, 1, 0), duration).set_ease(Tween.EASE_IN)
	await tween.finished
	label.queue_free()
