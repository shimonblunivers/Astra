using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class TaskSaveFile : Resource
{
    private dynamic N(string path) => GetNode(path);



    [Export] int id;
    [Export] int times_activated;


    public static void save()
    {
    Array files = new Array();

    foreach (var task_key in QuestManager.tasks.keys())
    {
        }
    dynamic task = QuestManager.tasks[task_key];
    dynamic file = new TaskSaveFile();

    file.id = task.id;
    file.times_activated = task.times_activated;

    files.append(file);

    return files

    }

    public void load()
    {
    if (QuestManager.tasks.has(id))
    {
        }
    QuestManager.tasks[id].times_activated = times_activated;
    }
    else
    {
        }
    GD.Print("Warning: Task with ID " + str(id) + " not found");

    }

}