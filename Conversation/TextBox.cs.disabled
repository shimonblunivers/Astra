using Godot;

public partial class TextBox : MarginContainer
{
    [Signal]
    public delegate void FinishedDisplayingEventHandler();

    private const int MaxWidth = 256;

    private Label _label;
    private Timer _timer;

    private string _text = "";
    private int _letterIndex = 0;

    private float _letterTime = 0.03f;
    private float _spaceTime = 0.06f;
    private float _punctuationTime = 0.2f;

    public override void _Ready()
    {
        _label = GetNode<Label>("MarginContainer/Label");
        _timer = GetNode<Timer>("LetterDisplayTimer");
    }

    public async void DisplayText(string textToDisplay)
    {
        _text = textToDisplay;
        _label.Text = textToDisplay;

        await ToSignal(this, "resized");
        CustomMinimumSize = new Vector2(Mathf.Min(Size.X, MaxWidth), CustomMinimumSize.Y);

        if (Size.X > MaxWidth)
        {
            _label.AutowrapMode = TextServer.AutowrapMode.Word;
            await ToSignal(this, "resized");
            await ToSignal(this, "resized");
            CustomMinimumSize = new Vector2(CustomMinimumSize.X, Size.Y);
        }

        Position = new Vector2(
            Position.X - (Size.X / 2f) * Scale.X,
            Position.Y - (Size.Y + 8f) * Scale.Y
        );

        _label.Text = "";
        DisplayLetter();
    }

    private void DisplayLetter()
    {
        _label.Text += _text[_letterIndex].ToString();
        _letterIndex++;

        if (_letterIndex >= _text.Length)
        {
            EmitSignal(SignalName.FinishedDisplaying);
            return;
        }

        char next = _text[_letterIndex];
        switch (next)
        {
            case '!':
            case '.':
            case ',':
            case '?':
                _timer.Start(_punctuationTime);
                break;
            case ' ':
                _timer.Start(_spaceTime);
                break;
            default:
                _timer.Start(_letterTime);
                break;
        }
    }

    private void OnLetterDisplayTimerTimeout()
    {
        DisplayLetter();
    }
}
