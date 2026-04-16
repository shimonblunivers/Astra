using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class Console : RichTextLabel
{
    private dynamic N(string path) => GetNode(path);


    public Timer timer;
    // onready: timer = GetNode<Timer>("ConsoleTimer");

    public int text_timeout = 5;


    public async Task print_out(string string)
    {
    modulate = Colors.White;
    text += "\n" + string;

    timer.start(text_timeout);

    await ToSignal(timer, Timer.SignalName.Timeout);

    dynamic tween = create_tween();

    tween.tween_property(this, "modulate", Color(1, 1, 1, 0), text_timeout / 2);

    tween.connect("finished", _clear);

    }

    public void _clear()
    {
    text = "";

    }

}