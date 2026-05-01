using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class EditorCamera : Camera2D
{
    private dynamic N(string path) => GetNode(path);




    public static readonly dynamic LEFT_LIMIT = -350;

    public locked := false;


    public override void _PhysicsProcess(double delta)
    {
    if locked: return;
    velocity := Vector2();
    direction := Input.get_vector("ui_left", "ui_right", "ui_up", "ui_down");
    dynamic running = Input.get_action_strength("game_run");

    if (direction != Vector2.Zero)
    {
        }
    velocity.x = direction.x * (SPEED + RUN_SPEED_MODIFIER * running) * (1 / zoom.x);
    velocity.y = direction.y * (SPEED + RUN_SPEED_MODIFIER * running) * (1 / zoom.y);

    if position.x + (float)(velocity.x * delta) < LEFT_LIMIT && !Options.DEVELOPMENT_MODE: velocity.x = 0;

    position += Vector2((float)(velocity.x * delta), (float)(velocity.y * delta));

    }

    public override void _UnhandledInput(InputEvent @event)
    {
    if locked: return;
    if (event is InputEventMouseButton && event.is_pressed())
    {
        }
    float zoom_modifier = 0;

    if (event.button_index == MOUSE_BUTTON_WHEEL_DOWN)
    {
        }
    if zoom.x > 1: zoom_modifier = -1;
    }
    else: zoom_modifier = -0.25;

    if (event.button_index == MOUSE_BUTTON_WHEEL_UP)
    {
        }
    zoom_modifier = 1;
    dynamic difference = zoom_modifier * ZOOM_SPEED;
    dynamic new_value = clampf(difference + zoom.x, MIN_ZOOM, MAX_ZOOM);
    zoom = Vector2(new_value, new_value);



    }

}