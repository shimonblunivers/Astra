using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class Item : Node2D
{
    private dynamic N(string path) => GetNode(path);


    public dynamic area;
    // onready: area = GetNode<Node>("Area2D");
    public dynamic sprite;
    // onready: sprite = GetNode<Node>("Sprite2D");
    public dynamic collision_shape;
    // onready: collision_shape = GetNode<Node>("Area2D/CollisionShape2D");
    public Label itemtag;
    // onready: itemtag = GetNode<Label>("Itemtag");

    public Ship ship = null;

    public int ship_slot_id;

    public bool can_pickup = false;

    public int id;

    public ItemType type;

    public static Array items = new Array();

    public static Array item_id_history = new Array();

    public static dynamic item_scene = GD.Load<PackedScene>("res://Items/Item.tscn");

    public static Dictionary types = new Dictionary();


    public static int get_uid()
    {
    int _id = 0;
    while (true)
    {
        }
    if (Item.get_item(_id) == null && !(_id in item_id_history))
    {
        }
    return _id
    _id += 1;
    return 0

    }

    public static void load_items()
    {
    string path = "res://Items/ItemTypes";
    dynamic dir = DirAccess.open(path);
    if (dir)
    {
        }
    dir.ListDirBegin();
    dynamic file_name = dir.GetNext();
    while (file_name != "")
    {
        }
    if ('.tres.remap' in file_name)
    {
        file_name = file_name.TrimSuffix('.remap');
        }
    if (".tres" in file_name)
    {
        load(path + "/" + file_name).create();
        }
    file_name = dir.GetNext();

    }

    public static Item get_item(int _id)
    {
    for item in items: if item.id == _id: return item;
    return null

    }

    public static ItemType random_item()
    {
    return types[types.keys().pick_random()]

    }

    public static Item spawn(ItemType _type, Vector2 global_coords, int _id = -1, dynamic _ship = null, int _ship_slot_id = -1)
    {
    dynamic new_item = item_scene.Instantiate();
    new_item.type = _type;

    if (_ship != null)
    {
        }
    new_item.ship = _ship;
    new_item.ship.items.add_child(new_item);
    }
    else
    {
        }
    dynamic closest_ship = ObjectList.get_closest_ship(global_coords);
    closest_ship.items.add_child(new_item);
    new_item.ship = closest_ship;

    new_item.ship_slot_id = _ship_slot_id;
    new_item.ship.used_item_slots += 1;

    new_item.GlobalPosition = global_coords;

    if (_id != -1 && Item.get_item(_id) == null)
    {
        }
    new_item.id = _id;
    }
    else
    {
        }
    _id = 0;
    while (true)
    {
        }
    if (Item.get_item(_id) == null)
    {
        new_item.id = _id;
        break;
        }
    _id += 1;

    items.append(new_item);
    item_id_history.append(new_item.id);
    return new_item

    }

    public override void _Ready()
    {
    sprite.Texture = type.Texture;
    collision_shape.shape = type.shape;

    sprite.Scale =  Vector2(collision_shape.shape.get_size().x / sprite.Texture.get_size().x, collision_shape.shape.get_size().x / sprite.Texture.get_size().x);

    itemtag.Position.y -= (collision_shape.shape.get_size().y / 2) + 12;

    itemtag.Text = type.nickname;

    dynamic random = new RandomNumberGenerator();
    dynamic tilt = random.GD.RandRange(-20, 20);
    sprite.RotationDegrees = tilt;
    area.RotationDegrees = tilt;

    CallDeferred("update_itemtag_color");


    }

    public void update_itemtag_color()
    {
    if (QuestManager.is_objective(this))
    {
        }
    itemtag.add_theme_color_override("font_outline_color", Quest.objective_of_quest_outline_color);
    return
    itemtag.add_theme_color_override("font_outline_color", Quest.default_outline_color);


    }

    public override void _PhysicsProcess(double delta)
    {
    if (global_position - Player.main_player.GlobalPosition).length() > Player.main_player.update_range: return;
    {
        }
    area.Position = (- ship.difference_in_position).rotated(-global_rotation);

    }

    public void _on_area_2d_input_event(Node _viewport, InputEvent event, int _shape_idx)
    {
    if (event is InputEventMouseButton && event.ButtonMask == 1 && can_pickup)
    {
        }
    pick_up();

    }

    public void _on_area_2d_mouse_entered()
    {
    N("Itemtag").Visible = true;

    }

    public void _on_area_2d_mouse_exited()
    {
    N("Itemtag").Visible = false;

    }

    public async Task pick_up()
    {
    N("PickedUpSound").play();

    dynamic tween = create_tween();
    tween.tween_property(this, "global_position", Player.main_player.GlobalPosition - Player.main_player.acceleration, (global_position - Player.main_player.GlobalPosition).length() / 1200).set_ease(Tween.EASE_OUT);

    await ToSignal(tween, Tween.SignalName.Finished);

    if (QuestManager.is_objective(this))
    {
        }
    QuestManager.Finished_quest_objective(QuestManager.get_quest_by_target(this));

    visible = false;


    }

    public void delete()
    {

    ship.used_item_slots -= 1;
    items.erase(this);
    ship.pickedup_items.append(id);

    queue_free();


    }

    public void _on_picked_up_sound_finished()
    {

    delete();
    }

}