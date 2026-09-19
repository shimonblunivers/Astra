using Godot;

[GlobalClass]
public partial class Console : RichTextLabel
{
    [Export] public Timer TimerNode { get; set; }

    public float TextTimeout { get; set; } = 5.0f;

    private Tween _fadeTween;

    public override void _Ready()
    {
        TimerNode ??= GetNodeOrNull<Timer>("ConsoleTimer");
        if (TimerNode != null)
        {
            TimerNode.Timeout += OnTimerTimeout;
        }
    }

    public void PrintOut(string message)
    {
        // Cancel any active fade tween
        if (_fadeTween != null && _fadeTween.IsValid())
        {
            _fadeTween.Kill();
        }

        Modulate = Colors.White;

        if (string.IsNullOrEmpty(Text))
        {
            Text = message;
        }
        else
        {
            Text += "\n" + message;
        }

        if (TimerNode != null)
        {
            TimerNode.Stop();
            TimerNode.Start(TextTimeout);
        }
    }

    private void OnTimerTimeout()
    {
        if (_fadeTween != null && _fadeTween.IsValid())
        {
            _fadeTween.Kill();
        }

        _fadeTween = CreateTween();
        _fadeTween.TweenProperty(this, "modulate", new Color(1, 1, 1, 0), TextTimeout / 2.0f);
        _fadeTween.Finished += ClearText;
    }

    private void ClearText()
    {
        Text = string.Empty;
    }
}