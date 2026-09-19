class_name Tool extends Resource

@export var name : String
@export var nickname : String
@export var texture : Texture2D

@export var price : int = 100

@export var rotatable : bool = false

@export var object : bool = false
@export var spawn_tile_on_remove : String = ""

@export var world_limit : int = -1
@export var placeable_on_atlas_choords : Vector2i = Vector2i(-1, -1)
@export var terrain_id : int = -1
@export var atlas_coords : Vector2i = Vector2i(-1, -1) # -1 means not set

@export var debug : bool = false

var number_of_instances = 0

func create():
    ShipEditor.tools[name] = self

