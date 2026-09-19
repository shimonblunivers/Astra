class_name Wall extends ShipPart


@onready var sprite : Sprite2D = $Sprite2D
@onready var light_occluder : LightOccluder2D = $LightOccluder2D
@onready var hp : ProgressBar = $HP
@onready var cracks : AnimatedSprite2D = $Cracks
@onready var button : Button = $Button

static var debris_scene = preload("res://Ship/Walls/Debris/Debris.tscn")

var layer : int = 0

func init(_ship, _coords : Vector2i, _durability : float = 100, _mass : float = 4):
	super(_ship, _coords, _durability, _mass)

func _ready():
	hp.max_value = durability_max
	hp.value = durability_max

func set_texture(texture) -> void:
	sprite.texture = texture

func _on_button_pressed():
	if Options.DEVELOPMENT_MODE:
		damage(25)

func damage(dmg: float):

	durability_current -= dmg

	button.tooltip_text = str(snapped(durability_current / durability_max, 0.01)) + "%"

	match durability_current / durability_max:
		1:
			cracks.frame = 0
		0.75:
			cracks.frame = 1
		0.5:
			cracks.frame = 2
		0.25:
			cracks.frame = 3

	hp.value = durability_current
	if (durability_current <= 0):
		destroy()


func destroy():
	var _debris_object := debris_scene.instantiate()

	ship.destroyed_walls.append(tilemap_coords)

	_debris_object.init(ship, tilemap_coords)
	_debris_object.position = position

	get_parent().add_child.call_deferred(_debris_object)
	var _pos = ship.wall_tile_map.local_to_map(position)
	ship.wall_tile_map.set_cells_terrain_connect([_pos] , 0, -1, false)

	remove()
