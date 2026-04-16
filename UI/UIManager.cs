using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class UIManager : CanvasLayer
{
    private dynamic N(string path) => GetNode(path);



    public dynamic health_label;
    // onready: health_label = GetNode<Node>("HUD/HealthBar/Value");
    public dynamic currency_label;
    // onready: currency_label = GetNode<Node>("HUD/Currency/Value");

    public dynamic death_screen;
    // onready: death_screen = GetNode<Node>("HUD/DeathScreen");

    public dynamic _quest_label;
    // onready: _quest_label = GetNode<Node>("HUD/Inventory/QuestLog/RichTextLabel");
    public dynamic _main_station_label;
    // onready: _main_station_label = GetNode<Node>("HUD/Inventory/MainStationLabel");

    public dynamic inventory;
    // onready: inventory = GetNode<Node>("HUD/Inventory");

    public dynamic loading_screen_node;
    // onready: loading_screen_node = GetNode<Node>("HUD/LoadingScreen");
    public Timer loading_screen_timer;
    // onready: loading_screen_timer = GetNode<Timer>("HUD/LoadingScreen/Timer");
    public ProgressBar loading_screen_bar;
    // onready: loading_screen_bar = GetNode<ProgressBar>("HUD/LoadingScreen/ProgressBar");
    public Node2D loading_screen_background;
    // onready: loading_screen_background = GetNode<Node2D>("HUD/LoadingScreen/Background");

    public AnimatedSprite2D quest_arrow;
    // onready: quest_arrow = GetNode<AnimatedSprite2D>("HUD/QuestArrow/Arrow");
    public Label quest_arrow_distance_label;
    // onready: quest_arrow_distance_label = GetNode<Label>("HUD/QuestArrow/Arrow/Distance");

    public dynamic saving_screen_node;
    // onready: saving_screen_node = GetNode<Node>("HUD/SavingScreen");

    public static dynamic quest_label;
    public static dynamic main_station_label;
    public static dynamic add_currency_label;
    public static dynamic remove_currency_label;
    public static dynamic currency_node;

    public static dynamic instance;

    public bool inventory_open = false;
    public dynamic inventory_positions = Vector2(0, -500) # open, closed;
    // DEBUG

    public dynamic floating;
    // onready: floating = GetNode<Node>("Debug/Floating");
    public dynamic player_position;
    // onready: player_position = GetNode<Node>("Debug/PlayerPosition");

    public bool _vfx_muted = false;
    public static bool loading_mute = false;

    // DEBUG


    public static async Task currency_change_effect(int amount)
    {
    dynamic label;
    if (amount > 0)
    {
        }
    label = add_currency_label.duplicate();
    label.Text = "+" + str(amount);
    }
    else if (amount < 0)
    {
        }
    label = remove_currency_label.duplicate();
    label.Text = str(amount);
    }
    else: return;
    label.Visible = true;
    currency_node.add_child(label);
    dynamic _start_position = label.Position;
    dynamic tween = label.create_tween();
    int duration = 1;
    label.Modulate = Colors.White;
    tween.parallel().tween_property(label, "position", _start_position + Vector2(0, -50), duration);
    tween.parallel().tween_property(label, "modulate", Color(1, 1, 1, 0), duration).set_ease(Tween.EASE_IN);

    await ToSignal(tween, Tween.SignalName.Finished);

    label.QueueFree();


    }

    public override void _Ready()
    {
    instance = this;
    health_label.Text = str(Player.main_player.health);
    currency_label.Text = str(Player.main_player.currency);
    quest_label = _quest_label;
    main_station_label = _main_station_label;
    currency_node = N("HUD/Currency");
    add_currency_label = N("HUD/Currency/AddCurrencyLabel");
    remove_currency_label = N("HUD/Currency/RemoveCurrencyLabel");

    loading_screen_node.Visible = !Options.DEVELOPMENT_MODE;
    floating.Visible = Options.DEVELOPMENT_MODE;
    player_position.Visible = Options.DEVELOPMENT_MODE;

    }

    public void player_health_updated_signal()
    {
    health_label.Text = str(Player.main_player.health);
    death_screen.Visible = !Player.main_player.alive;

    }

    public void _on_player_currency_updated_signal()
    {
    currency_label.Text = str(Player.main_player.currency);


    }

    public async Task loading_screen(float time = 1.6)
    {
    if Options.DEVELOPMENT_MODE: return;
    loading_screen_node.Visible = true;
    if (AudioServer.is_bus_mute(AudioServer.get_bus_index("SFX")))
    {
        }
    _vfx_muted = true;
    }
    else
    {
        }
    _vfx_muted = false;
    loading_mute = true;
    loading_screen_node.Visible = true;
    loading_screen_node.Modulate = Colors.White;
    loading_screen_timer.start(time);

    await ToSignal(loading_screen_timer, Timer.SignalName.Timeout);

    dynamic tween = create_tween();
    tween.tween_property(loading_screen_node, "modulate", Color(1, 1, 1, 0), time / 2);

    tween.connect("finished", _clear_loading_screen);

    }

    public void _clear_loading_screen()
    {

    loading_screen_node.Visible = false;
    if (!_vfx_muted)
    {
        }
    loading_mute = false;

    }

    public void saving_screen(float time = 1.6)
    {
    // if Options.DEBUG_MODE: return

    saving_screen_node.Visible = true;
    saving_screen_node.Modulate = Colors.White;
    dynamic tween = create_tween();
    tween.tween_property(saving_screen_node, "modulate", Colors.White, time / 2);
    tween.tween_property(saving_screen_node, "modulate", Color(1, 1, 1, 0), time / 2);


    tween.connect("finished", _clear_saving_screen);

    }

    public void _clear_saving_screen()
    {
    saving_screen_node.Visible = false;

    }

    public override void _UnhandledInput(InputEvent @event)
    {
    if (event.is_action_pressed("game_toggle_inventory"))
    {
        }
    inventory_open = !inventory_open;
    dynamic tween = create_tween();
    float duration = 0.5;
    if inventory_open: tween.tween_property(inventory, "position", Vector2(inventory_positions.x, 0), duration).set_ease(Tween.EASE_OUT) # FIX VECTORS PLS;
    }
    else: tween.tween_property(inventory, "position", Vector2(inventory_positions.y, 0), duration).set_ease(Tween.EASE_IN);

    }

    public void _on_quest_meta_clicked(Variant meta)
    {
    if ("cancel" in meta)
    {
        }
    QuestManager.get_quest(int(meta)).delete();
    QuestManager.highlighted_quest_id = -1;
    }
    else if ("main_ship" in meta)
    {
        }
    QuestManager.highlighted_quest_id = -1;
    QuestManager.highlight_main_station = !QuestManager.highlight_main_station;
    }
    else if (QuestManager.highlighted_quest_id == int(meta))
    {
        }
    QuestManager.highlighted_quest_id = -1;
    QuestManager.highlight_main_station = false;
    }
    else
    {
        }
    QuestManager.highlighted_quest_id = int(meta);
    QuestManager.highlight_main_station = false;

    QuestManager.update_quest_log();

    }

    public override void _Process(double delta)
    {
    if (loading_mute != AudioServer.is_bus_mute(AudioServer.get_bus_index("SFX")))
    {
        }
    AudioServer.set_bus_mute(AudioServer.get_bus_index("SFX"), loading_mute);

    if (!loading_mute && Player.main_player.floating() != AudioServer.is_bus_mute(AudioServer.get_bus_index("SFX")))
    {

        }
    AudioServer.set_bus_mute(AudioServer.get_bus_index("SFX"), Player.main_player.floating());


    if (Options.DEVELOPMENT_MODE)
    {
        }
    floating.Visible = Player.main_player.floating();
    dynamic pos = World.instance.get_distance_from_center(Player.main_player.GlobalPosition);
    player_position.Text = "";
    player_position.Text += "PPU: X: " + str(round(pos.x)) + ", Y: " + str(round(pos.y)) + "\n" # Player Position in Universe;
    pos = World.instance._center_of_universe;
    player_position.Text += "COU: X: " + str(round(pos.x)) + ", Y: " + str(round(pos.y)) + "\n" # Center Of Universe;


    if (loading_screen_timer.time_left > 0)
    {
        }
    dynamic ratio = (loading_screen_timer.wait_time - loading_screen_timer.time_left) / loading_screen_timer.wait_time;
    loading_screen_bar.Value = ratio - 0.0420;

    dynamic _scale = 0.65 + ratio * 10;
    loading_screen_background.Scale = Vector2(_scale, _scale);
    loading_screen_background.rotation = ratio * 2;


    }

}