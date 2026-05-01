using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class Quest : Node
{
    private dynamic N(string path) => GetNode(path);

    // # Instance of a task.
    // TODO: class_name Quest

    public Task task;

    public int id;

    public int npc_id = -1;
    public int target_id = -1;
    public Goal.Type target_type;

    public int status = 0;

    public static dynamic default_outline_color = Color.BLACK;
    public static dynamic active_quest_outline_color = Color.DARK_SEA_GREEN;
    public static dynamic objective_of_quest_outline_color = Color.DARK_GOLDENROD;
    public static dynamic talking_about_quest_outline_color = Color.MEDIUM_PURPLE;


    public NPC get_npc()
    {
    if (npc_id == -1): return null;
    {
        }
    dynamic npc = NPC.get_npc(npc_id);
    if (is_instance_valid(npc)): return npc;
    {

        }
    GD.Print("Warning: NPC with ID " + str(npc_id) + " not found");
    return null

    }

    public Node2D get_target()
    {
    if (target_id == -1): return null;
    {
        }
    Node2D target = null;

    switch (target_type)
    {
        }
    case Goal.Type.go_to_place:
        }
    target = null;
    case Goal.Type.talk_to_npc:
        }
    target = NPC.get_npc(target_id);
    case Goal.Type.pick_up_item:
        }
    target = Item.get_item(target_id);

    if (is_instance_valid(target)): return target;
    {

        }
    GD.Print("Warning: Target with ID " + str(target_id) + " and type " + str(target_type) + " not found");
    return null

    }

    public void _init(int _task_id, NPC _npc, int _target_id = -1, int _id = -1)
    {
    this.task = QuestManager.get_task(_task_id);
    this.npc_id = _npc.id;

    if _id == -1: id = QuestManager.get_uid();
    }
    else: id = _id;

    QuestManager.active_quests[id] = this;

    QuestManager.quest_id_history.append(id);

    QuestManager.active_task_ids.append(_task_id);

    target_type = task.goal.type;

    task.times_activated += 1;

    if _target_id != -1: target_id = _target_id;
    }
    else: spawn_quest_ship();

    if !(task.add_role_on_accept in _npc.roles): _npc.roles.append(task.add_role_on_accept);

    _npc.active_quest_id = id;
    _npc.selected_quest_id = -1;

    QuestManager.highlighted_quest_id = id;

    QuestManager.update_quest_log();

    }

    public void finish()
    {
    Player.main_player.add_currency(task.reward);

    World.difficulty_multiplier += 0.2 # Increase difficulty for the next quests;

    target_id = -1;
    get_npc().quest_finished();

    delete();

    }

    public void delete()
    {
    if (QuestManager.highlighted_quest_id == id)
    {
        }
    QuestManager.highlighted_quest_id = -1;

    get_npc().active_quest_id = -1;
    get_npc().selected_quest_id = -1;

    QuestManager.active_task_ids.erase(task.id);
    QuestManager.active_quests.erase(id);

    QuestManager.update_quest_log();

    }

    public void progress()
    {
    status += 1;

    if !task.return_to_npc && status == 1 || task.return_to_npc && status > 1: finish();
    }
    else
    {
        }
    target_id = npc_id;
    target_type = Goal.Type.talk_to_npc;
    get_npc().update_nametag_color();

    }

    public void spawn_quest_ship()
    {
    dynamic distances = Vector2(50000 + 10000 * World.difficulty_multiplier * task.difficulty_multiplier, 200000 + 10000 * World.difficulty_multiplier * (task.difficulty_multiplier + 1)) #Vector2(10000, 50000);

    dynamic rng = new RandomNumberGenerator();

    dynamic _distance = rng.GD.RandRange(distances.x, distances.y);
    dynamic _angle = rng.GD.RandRange(0, 2 * PI);

    dynamic new_ship_pos = Vector2(Player.main_player.GlobalPosition.x + _distance * cos(_angle), Player.main_player.GlobalPosition.y + _distance * sin(_angle));
    npc_presets := [];;
    item_presets := [];;

    switch (target_type)
    {
        }
    Goal.Type.go_to_place: # TODO;
    // pass
    case Goal.Type.talk_to_npc:
        }
    target_id = NPC.get_uid();
    npc_presets = [NPCPreset.new(target_id, NPC.names.pick_random(), [NPC.Roles.CIVILIAN])];
    case Goal.Type.pick_up_item:
        }
    target_id = Item.get_uid();
    item_presets = [ItemPreset.new(target_id, Item.types[task.goal.item_type], 0)];

    dynamic _custom_object_spawn = CustomObjectSpawn.create(npc_presets, item_presets);

    ShipManager.spawn_ship(new_ship_pos, ShipManager.get_quest_ship_path(task.id), _custom_object_spawn)#.linear_velocity = Player.main_player.parent_ship.linear_velocity;
    }

}