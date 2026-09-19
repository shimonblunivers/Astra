using Godot;
using Godot.Collections;

[GlobalClass]
public partial class TaskSaveFile : Resource
{
    [Export] public int Id { get; set; }
    [Export] public int TimesActivated { get; set; }

    public static Array<TaskSaveFile> Save()
    {
        var files = new Array<TaskSaveFile>();
        var questManager = QuestManager.Instance;

        if (questManager == null)
        {
            questManager = (QuestManager)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("QuestManager");
            if (questManager == null) return files;
        }

        foreach (var kvp in questManager.Tasks)
        {
            var task = kvp.Value;
            if (task == null) continue;

            var file = new TaskSaveFile
            {
                Id = task.Id,
                TimesActivated = task.TimesActivated
            };

            files.Add(file);
        }

        return files;
    }

    public void Load()
    {
        var questManager = QuestManager.Instance;

        if (questManager == null)
        {
            questManager = (QuestManager)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("QuestManager");
        }

        if (questManager != null && questManager.Tasks.TryGetValue(Id, out var task))
        {
            task.TimesActivated = TimesActivated;
        }
        else
        {
            GD.PushWarning($"Warning: Task with ID {Id} not found");
        }
    }
}