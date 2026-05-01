using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class World : Node2D
{
    private dynamic N(string path) => GetNode(path);


    public static World instance;

    public dynamic _center_of_universe = Vector2.Zero;

    public static SaveFile save_file;

    public static Builder used_builder = null;

    public static float difficulty_multiplier = 0;

    public dynamic canvas_modulate;
    // onready: canvas_modulate = GetNode<Node>("CanvasModulate");
    public dynamic ui_node;
    // onready: ui_node = GetNode<Node>("UI");

    public static readonly PackedScene editor_scene = GD.Load<PackedScene>("res://Scenes/Editor.tscn");
    public static readonly PackedScene menu_scene = GD.Load<PackedScene>("res://Scenes/Menu.tscn");


    // func _unhandled_input(event: InputEvent):
    // if Options.DEVELOPMENT_MODE:
    // if event.is_action_pressed("game_toggle_menu"):
    // open_editor()



    public void load_missions()
    {
    string path = "res://Quests/Missions";
    dynamic dir = DirAccess.open(path);
    if (dir)
    {
        }
    dir.ListDirBegin();
    dynamic file_name = dir.GetNext();
    while (file_name != "")
    {
        }
    if (!dir.CurrentIsDir())
    {
        if '.tres.remap' in file_name: # <---- NEW;
        file_name = file_name.TrimSuffix('.remap') # <---- NEW;
        if (".tres" in file_name)
        {
            }
        load(path + "/" + file_name).create();
        }
    file_name = dir.GetNext();


    }

    public override void _Ready()
    {
    World.instance = this;
    DisplayServer.window_set_max_size(Vector2I(3840, 2160));
    ObjectList.started_game = true;
    load_missions();

    save_file = new SaveFile();
    save_file.initialize_files();

    }

    public static void reset_values()
    {
    UIManager.instance.loading_screen();
    while Ship.ships.Count != 0: Ship.ships[0].delete();
    while NPC.npcs.Count != 0: NPC.npcs[0].delete();
    while Item.items.Count != 0: Item.items[0].delete();

    for quest in QuestManager.active_quests.Values(): quest.delete();

    Item.items.clear();
    Item.item_id_history.clear();
    NPC.npcs.clear();

    QuestManager.quest_id_history.clear();
    QuestManager.highlighted_quest_id = -1;
    QuestManager.highlight_main_station = false;


    Player.main_player.health = Player.main_player.max_health;
    Player.main_player.currency = 0;
    Player.main_player.currency_updated_signal.emit();

    World.instance._center_of_universe = Vector2.Zero;
    World.instance.transform.origin = Vector2.Zero;


    }

    public void new_world()
    {

    // var tree := World.instance.get_tree().get_root()
    // World.instance.free()
    // tree.add_child(load("res://Scenes/Game.tscn").instantiate())

    save_file = new SaveFile();
    save_file.initialize_files();

    World.reset_values();

    GD.Print("Generating ships..");

    ShipManager.randomly_generate_ships();

    GD.Print("Spawning player..");

    Player.main_player.spawn();

    GD.Print("New world created.");


    }

    public override void _UnhandledInput(InputEvent @event)
    {
    if (event.is_action_pressed("game_toggle_menu"))
    {
        }
    open_menu();

    }

    public void shift_origin(Vector2 by)
    {
    global_position += by;
    _center_of_universe += by;

    }

    public Vector2 get_distance_from_center(Vector2 pos)
    {
    return pos
    // return pos - _center_of_universe

    }

    public override void _Process(double delta)
    {
    if !Options.DEVELOPMENT_MODE: return;
    queue_redraw();

    }

    public override void _Draw()
    {
    if !Options.DEVELOPMENT_MODE: return;
    draw_circle(-_center_of_universe , 25 , Colors.LightBlue);

    }

    public void open_editor(Builder _builder = null)
    {
    used_builder = _builder;

    visible = false;
    ui_node.Visible = false;
    dynamic root = get_tree().root;
    // root.remove_child(self)
    get_tree().paused = true;
    dynamic editor_object = editor_scene.Instantiate();
    root.CallDeferred("add_child", editor_object);

    }

    public void open_menu()
    {
    // visible = false
    // ui_node.visible = false
    dynamic root = get_tree().root;
    // root.remove_child(self)
    get_tree().paused = true;
    dynamic menu_object = menu_scene.Instantiate();
    root.CallDeferred("add_child", menu_object);




    }

    public void _on_audio_stream_player_finished()
    {
    N("AudioStreamPlayer").play();
    }

}