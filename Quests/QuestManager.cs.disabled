using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class QuestManager : Node
{
    private dynamic N(string path) => GetNode(path);

    // # Global singleton that manages all quests in the game.

    // # Dictionary of all tasks (possible quests), identified by their ID.
    public Dictionary tasks = new Dictionary();

    // # Dictionary of quests that are currently active, identified by their ID.
    public Dictionary active_quests = new Dictionary();
    public Array active_task_ids = new Array();

    // # List of all quest IDs that have been used in this save.
    public Array quest_id_history = new Array();

    public highlighted_quest_id := -1;
    public bool highlight_main_station = false;


    public int get_uid()
    {
    dynamic _id = quest_id_history.Count;
    while (true)
    {
        }
    if (!(_id in quest_id_history))
    {
        }
    return _id
    _id += 1;
    return 0

    }

    public override void _Ready()
    {
    dynamic dir = DirAccess.open("res://Quests/Tasks");
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
        if ('.tres.remap' in file_name)
        {
            }
        file_name = file_name.TrimSuffix('.remap');
        if (".tres" in file_name)
        {
            }
        Task task = load("res://Quests/Tasks/" + file_name);
        if (task)
        {
            }
        GD.Print("Loaded task: " + task.title);
        tasks[task.id] = task;
        }
        else: printerr("Error: Failed to load quest resource " + file_name);
        }
    file_name = dir.GetNext();
    dir.ListDirEnd();

    }

    public override void _Process(double delta)
    {
    if (UIManager.instance)
    {
        }
    dynamic show_arrow = active_quests.Count > 0 && highlighted_quest_id != -1 || highlight_main_station;
    UIManager.instance.quest_arrow.Visible = show_arrow;
    UIManager.instance.quest_arrow_distance_label.Visible = show_arrow;
    if (show_arrow)
    {
        }
    distance := 0.0;
    if highlight_main_station: distance = (ShipManager.main_station.GlobalPosition - Player.main_player.GlobalPosition).length();
    }
    else if (highlighted_quest_id != -1)
    {
        if (get_quest(highlighted_quest_id))
        {
            }
        if get_quest(highlighted_quest_id).get_target(): distance = (get_quest(highlighted_quest_id).get_target().GlobalPosition - Player.main_player.GlobalPosition).length();
        }
        else
        {
            }
        GD.Print("Warning: Quest with ID " + str(highlighted_quest_id) + " not found.");
        }
    int minimal_range = 150;
    int maximal_range = 250;

    if (distance < 999999 && distance > maximal_range)
    {
        UIManager.instance.quest_arrow_distance_label.rotation = -UIManager.instance.quest_arrow.rotation;
        UIManager.instance.quest_arrow_distance_label.Text = str(round(distance/100)) + "m";
        }
    else
    {
        UIManager.instance.quest_arrow_distance_label.Text = "";

        }
    if (distance < minimal_range)
    {
        UIManager.instance.quest_arrow.Visible = false;
        UIManager.instance.quest_arrow.Modulate.a = 0.75;
        }
    else
    {
        UIManager.instance.quest_arrow.Visible = true;
        dynamic normalized_distance = clamp((distance - minimal_range) / (maximal_range - minimal_range), 0, 0.75);
        UIManager.instance.quest_arrow.Modulate.a = normalized_distance;
        }
    if highlight_main_station: UIManager.instance.quest_arrow.rotation = (ShipManager.main_station.GlobalPosition - Player.main_player.GlobalPosition).angle() - Player.main_player.global_rotation;
    }
    else if (highlighted_quest_id != -1)
    {
        if (get_quest(highlighted_quest_id))
        {
            }
        if get_quest(highlighted_quest_id).get_target(): UIManager.instance.quest_arrow.rotation = (get_quest(highlighted_quest_id).get_target().GlobalPosition - Player.main_player.GlobalPosition).angle() - Player.main_player.global_rotation;
        }
        else
        {
            }
        GD.Print("Warning: Quest with ID " + str(highlighted_quest_id) + " not found.");

        }
    }

    public void update_quest_log()
    {
    string string_to_add = "";
    foreach (var quest_key in active_quests.keys())
    {
        }
    dynamic quest = active_quests[quest_key];
    if quest.id == highlighted_quest_id: string_to_add += "[u]";
    string_to_add += "\n[url=" + str(quest.id) + "][b]" + quest.task.title;
    if quest.status > 0: string_to_add += " [" + str(quest.status) + "/2]";
    string_to_add += "[/b][/url]";
    if (quest.id == highlighted_quest_id)
    {
        }
    string_to_add += "[/u] [url=cancel" + str(quest.id) + "](X)[/url]";
    string_to_add += "\n" + quest.task.description;
    UIManager.quest_label.Text = string_to_add;

    string_to_add = "[center][b]";
    if highlight_main_station: string_to_add += "[u]";
    string_to_add += "[url=main_ship]Hlavní stanice";
    UIManager.main_station_label.Text = string_to_add;

    }

    public void finished_quest_objective(Quest quest)
    {
    quest.progress();
    QuestManager.update_quest_log();

    }

    public Task get_task(int id)
    {
    if tasks.has(id): return tasks[id];
    GD.Print("Warning: No task found with id " + str(id));
    return null

    }

    public Quest get_quest(int id)
    {
    if active_quests.has(id): return active_quests[id];
    GD.Print("Warning: No quest found with id " + str(id));
    return null

    }

    public Quest get_quest_by_target(Node2D target)
    {
    foreach (var quest_key in active_quests.keys())
    {
        }
    dynamic quest = active_quests[quest_key];
    if quest.get_target() == target: return quest;
    return null

    }

    public bool is_objective(Node2D target)
    {
    foreach (var quest_key in active_quests.keys())
    {
        }
    dynamic quest = active_quests[quest_key];
    if quest.get_target() == target: return true;
    return false

    }

}