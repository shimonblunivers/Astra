using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class Editor : Node2D
{
    private dynamic N(string path) => GetNode(path);



    public Console console;
    // onready: console = GetNode<Console>("HUD/Console/ConsoleLog");
    public ShipEditor ship_editor;
    // onready: ship_editor = GetNode<ShipEditor>("Ship");


    public LineEdit ship_name_label;
    // onready: ship_name_label = GetNode<LineEdit>("HUD/SavemenuUI/ShipName");


    public Inventory inventory;
    // onready: inventory = GetNode<Inventory>("HUD/Inventory");

    public dynamic direction_label;
    // onready: direction_label = GetNode<Node>("HUD/DirectionLabel");
    public dynamic limit_rect;
    // onready: limit_rect = GetNode<Node>("LimitRect");

    public Array ships = new Array();

    public static dynamic instance;

    public bool inventory_open = false;
    public dynamic inventory_positions = Vector2(160, -165) # open, closed;
    // ## Returns true if the files are the same
    // func compare_files(path1 : String, path2 : String) -> bool:
    // var content1 = FileAccess.get_file_as_bytes(path1)
    // var content2 = FileAccess.get_file_as_bytes(path2)
    // return content1 == content2


    public override void _UnhandledInput(InputEvent @event)
    {
    if (event.is_action_pressed("editor_toggle_toolmenu"))
    {
        }
    inventory_open = !inventory_open;
    dynamic tween = create_tween();
    float duration = 0.5;
    if inventory_open: tween.tween_property(inventory, "position", Vector2(inventory_positions.x, 0), duration).set_ease(Tween.EASE_OUT);
    }
    else: tween.tween_property(inventory, "position", Vector2(inventory_positions.y, 0), duration).set_ease(Tween.EASE_IN);

    if (event.is_action_pressed("game_toggle_menu"))
    {
        }
    _exit();

    }

    public override void _Process(double delta)
    {
    limit_rect.Position.y = camera.Position.y - limit_rect.size.y / 2;

    }

    public void center_camera()
    {
    camera.Position = Vector2(ShipEditor.starting_block_coords.x, ShipEditor.starting_block_coords.y);
    camera.zoom = Vector2(1, 1);

    }

    public void _exit()
    {
    this.QueueFree();
    Player.main_player.camera.make_current();

    get_tree().paused = false;
    // get_tree().set_deferred("paused", false)

    World.instance.Visible = true;
    World.instance.ui_node.Visible = true;
    World.used_builder = null;

    }

    public override void _Ready()
    {
    Editor.instance = this;

    limit_rect.Visible = !Options.DEVELOPMENT_MODE;

    _update_ship_list();

    ship_editor.inventory = inventory;

    inventory.load_grid();

    if (Player.main_player.owned_ship == null)
    {
        }
    ship_editor.load_ship("_start_ship", false);
    }
    else
    {
        }
    ship_editor.load_ship(Player.main_player.owned_ship.path, false);

    camera.make_current();

    center_camera();



    }

    public void _on_save_pressed()
    {
    if (!ShipValidator.check_validity(ship_editor.wall_tile_map))
    {
        }
    ship_editor.console.print_out("[color=red]Loď nesplňuje podmínky pro uložení![/color]\nZkontrolujte, zda máte v lodi jádro.\nTaké zkontrolujte zda jsou všechny bloky spojeny.");
    return
    if ((ship_name_label.Text == ""))
    {
        }
    ship_editor.save_ship();
    _update_ship_list();
    return
    ship_editor.save_ship(ship_name_label.Text);
    _update_ship_list();


    }

    public void _update_ship_list()
    {
    ship_editor.evide_tiles();
    string ship_text = "[center][table=3]";
    dynamic dir = DirAccess.open("user://saves/ships");
    if (dir)
    {
        }
    dir.ListDirBegin();
    dynamic file_name = dir.GetNext();
    while (file_name != "")
    {
        }
    if (dir.CurrentIsDir())
    {
        if (!(!Options.DEVELOPMENT_MODE && file_name.BeginsWith('_') || !Options.DEVELOPMENT_MODE && file_name.BeginsWith('%')))
        {
            }
        ship_text += "[cell=1][left][url=" + file_name + "]" + file_name + "[/url][/left][/cell]";
        if (!file_name.BeginsWith('_') && FileAccess.file_exists("user://saves/ships/" + file_name + "/details.dat"))
        {
            }
        dynamic details = FileAccess.open("user://saves/ships/" + file_name + "/details.dat", FileAccess.READ);
        dynamic price = details.get_16();
        ship_text += "[cell=1]      ->      [/cell][cell=1][right]";
        if price > ship_editor.current_ship_price + inventory.currency: ship_text += "[color=red]";
        ship_text += str(price);
        if price > ship_editor.current_ship_price + inventory.currency: ship_text += "[/color]";
        ship_text += " [img]res://UI/currency.png[/img]" + "[/right][/cell]";
        details.close();
        }
        else
        {
            }
        ship_text += "[cell=1][/cell][cell=1][/cell]";
        // ship_text += "[/url]"
        }
    file_name = dir.GetNext();
    dir.ListDirEnd();

    if (Options.DEVELOPMENT_MODE)
    {
        }
    dir = DirAccess.open("res://DefaultSave/ships");
    if (dir)
    {
        }
    dir.ListDirBegin();
    dynamic file_name = dir.GetNext();
    while (file_name != "")
    {
        if (dir.CurrentIsDir())
        {
            }
        ship_text += "[cell=1][left][url=" + file_name + "]" + file_name + "[/url][/left][/cell]";
        if (!file_name.BeginsWith('_') && FileAccess.file_exists("user://saves/ships/" + file_name + "/details.dat"))
        {
            }
        dynamic details = FileAccess.open("user://saves/ships/" + file_name + "/details.dat", FileAccess.READ);
        dynamic price = details.get_16();
        ship_text += "[cell=1]      ->      [/cell][cell=1][right]";
        if price > ship_editor.current_ship_price + inventory.currency: ship_text += "[color=red]";
        ship_text += str(price);
        if price > ship_editor.current_ship_price + inventory.currency: ship_text += "[/color]";
        ship_text += " [img]res://UI/currency.png[/img]" + "[/right][/cell]";
        details.close();
        }
        else
        {
            }
        ship_text += "[cell=1][/cell][cell=1][/cell]";
        file_name = dir.GetNext();
        }
    dir.ListDirEnd();

    ship_text += "[/table][/center]";
    ship_list.Text = ship_text;


    }

    public void _on_load_pressed()
    {
    dynamic success;
    if (!ship_name_label.Text.BeginsWith('_') && FileAccess.file_exists("user://saves/ships/" + ship_name_label.Text + "/details.dat"))
    {
        }
    dynamic details = FileAccess.open("user://saves/ships/" + ship_name_label.Text + "/details.dat", FileAccess.READ);
    dynamic price = details.get_16();
    if (price > ship_editor.current_ship_price + inventory.currency && !Options.DEVELOPMENT_MODE)
    {
        }
    console.print_out("[color=red]Na tuto loď nemáš dostatek prostředků![/color]");
    return
    if (ship_name_label.Text == ""): success = ship_editor.load_ship();
    {
        }
    else: success = ship_editor.load_ship(ship_name_label.Text);
    if !success: console.print_out("[color=red]Loď s názvem '" + ship_name_label.Text + "' nebyla nalezena![/color]");
    }
    else
    {
        }
    _on_exit_pressed();
    center_camera();

    }

    public void _on_open_savemenu_pressed()
    {
    ShipValidator.autofill_floor(ShipEditor.instance.wall_tile_map);
    savemenu.Visible = true;
    N("HUD/Savemenu").Visible = false;
    camera.locked = true;
    _update_ship_list();

    }

    public void _on_exit_pressed()
    {
    savemenu.Visible = false;
    N("HUD/Savemenu").Visible = true;
    camera.locked = false;

    }

    public void _on_ship_list_meta_clicked(Variant meta)
    {
    ship_name_label.Text = meta;

    }

    public void _on_autofloor_pressed()
    {
    ShipValidator.autofill_floor(ShipEditor.instance.wall_tile_map);

    }

    public void _on_autofloor_button_toggled(bool toggled_on)
    {
    ShipEditor.autoflooring = toggled_on;
    if toggled_on: ShipValidator.autofill_floor(ShipEditor.instance.wall_tile_map);

    }

    public void _on_deploy_pressed()
    {
    ShipValidator.autofill_floor(ShipEditor.instance.wall_tile_map);


    if (!ShipValidator.check_validity(ship_editor.wall_tile_map))
    {
        }
    ship_editor.console.print_out("[color=red]Loď nesplňuje podmínky pro uložení![/color]\nZkontrolujte, zda máte v lodi jádro.\nTaké zkontrolujte zda jsou všechny bloky spojeny.");
    return

    ship_editor.save_ship("%player_ship_new");



    // var path = "user://saves/ships"
    // var dir = DirAccess.open(path)

    // var ship_names = []
    // if dir:
    // dir.list_dir_begin()
    // var file_name = dir.get_next()
    // while file_name != "":
    // if dir.current_is_dir():
    // ship_names.append(file_name)
    // file_name = dir.get_next()

    // var saved_file_name = ""

    // var ship_num = 0
    // while saved_file_name == "":
    // if "%player_ship_" + str(ship_num) in ship_names: ship_num += 1
    // else: saved_file_name = "%player_ship_" + str(ship_num)


    // 
    // dir = DirAccess.open("user://saves/ships")
    // if dir:
    // dir.list_dir_begin()
    // var file_name = dir.get_next()
    // while file_name != "":
    // if file_name != saved_file_name:
    // if compare_files(path + "/" + file_name + "/walls.dat", path + "/" + saved_file_name + "/walls.dat"):
    // if compare_files(path + "/" + file_name + "/objects.dat", path + "/" + saved_file_name + "/objects.dat"):
    // delete_directory(path + "/" + file_name)

    // file_name = dir.get_next()

    if (World.used_builder != null)
    {
        }
    ShipManager.build_ship(World.used_builder, true, "%player_ship_new");
    Player.main_player.currency = ship_editor.inventory.currency;

    _exit();

    }

}