using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class Menu : CanvasLayer
{
    private dynamic N(string path) => GetNode(path);



    public static readonly PackedScene settings_scene = GD.Load<PackedScene>("res://Scenes/Settings.tscn");
    public static readonly PackedScene credits_scene = GD.Load<PackedScene>("res://Scenes/Credits.tscn");

    public static dynamic instance;

    public dynamic playbutton;
    // onready: playbutton = GetNode<Node>("MarginContainer/VBoxContainer/Play");


    public override void _Ready()
    {
    instance = this;
    N("AudioStreamPlayer").Playing = !ObjectList.started_game;
    // $Camera2D.make_current()
    if (ObjectList.started_game)
    {

        }
    // global_position = Vector2(Player.main_player.camera.global_position.x - get_viewport_rect().size.x / 2, Player.main_player.camera.global_position.y - get_viewport_rect().size.y / 2)
    playbutton.Text = "Zpět do hry";
    N("FurtherBackground").Visible = false;
    N("CloserBackground").Visible = false;

    }
    else if (!DirAccess.dir_exists_absolute("user://saves/worlds/last_save"))
    {
        }
    playbutton.QueueFree();


    }

    public void _on_new_game_pressed()
    {
    if (ObjectList.started_game)
    {
        }
    Player.main_player.camera.make_current();
    get_tree().paused = false;
    World.instance.Visible = true;
    World.instance.ui_node.Visible = true;
    this.QueueFree();
    if (FileAccess.file_exists(SaveFile.get_save_path()))
    {
        }
    DirAccess.remove_absolute(SaveFile.get_save_path());
    World.instance.new_world();
    }
    else
    {
        }
    if (FileAccess.file_exists(SaveFile.get_save_path()))
    {
        }
    DirAccess.remove_absolute(SaveFile.get_save_path());
    get_tree().change_scene_to_file("res://Scenes/Game.tscn");


    }

    public void _on_play_pressed()
    {
    if (ObjectList.started_game)
    {
        }
    // Player.main_player.camera.make_current()
    get_tree().paused = false;
    World.instance.Visible = true;
    World.instance.ui_node.Visible = true;

    this.QueueFree();
    }
    else
    {
        }
    get_tree().change_scene_to_file("res://Scenes/Game.tscn");

    }

    public void _on_settings_pressed()
    {
    visible = false;

    dynamic root = get_tree().root;
    dynamic settings_object = settings_scene.Instantiate();

    // settings_object.global_position = global_position
    root.CallDeferred("add_child", settings_object);

    }

    public void _on_credits_pressed()
    {
    visible = false;

    dynamic root = get_tree().root;
    dynamic credits_object = credits_scene.Instantiate();
    // credits_object.global_position = global_position
    root.CallDeferred("add_child", credits_object);


    }

    public void _on_quit_pressed()
    {
    get_tree().quit();

    }

}