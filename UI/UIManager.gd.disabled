class_name UIManager extends CanvasLayer


@onready var health_label = $HUD/HealthBar/Value
@onready var currency_label = $HUD/Currency/Value

@onready var death_screen = $HUD/DeathScreen

@onready var _quest_label = $HUD/Inventory/QuestLog/RichTextLabel
@onready var _main_station_label = $HUD/Inventory/MainStationLabel

@onready var inventory = $HUD/Inventory

@onready var loading_screen_node = $HUD/LoadingScreen
@onready var loading_screen_timer: Timer = $HUD/LoadingScreen/Timer
@onready var loading_screen_bar: ProgressBar = $HUD/LoadingScreen/ProgressBar
@onready var loading_screen_background: Node2D = $HUD/LoadingScreen/Background

@onready var quest_arrow: AnimatedSprite2D = $HUD/QuestArrow/Arrow
@onready var quest_arrow_distance_label: Label = $HUD/QuestArrow/Arrow/Distance

@onready var saving_screen_node = $HUD/SavingScreen

static var quest_label
static var main_station_label
static var add_currency_label
static var remove_currency_label
static var currency_node

static var instance

var inventory_open = false
var inventory_positions = Vector2(0, -500) # open, closed
# DEBUG

@onready var floating = $Debug/Floating
@onready var player_position = $Debug/PlayerPosition

static func currency_change_effect(amount: int):
	var label
	if amount > 0:
		label = add_currency_label.duplicate()
		label.text = "+" + str(amount)
	elif amount < 0:
		label = remove_currency_label.duplicate()
		label.text = str(amount)
	else: return
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

	
func _ready():
	instance = self
	health_label.text = str(int(Player.main_player.health))
	currency_label.text = str(int(Player.main_player.currency))
	quest_label = _quest_label
	main_station_label = _main_station_label
	currency_node = $HUD/Currency
	add_currency_label = $HUD/Currency/AddCurrencyLabel
	remove_currency_label = $HUD/Currency/RemoveCurrencyLabel

	loading_screen_node.visible = !Options.DEVELOPMENT_MODE
	floating.visible = Options.DEVELOPMENT_MODE
	player_position.visible = Options.DEVELOPMENT_MODE

func player_health_updated_signal() -> void:
	health_label.text = str(int(Player.main_player.health))
	death_screen.visible = !Player.main_player.alive
	
func _on_player_currency_updated_signal() -> void:
	currency_label.text = str(int(Player.main_player.currency))


var _vfx_muted = false
static var loading_mute = false

func loading_screen(time: float = 1.6):
	if Options.DEVELOPMENT_MODE: return
	loading_screen_node.visible = true
	if AudioServer.is_bus_mute(AudioServer.get_bus_index("SFX")):
		_vfx_muted = true
	else:
		_vfx_muted = false
		loading_mute = true
	loading_screen_node.visible = true
	loading_screen_node.modulate = Color.WHITE
	loading_screen_timer.start(time)

	await loading_screen_timer.timeout
	
	var tween = create_tween()
	tween.tween_property(loading_screen_node, "modulate", Color(1, 1, 1, 0), time / 2)

	tween.connect("finished", _clear_loading_screen)

func _clear_loading_screen():
	loading_screen_node.visible = false
	if !_vfx_muted:
		loading_mute = false

func saving_screen(time: float = 1.6):
	# if Options.DEBUG_MODE: return
	saving_screen_node.visible = true
	saving_screen_node.modulate = Color.WHITE
	var tween = create_tween()
	tween.tween_property(saving_screen_node, "modulate", Color.WHITE, time / 2)
	tween.tween_property(saving_screen_node, "modulate", Color(1, 1, 1, 0), time / 2)


	tween.connect("finished", _clear_saving_screen)

func _clear_saving_screen():
	saving_screen_node.visible = false

func _unhandled_input(event: InputEvent):
	if event.is_action_pressed("game_toggle_inventory"):
		inventory_open = !inventory_open
		var tween = create_tween()
		var duration = 0.5
		if inventory_open: tween.tween_property(inventory, "position", Vector2(inventory_positions.x, 0), duration).set_ease(Tween.EASE_OUT) # FIX VECTORS PLS
		else: tween.tween_property(inventory, "position", Vector2(inventory_positions.y, 0), duration).set_ease(Tween.EASE_IN)

func _on_quest_meta_clicked(meta: Variant) -> void:
	if "cancel" in meta:
		QuestManager.get_quest(int(meta)).delete()
		QuestManager.highlighted_quest_id = -1
	elif "main_ship" in meta:
		QuestManager.highlighted_quest_id = -1
		QuestManager.highlight_main_station = !QuestManager.highlight_main_station
	elif QuestManager.highlighted_quest_id == int(meta):
		QuestManager.highlighted_quest_id = -1
		QuestManager.highlight_main_station = false
	else:
		QuestManager.highlighted_quest_id = int(meta)
		QuestManager.highlight_main_station = false
		
	QuestManager.update_quest_log()

# DEBUG

func _process(_delta):
	if loading_mute != AudioServer.is_bus_mute(AudioServer.get_bus_index("SFX")):
		AudioServer.set_bus_mute(AudioServer.get_bus_index("SFX"), loading_mute)
	
	if !loading_mute && Player.main_player.floating() != AudioServer.is_bus_mute(AudioServer.get_bus_index("SFX")):
		AudioServer.set_bus_mute(AudioServer.get_bus_index("SFX"), Player.main_player.floating())
	

	if Options.DEVELOPMENT_MODE:
		floating.visible = Player.main_player.floating()
		var pos = World.instance.get_distance_from_center(Player.main_player.global_position)
		player_position.text = ""
		player_position.text += "PPU: X: " + str(round(pos.x)) + ", Y: " + str(round(pos.y)) + "\n" # Player Position in Universe
		pos = World.instance._center_of_universe
		player_position.text += "COU: X: " + str(round(pos.x)) + ", Y: " + str(round(pos.y)) + "\n" # Center Of Universe


	if loading_screen_timer.time_left > 0:
		var ratio = (loading_screen_timer.wait_time - loading_screen_timer.time_left) / loading_screen_timer.wait_time
		loading_screen_bar.value = ratio - 0.0420

		var _scale = 0.65 + ratio * 10
		loading_screen_background.scale = Vector2(_scale, _scale)
		loading_screen_background.rotation = ratio * 2
