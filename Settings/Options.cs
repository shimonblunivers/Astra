using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class Options : Node
{
    private dynamic N(string path) => GetNode(path);


    [Export] bool DEVELOPMENT_MODE = (OS.has_feature("editor") ? true : false);
    [Export] bool FULLSCREEN = false;



    public override void _UnhandledInput(InputEvent @event)
    {
    if (event.is_action_pressed("toggle_fullscreen"))
    {
        }
    FULLSCREEN = !FULLSCREEN;
    if (FULLSCREEN)
    {
        }
    DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_EXCLUSIVE_FULLSCREEN);
    }
    else
    {
        }
    DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_WINDOWED);
    }

}