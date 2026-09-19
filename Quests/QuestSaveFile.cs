using Godot;
using Godot.Collections;

[GlobalClass]
public partial class QuestSaveFile : Resource
{
    [Export] public int TaskId { get; set; }
    [Export] public int NpcId { get; set; }
    [Export] public int TargetId { get; set; }
    [Export] public int TargetType { get; set; }
    [Export] public int Status { get; set; }
    [Export] public int Id { get; set; }

    public static Array<QuestSaveFile> Save()
    {
        var files = new Array<QuestSaveFile>();
        var questManager = (Node)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("QuestManager");
        if (questManager == null) return files;

        var activeQuests = questManager.Get("active_quests").AsGodotDictionary();

        foreach (var key in activeQuests.Keys)
        {
            var quest = (Quest)activeQuests[key];
            if (quest == null) continue;

            var file = new QuestSaveFile
            {
                TaskId = quest.Task.Id,
                NpcId = quest.NpcId,
                TargetId = quest.TargetId,
                TargetType = (int)quest.TargetType,
                Status = quest.Status,
                // Fixed: Assign Id so loaded quests preserve their historical/active ID
                Id = quest.Id
            };

            files.Add(file);
        }

        return files;
    }

    public void Load()
    {
        NPC npc = NPC.GetNpc(NpcId);
        if (!GodotObject.IsInstanceValid(npc))
        {
            GD.PushWarning($"Warning: Cannot load quest {Id}, issuing NPC {NpcId} was not found.");
            return;
        }

        var quest = new Quest(TaskId, npc, TargetId, Id);

        if (quest.Task != null)
        {
            quest.Task.TimesActivated--;
        }

        quest.Status = Status;
        quest.TargetType = (Goal.GoalType)TargetType;

        var questManager = (Node)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("QuestManager");
        questManager?.Call("update_quest_log");
    }
}