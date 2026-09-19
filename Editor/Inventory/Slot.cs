using Godot;

[GlobalClass]
public partial class Slot : Control
{
    [Export] public TextureRect TextureRectNode { get; set; }
    [Export] public Label PriceLabelNode { get; set; }
    [Export] public Label NicknameLabelNode { get; set; }

    public Tool CurrentTool { get; private set; }

    public override void _Ready()
    {
        EnsureNodes();
    }

    private void EnsureNodes()
    {
        TextureRectNode ??= GetNodeOrNull<TextureRect>("Panel/TextureRect");
        PriceLabelNode ??= GetNodeOrNull<Label>("Price");
        NicknameLabelNode ??= GetNodeOrNull<Label>("Nickname");
    }

    public void SetTool(Tool newTool)
    {
        CurrentTool = newTool;
        if (CurrentTool == null) return;

        EnsureNodes();

        if (TextureRectNode != null)
        {
            TextureRectNode.Texture = (Texture2D)CurrentTool.Get("texture");
        }

        if (NicknameLabelNode != null)
        {
            NicknameLabelNode.Text = (string)CurrentTool.Get("nickname");
        }

        if (PriceLabelNode != null)
        {
            PriceLabelNode.Text = CurrentTool.Get("price").ToString();
        }
    }

    public void OnPanelGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && (mouseEvent.ButtonMask & MouseButtonMask.Left) != 0)
        {
            if (CurrentTool == null) return;

            var shipEditor = (Node)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("ShipEditor");
            if (shipEditor != null)
            {
                shipEditor.Call("change_tool", CurrentTool.Get("name"));
            }
        }
    }
}