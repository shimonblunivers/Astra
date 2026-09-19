using Godot;

[GlobalClass]
public partial class Credits : CanvasLayer
{
    [Export] public Button BackButton { get; set; }

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;

        BackButton ??= GetNodeOrNull<Button>("MarginContainer/Back");

        if (BackButton != null)
        {
            BackButton.Pressed += OnBackPressed;
        }
    }

    public void OnBackPressed()
    {
        if (GodotObject.IsInstanceValid(Menu.Instance))
        {
            Menu.Instance.Visible = true;
        }

        QueueFree();
    }
}