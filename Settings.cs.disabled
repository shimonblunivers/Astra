using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class Settings : CanvasLayer
{
    private dynamic N(string path) => GetNode(path);


    public dynamic sound_slider;
    // onready: sound_slider = GetNode<Node>("MarginContainer/VBoxContainer/SoundSlider");
    public dynamic soundtrack_slider;
    // onready: soundtrack_slider = GetNode<Node>("MarginContainer/VBoxContainer/SoundtrackSlider");

    public dynamic sfx_sound_text;
    // onready: sfx_sound_text = GetNode<Node>("SFXTestSound");


    public override void _Ready()
    {
    sound_slider.Value = (AudioServer.get_bus_volume_db(AudioServer.get_bus_index("SFX")) + 30) / 36 * 100;
    soundtrack_slider.Value = (AudioServer.get_bus_volume_db(AudioServer.get_bus_index("Music")) + 30) / 36 * 100;


    }

    public void _on_sound_slider_drag_ended(float value)
    {
    dynamic _value = -30 + 36 * sound_slider.Value / 100;
    AudioServer.set_bus_volume_db(AudioServer.get_bus_index("SFX"), _value);

    if (_value <= -29)
    {
        }
    AudioServer.set_bus_mute(AudioServer.get_bus_index("SFX"), true);
    }
    else
    {
        }
    AudioServer.set_bus_mute(AudioServer.get_bus_index("SFX"), false);

    sfx_sound_text.play();

    }

    public void _on_soundtrack_slider_value_changed(float value)
    {
    dynamic _value = -30 + 36 * value / 100;

    AudioServer.set_bus_volume_db(AudioServer.get_bus_index("Music"), _value);
    if (_value <= -29)
    {
        }
    AudioServer.set_bus_mute(AudioServer.get_bus_index("Music"), true);
    }
    else
    {
        }
    AudioServer.set_bus_mute(AudioServer.get_bus_index("Music"), false);

    }

    public void _on_back_pressed()
    {
    Menu.instance.Visible = true;
    this.QueueFree();




    }

}