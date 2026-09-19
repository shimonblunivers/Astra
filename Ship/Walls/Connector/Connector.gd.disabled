class_name Connector extends ShipPart


@onready var sprite : AnimatedSprite2D = $Sprite2D
@onready var pin_joint1 : PinJoint2D = $PinJoint2D1
@onready var pin_joint2 : PinJoint2D = $PinJoint2D2

var layer : int = 0

var connected_to : Connector = null

var connectors_in_range = []

func init(_ship, _coords : Vector2i, _durability : float = 200, _mass : float = 5):
	super(_ship, _coords, _durability, _mass)
	ship.connectors.append(self)

func _ready() -> void:
	pin_joint1.node_a = ship.get_path()
	pin_joint2.node_a = ship.get_path()


func connect_to(to : Connector):
	if connected_to != null:
		connected_to.pin_joint1.node_b = ""
		connected_to.pin_joint2.node_b = ""
		connected_to.connected_to = null
		connected_to.sprite.frame = 0
	if to != null:
		pin_joint1.node_b = to.ship.get_path()
		pin_joint2.node_b = to.ship.get_path()
		sprite.frame = 1
		
		connected_to = to
		connected_to.pin_joint1.node_b = self.ship.get_path()
		connected_to.pin_joint2.node_b = self.ship.get_path()
		connected_to.sprite.frame = 1
	else:
		sprite.frame = 0
		connected_to = null 
		pin_joint1.node_b = ""
		pin_joint2.node_b = ""
	

func _on_connector_area_area_entered(area:Area2D) -> void:
	if connected_to == null:
		var body = area.get_parent()
		if body is Connector:
			connectors_in_range.append(body)


func _on_connector_area_area_exited(area:Area2D) -> void:
	var body = area.get_parent()
	if body is Connector:
		connectors_in_range.erase(body)
