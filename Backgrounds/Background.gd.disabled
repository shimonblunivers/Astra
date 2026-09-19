extends Node2D

func _process(_delta):
	var distance = World.instance.get_distance_from_center(global_position)
	var _zoom = Player.main_player.camera.zoom.x
	for child in get_children():
		child.material.set_shader_parameter("background_position", distance)
		child.region_rect.position = -Vector2(2048 / _zoom, 2048 / _zoom)/2
		child.region_rect.size = Vector2(2048 + 2048 / _zoom, 2048 + 2048 / _zoom)
	global_rotation = 0.0



