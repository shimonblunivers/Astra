class_name ItemSaveFile extends Resource

@export var position : Vector2
@export var id : int
@export var type : ItemType
@export var ship_id : int
@export var ship_slot_id : int

static func save():
    var files = []

    for item in Item.items:
        var file = ItemSaveFile.new()

        file.id = item.id
        file.position = item.global_position
        file.type = item.type
        file.ship_id = item.ship.id
        file.ship_slot_id = item.ship_slot_id

        files.append(file)

    return files

func load():
    pass