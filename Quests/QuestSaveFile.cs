using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class QuestSaveFile : Resource
{
    private dynamic N(string path) => GetNode(path);


    [Export] int task_id;

    [Export] int npc_id;

    [Export] int target_id;
    [Export] int target_type;

    [Export] int status;

    [Export] int id;


    public static void save()
    {
    Array files = new Array();

    foreach (var quest_key in QuestManager.active_quests.keys())
    {
        }
    dynamic quest = QuestManager.active_quests[quest_key];
    dynamic file = new QuestSaveFile();

    file.task_id = quest.task.id;

    file.npc_id = quest.npc_id;
    file.target_id = quest.target_id;
    file.target_type = quest.target_type;
    file.status = quest.status;
    files.append(file);

    return files

    }

    public void load()
    {
    dynamic quest = Quest.new(task_id, NPC.get_npc(npc_id), target_id, id);

    quest.task.times_activated -= 1;

    quest.status = status;
    quest.target_type = target_type;

    QuestManager.update_quest_log();





    }

}