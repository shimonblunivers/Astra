using Godot;

[GlobalClass]
public partial class Goal : Resource
{
    public enum GoalType
    {
        GoToPlace,
        TalkToNpc,
        PickUpItem
    }

    [Export] public GoalType Type { get; set; } = GoalType.GoToPlace;

    /// <summary>
    /// Is significant only if the type is "PickUpItem".
    /// </summary>
    [Export] public string ItemType { get; set; } = "Chip";
}