using Godot;
using Godot.Collections;
using System;
using Array = Godot.Collections.Array;

[GlobalClass]
public partial class Quest : RefCounted
{
    public Task Task { get; set; }
    public int Id { get; set; }

    public int NpcId { get; set; } = -1;
    public int TargetId { get; set; } = -1;
    public Goal.GoalType TargetType { get; set; }

    public int Status { get; set; } = 0;

    public static Color DefaultOutlineColor { get; } = Colors.Black;
    public static Color ActiveQuestOutlineColor { get; } = Colors.DarkSeaGreen;
    public static Color ObjectiveOfQuestOutlineColor { get; } = Colors.DarkGoldenrod;
    public static Color TalkingAboutQuestOutlineColor { get; } = Colors.MediumPurple;

    public Quest()
    {
    }

    public Quest(int taskId, NPC npc, int targetId = -1, int id = -1)
    {
        var questManager = (Node)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("QuestManager");
        Task = (Task)questManager?.Call("get_task", taskId);
        NpcId = npc != null ? npc.Id : -1;

        if (id == -1)
        {
            Id = (int)questManager.Call("get_uid");
        }
        else
        {
            Id = id;
        }

        var activeQuests = questManager.Get("active_quests").AsGodotDictionary();
        activeQuests[Id] = this;

        var questIdHistory = (Array)questManager.Get("quest_id_history");
        questIdHistory.Add(Id);

        var activeTaskIds = (Array)questManager.Get("active_task_ids");
        activeTaskIds.Add(taskId);

        TargetType = Task.Goal.Type;
        Task.TimesActivated++;

        if (targetId != -1)
        {
            TargetId = targetId;
        }
        else
        {
            SpawnQuestShip();
        }

        if (npc != null)
        {
            var addRole = Task.Get("add_role_on_accept");
            if (addRole.VariantType != Variant.Type.Nil && (int)addRole != (int)NPC.Roles.None)
            {
                var targetRole = (NPC.Roles)(int)addRole;
                if (!npc.RoleList.Contains(targetRole))
                {
                    npc.RoleList.Add(targetRole);
                }
            }

            npc.ActiveQuestId = Id;
            npc.SelectedQuestId = -1;
        }

        questManager.Set("highlighted_quest_id", Id);
        questManager.Call("update_quest_log");
    }

    public NPC GetNpc()
    {
        if (NpcId == -1) return null;

        var npc = NPC.GetNpc(NpcId);
        if (GodotObject.IsInstanceValid(npc))
        {
            return npc;
        }

        GD.PushWarning($"Warning: NPC with ID {NpcId} not found");
        return null;
    }

    public Node2D GetTarget()
    {
        if (TargetId == -1) return null;

        Node2D target = null;
        switch (TargetType)
        {
            case Goal.GoalType.GoToPlace:
                target = null;
                break;
            case Goal.GoalType.TalkToNpc:
                target = NPC.GetNpc(TargetId);
                break;
            case Goal.GoalType.PickUpItem:
                target = Item.GetItem(TargetId);
                break;
        }

        if (GodotObject.IsInstanceValid(target))
        {
            return target;
        }

        GD.PushWarning($"Warning: Target with ID {TargetId} and type {TargetType} not found");
        return null;
    }

    public void Finish()
    {
        if (Player.MainPlayer != null && Task != null)
        {
            Player.MainPlayer.AddCurrency(Task.Reward);
        }

        var world = (Node)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("World");
        if (world != null)
        {
            float difficulty = (float)world.Get("difficulty_multiplier");
            world.Set("difficulty_multiplier", difficulty + 0.2f);
        }

        TargetId = -1;

        var npc = GetNpc();
        npc?.Call("quest_finished");

        Delete();
    }

    public void Delete()
    {
        var questManager = (Node)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("QuestManager");
        if (questManager != null)
        {
            int highlightedId = (int)questManager.Get("highlighted_quest_id");
            if (highlightedId == Id)
            {
                questManager.Set("highlighted_quest_id", -1);
            }

            var activeTaskIds = (Array)questManager.Get("active_task_ids");
            if (Task != null)
            {
                activeTaskIds.Remove(Task.Id);
            }

            var activeQuests = questManager.Get("active_quests").AsGodotDictionary();
            activeQuests.Remove(Id);

            questManager.Call("update_quest_log");
        }

        var npc = GetNpc();
        if (npc != null)
        {
            npc.ActiveQuestId = -1;
            npc.SelectedQuestId = -1;
        }
    }

    public void Progress()
    {
        Status++;

        bool returnToNpc = Task != null && (bool)Task.Get("return_to_npc");
        if ((!returnToNpc && Status == 1) || (returnToNpc && Status > 1))
        {
            Finish();
        }
        else
        {
            TargetId = NpcId;
            TargetType = Goal.GoalType.TalkToNpc;
            GetNpc()?.Call("update_nametag_color");
        }
    }

    public void SpawnQuestShip()
    {
        var world = (Node)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("World");
        float worldDifficulty = world != null ? (float)world.Get("difficulty_multiplier") : 1.0f;
        float taskDifficulty = Task != null ? (float)Task.Get("difficulty_multiplier") : 1.0f;

        Vector2 distances = new Vector2(
            50000f + 10000f * worldDifficulty * taskDifficulty,
            200000f + 10000f * worldDifficulty * (taskDifficulty + 1f)
        );

        var rng = new RandomNumberGenerator();
        rng.Randomize();

        float distance = rng.RandfRange(distances.X, distances.Y);
        float angle = rng.RandfRange(0f, Mathf.Tau);

        Vector2 playerPos = Player.MainPlayer != null ? Player.MainPlayer.GlobalPosition : Vector2.Zero;
        Vector2 newShipPos = playerPos + Vector2.FromAngle(angle) * distance;

        var npcPresets = new Array();
        var itemPresets = new Array();

        switch (TargetType)
        {
            case Goal.GoalType.GoToPlace:
                break;

            case Goal.GoalType.TalkToNpc:
                TargetId = NPC.GetUid();
                var civilianRoles = new Array { (int)NPC.Roles.Civilian };
                string randomName = NPC.Names.Length > 0 ? NPC.Names[GD.Randi() % NPC.Names.Length] : "Crew";
                npcPresets.Add(new NPCPreset(TargetId, randomName, civilianRoles));
                break;

            case Goal.GoalType.PickUpItem:
                TargetId = Item.GetUid();
                string itemKey = Task?.Goal?.ItemType ?? string.Empty;

                if (Item.Types.TryGetValue(itemKey, out var itemType))
                {
                    itemPresets.Add(new ItemPreset(TargetId, itemType, 0));
                }
                else
                {
                    GD.PushWarning($"Warning: ItemType '{itemKey}' not found in Item.Types during Quest setup.");
                }
                break;
        }

        var customObjectSpawn = CustomObjectSpawn.Create(npcPresets, itemPresets);
        var shipManager = (Node)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("ShipManager");

        if (shipManager != null && Task != null)
        {
            string shipPath = (string)shipManager.Call("get_quest_ship_path", Task.Id);
            shipManager.Call("spawn_ship", newShipPos, shipPath, customObjectSpawn);
        }
    }
}