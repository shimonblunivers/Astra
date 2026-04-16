using Godot;

public partial class Inventory : Node2D
{
    private GridContainer _grid;

    private static readonly PackedScene SlotScene = GD.Load<PackedScene>("res://Editor/Inventory/Slot.tscn");

    public static Node CurrencyNode;
    public static Label CurrencyValue;
    public static Label AddCurrencyLabel;
    public static Label RemoveCurrencyLabel;
    public static Timer CurrencyTimer;

    public static int Currency = 0;

    public override void _Ready()
    {
        _grid = GetNode<GridContainer>("GridContainer");

        CurrencyNode = GetNode<Node>("Currency");
        CurrencyValue = GetNode<Label>("Currency/Value");
        AddCurrencyLabel = GetNode<Label>("Currency/AddCurrencyLabel");
        RemoveCurrencyLabel = GetNode<Label>("Currency/RemoveCurrencyLabel");
        CurrencyTimer = GetNode<Timer>("Currency/Timer");

        Currency = Player.MainPlayer.Currency;
        CurrencyValue.Text = Currency.ToString();
    }

    public void LoadGrid()
    {
        foreach (Variant key in ShipEditor.Tools.Keys)
        {
            var tool = ShipEditor.Tools[key].AsGodotObject<Tool>();
            if (tool.Debug && !Options.DevelopmentMode) continue;

            var newSlot = SlotScene.Instantiate<Slot>();
            _grid.AddChild(newSlot);
            newSlot.SetTool(tool);
        }
    }

    public static bool AddCurrency(int amount, bool visual = true)
    {
        if (amount == 0) return false;
        if (Currency + amount < 0)
        {
            if (visual) CurrencyChangeEffect(0);
            return false;
        }
        if (visual) CurrencyChangeEffect(amount);
        Currency += amount;
        CurrencyValue.Text = Currency.ToString();
        return true;
    }

    public static async void CurrencyChangeEffect(int amount)
    {
        if (amount == 0)
        {
            CurrencyTimer.Start(1);
            CurrencyValue.Set("theme_override_colors/font_color", Colors.Red);
            await CurrencyValue.ToSignal(CurrencyTimer, Timer.SignalName.Timeout);
            CurrencyValue.Set("theme_override_colors/font_color", Colors.White);
            return;
        }

        Label label;
        if (amount > 0)
        {
            label = (Label)AddCurrencyLabel.Duplicate();
            label.Text = "+" + amount.ToString();
        }
        else
        {
            label = (Label)RemoveCurrencyLabel.Duplicate();
            label.Text = amount.ToString();
        }

        label.Visible = true;
        CurrencyNode.AddChild(label);

        Vector2 startPosition = label.Position;
        Tween tween = label.CreateTween();
        float duration = 1f;
        label.Modulate = Colors.White;

        tween.Parallel().TweenProperty(label, "position", startPosition + new Vector2(0, -50), duration);
        tween.Parallel().TweenProperty(label, "modulate", new Color(1, 1, 1, 0), duration).SetEase(Tween.EaseType.In);

        await label.ToSignal(tween, Tween.SignalName.Finished);
        label.QueueFree();
    }
}
