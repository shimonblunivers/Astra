using Godot;
using System.Threading.Tasks;

[GlobalClass]
public partial class TextBox : MarginContainer
{
    [Signal] public delegate void FinishedDisplayingEventHandler();

    [Export] public Label TextLabel { get; set; }
    [Export] public Timer DisplayTimer { get; set; }

    public const float MaxWidth = 256.0f;

    private string _text = string.Empty;
    private int _letterIndex = 0;

    public float LetterTime { get; set; } = 0.03f;
    public float SpaceTime { get; set; } = 0.06f;
    public float PunctuationTime { get; set; } = 0.2f;

    public override void _Ready()
    {
        TextLabel ??= GetNode<Label>("MarginContainer/Label");
        DisplayTimer ??= GetNode<Timer>("LetterDisplayTimer");

        DisplayTimer.Timeout += OnLetterDisplayTimerTimeout;
    }

    public async void DisplayText(string textToDisplay)
    {
        _text = textToDisplay ?? string.Empty;
        _letterIndex = 0; // Fixed: reset index for subsequent calls

        // Fixed: Handle empty string safely without indexing out of bounds
        if (string.IsNullOrEmpty(_text))
        {
            EmitSignal(SignalName.FinishedDisplaying);
            return;
        }

        TextLabel.Text = _text;
        await ToSignal(this, Control.SignalName.Resized);

        CustomMinimumSize = new Vector2(Mathf.Min(Size.X, MaxWidth), CustomMinimumSize.Y);

        if (Size.X > MaxWidth)
        {
            TextLabel.AutowrapMode = TextServer.AutowrapMode.Word;
            await ToSignal(this, Control.SignalName.Resized);
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            CustomMinimumSize = new Vector2(CustomMinimumSize.X, Size.Y);
        }

        Position = new Vector2(
            Position.X - (Size.X / 2.0f) * Scale.X,
            Position.Y - (Size.Y + 8.0f) * Scale.Y
        );

        TextLabel.Text = string.Empty;
        DisplayLetter();
    }

    private void DisplayLetter()
    {
        if (_letterIndex >= _text.Length)
        {
            EmitSignal(SignalName.FinishedDisplaying);
            return;
        }

        TextLabel.Text += _text[_letterIndex];
        _letterIndex++;

        if (_letterIndex >= _text.Length)
        {
            EmitSignal(SignalName.FinishedDisplaying);
            return;
        }

        switch (_text[_letterIndex])
        {
            case '!':
            case '.':
            case ',':
            case '?':
                DisplayTimer.Start(PunctuationTime);
                break;
            case ' ':
                DisplayTimer.Start(SpaceTime);
                break;
            default:
                DisplayTimer.Start(LetterTime);
                break;
        }
    }

    private void OnLetterDisplayTimerTimeout()
    {
        DisplayLetter();
    }
}