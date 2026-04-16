using Godot;

public partial class Slot : Control
{
    private TextureRect _textureRect;
    private Label _priceLabel;
    private Label _nicknameLabel;

    private Tool _tool;

    public override void _Ready()
    {
        _textureRect = GetNode<TextureRect>("Panel/TextureRect");
        _priceLabel = GetNode<Label>("Price");
        _nicknameLabel = GetNode<Label>("Nickname");
    }

    public void SetTool(Tool tool)
    {
        _tool = tool;
        _textureRect.Texture = tool.Texture;
        _nicknameLabel.Text = tool.Nickname;
        _priceLabel.Text = tool.Price.ToString();
    }

    private void OnPanelGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonMask == MouseButtonMask.Left)
        {
            ShipEditor.ChangeTool(_tool.Name);
        }
    }
}
