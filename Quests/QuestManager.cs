using Godot;
using Godot.Collections;
using System.Text;

[GlobalClass]
public partial class QuestManager : Node
{
    public static QuestManager Instance { get; private set; }

    /// <summary>
    /// Dictionary of all tasks (possible quests), identified by their ID.
    /// </summary>
    public Dictionary<int, Task> Tasks { get; } = new Dictionary<int, Task>();

    /// <summary>
    /// Dictionary of quests that are currently active, identified by their ID.
    /// </summary>
    public Dictionary<int, Quest> ActiveQuests { get; } = new Dictionary<int, Quest>();

    public Array<int> ActiveTaskIds { get; } = new Array<int>();

    /// <summary>
    /// List of all quest IDs that have been used in this save.
    /// </summary>
    public Array<int> QuestIdHistory { get; } = new Array<int>();

    public int HighlightedQuestId { get; set; } = -1;
    public bool HighlightMainStation { get; set; } = false;

    public int GetUid()
    {
        int checkId = QuestIdHistory.Count;
        while (true)
        {
            if (!QuestIdHistory.Contains(checkId))
            {
                return checkId;
            }
            checkId++;
        }
    }

    public override void _Ready()
    {
        Instance = this;

        const string path = "res://Quests/Tasks";
        using var dir = DirAccess.Open(path);
        if (dir != null)
        {
            dir.ListDirBegin();
            string fileName = dir.GetNext();

            while (!string.IsNullOrEmpty(fileName))
            {
                if (!dir.CurrentIsDir())
                {
                    if (fileName.Contains(".tres.remap"))
                    {
                        fileName = fileName.TrimSuffix(".remap");
                    }

                    if (fileName.Contains(".tres"))
                    {
                        var task = GD.Load<Task>($"{path}/{fileName}");
                        if (task != null)
                        {
                            GD.Print($"Loaded task: {task.Title}");
                            Tasks[task.Id] = task;
                        }
                        else
                        {
                            GD.PushError($"Error: Failed to load quest resource {fileName}");
                        }
                    }
                }
                fileName = dir.GetNext();
            }

            dir.ListDirEnd();
        }
    }

    public override void _Process(double delta)
    {
        var uiManager = (Node)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("UIManager");
        var uiInstance = (CanvasItem)uiManager?.Get("instance");
        if (uiInstance == null) return;

        var questArrow = (CanvasItem)uiInstance.Get("quest_arrow");
        var distanceLabel = (Label)uiInstance.Get("quest_arrow_distance_label");

        bool showArrow = (ActiveQuests.Count > 0 && HighlightedQuestId != -1) || HighlightMainStation;

        if (questArrow != null) questArrow.Visible = showArrow;
        if (distanceLabel != null) distanceLabel.Visible = showArrow;

        if (!showArrow || Player.MainPlayer == null) return;

        Vector2 playerGlobalPos = Player.MainPlayer.GlobalPosition;
        float playerGlobalRot = (float)Player.MainPlayer.GlobalRotation;

        float distance = 0f;
        Vector2 targetGlobalPos = Vector2.Zero;
        bool hasValidTarget = false;

        var shipManager = (Node)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("ShipManager");
        var mainStation = (Node2D)shipManager?.Get("main_station");

        if (HighlightMainStation)
        {
            if (GodotObject.IsInstanceValid(mainStation))
            {
                targetGlobalPos = mainStation.GlobalPosition;
                distance = (targetGlobalPos - playerGlobalPos).Length();
                hasValidTarget = true;
            }
        }
        else if (HighlightedQuestId != -1)
        {
            if (ActiveQuests.TryGetValue(HighlightedQuestId, out var quest))
            {
                Node2D target = quest.GetTarget();
                if (GodotObject.IsInstanceValid(target))
                {
                    targetGlobalPos = target.GlobalPosition;
                    distance = (targetGlobalPos - playerGlobalPos).Length();
                    hasValidTarget = true;
                }
            }
        }

        if (!hasValidTarget) return;

        const float minimalRange = 150f;
        const float maximalRange = 250f;

        if (distanceLabel != null && questArrow != null)
        {
            if (distance < 999999f && distance > maximalRange)
            {
                distanceLabel.Rotation = -(float)questArrow.Get("rotation");
                distanceLabel.Text = $"{Mathf.RoundToInt(distance / 100f)}m";
            }
            else
            {
                distanceLabel.Text = string.Empty;
            }
        }

        if (questArrow != null)
        {
            if (distance < minimalRange)
            {
                questArrow.Visible = false;
                Color mod = questArrow.Modulate;
                mod.A = 0.75f;
                questArrow.Modulate = mod;
            }
            else
            {
                questArrow.Visible = true;
                float normalizedDistance = Mathf.Clamp((distance - minimalRange) / (maximalRange - minimalRange), 0f, 0.75f);
                Color mod = questArrow.Modulate;
                mod.A = normalizedDistance;
                questArrow.Modulate = mod;
            }

            float arrowRotation = (targetGlobalPos - playerGlobalPos).Angle() - playerGlobalRot;
            questArrow.Set("rotation", arrowRotation);
        }
    }

    public void UpdateQuestLog()
    {
        var uiManager = (Node)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("UIManager");
        if (uiManager == null) return;

        var questLabel = (RichTextLabel)uiManager.Get("quest_label");
        var mainStationLabel = (RichTextLabel)uiManager.Get("main_station_label");

        var sb = new StringBuilder();

        foreach (var kvp in ActiveQuests)
        {
            var quest = kvp.Value;
            if (quest.Id == HighlightedQuestId) sb.Append("[u]");

            sb.Append($"\n[url={quest.Id}][b]{quest.Task.Title}");
            if (quest.Status > 0)
            {
                sb.Append($" [{quest.Status}/2]");
            }
            sb.Append("[/b][/url]");

            if (quest.Id == HighlightedQuestId)
            {
                sb.Append($"[/u] [url=cancel{quest.Id}](X)[/url]");
                sb.Append($"\n{quest.Task.Description}");
            }
        }

        if (questLabel != null)
        {
            questLabel.Text = sb.ToString();
        }

        // Fixed: Ensure all BBCode formatting tags are properly closed
        var stationSb = new StringBuilder("[center][b]");
        if (HighlightMainStation) stationSb.Append("[u]");

        stationSb.Append("[url=main_ship]Main Station[/url]");

        if (HighlightMainStation) stationSb.Append("[/u]");
        stationSb.Append("[/b][/center]");

        if (mainStationLabel != null)
        {
            mainStationLabel.Text = stationSb.ToString();
        }
    }

    public void FinishedQuestObjective(Quest quest)
    {
        quest?.Progress();
        UpdateQuestLog();
    }

    public Task GetTask(int id)
    {
        if (Tasks.TryGetValue(id, out var task))
        {
            return task;
        }

        GD.PushWarning($"Warning: No task found with id {id}");
        return null;
    }

    public Quest GetQuest(int id)
    {
        if (ActiveQuests.TryGetValue(id, out var quest))
        {
            return quest;
        }

        GD.PushWarning($"Warning: No quest found with id {id}");
        return null;
    }

    public Quest GetQuestByTarget(Node2D target)
    {
        if (target == null) return null;

        foreach (var kvp in ActiveQuests)
        {
            if (kvp.Value.GetTarget() == target)
            {
                return kvp.Value;
            }
        }
        return null;
    }

    public bool IsObjective(Node2D target)
    {
        if (target == null) return false;

        foreach (var kvp in ActiveQuests)
        {
            if (kvp.Value.GetTarget() == target)
            {
                return true;
            }
        }
        return false;
    }
}