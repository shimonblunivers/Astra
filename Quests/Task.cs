using Godot;

/// <summary>
/// Task that the player can accept from an NPC.
/// </summary>
[GlobalClass]
public partial class Task : Resource
{
    [Export] public int Id { get; set; }
    [Export] public string Title { get; set; } = string.Empty;
    [Export(PropertyHint.MultilineText)] public string Description { get; set; } = string.Empty;

    [Export] public int Reward { get; set; }
    [Export] public Goal Goal { get; set; }

    [Export] public int WorldLimit { get; set; } = -1;

    /// <summary>
    /// If true, the NPC won't give this mission randomly, only if the previous task is completed.
    /// </summary>
    [Export] public bool IsFollowupTask { get; set; } = false;

    /// <summary>
    /// If true, the mission will be finished completely only when you get back to the NPC.
    /// </summary>
    [Export] public bool ReturnToNpc { get; set; } = true;

    /// <summary>
    /// Required role for NPC to give this mission.
    /// </summary>
    [Export] public NPC.Roles RequiredRole { get; set; } = NPC.Roles.None;

    /// <summary>
    /// Role that will be added to the NPC if they give this mission.
    /// </summary>
    [Export] public NPC.Roles AddRoleOnAccept { get; set; } = NPC.Roles.None;

    [Export] public float DifficultyMultiplier { get; set; } = 1.0f;

    public int TimesActivated { get; set; } = 0;
}