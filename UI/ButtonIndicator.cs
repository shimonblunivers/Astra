using Godot;

[GlobalClass]
public partial class ButtonIndicator : Node2D
{
    [Export] public Button ButtonNode { get; set; }

    public InteractableShipPart InteractableShipPart { get; set; }

    public override void _Ready()
    {
        base._Ready();
        ButtonNode ??= GetNodeOrNull<Button>("Button");
    }

    public void Init(InteractableShipPart interactableShipPart)
    {
        InteractableShipPart = interactableShipPart;
        ButtonNode ??= GetNodeOrNull<Button>("Button");

        if (ButtonNode == null) return;

        var events = InputMap.ActionGetEvents("game_use");
        if (events.Count > 0 && events[0] != null)
        {
            string keyText = events[0].AsText();
            // Clean up standard Godot physical key string representation (e.g., "E (Physical)" -> "E")
            keyText = keyText.Replace(" (Physical)", string.Empty);
            ButtonNode.Text = keyText;
        }
        else
        {
            ButtonNode.Text = "E";
        }
    }
}