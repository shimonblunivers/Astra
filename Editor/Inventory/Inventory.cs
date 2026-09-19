using Godot;
using System.Threading.Tasks;

[GlobalClass]
public partial class Inventory : Node2D
{
    private static readonly PackedScene SlotScene = GD.Load<PackedScene>("res://Editor/Inventory/Slot.tscn");

    public static Inventory Instance { get; private set; }

    [Export] public GridContainer Grid { get; set; }
    [Export] public Node2D CurrencyNode { get; set; }
    [Export] public Label CurrencyValue { get; set; }
    [Export] public Label AddCurrencyLabel { get; set; }
    [Export] public Label RemoveCurrencyLabel { get; set; }
    [Export] public Timer CurrencyTimer { get; set; }

    public static float Currency
    {
        get => Player.MainPlayer != null ? Player.MainPlayer.Currency : 0;
        set
        {
            if (Player.MainPlayer != null)
            {
                Player.MainPlayer.Currency = value;
            }
        }
    }

    public override void _Ready()
    {
        Instance = this;

        Grid ??= GetNode<GridContainer>("GridContainer");
        CurrencyNode ??= GetNode<Node2D>("Currency");
        CurrencyValue ??= GetNode<Label>("Currency/Value");
        AddCurrencyLabel ??= GetNode<Label>("Currency/AddCurrencyLabel");
        RemoveCurrencyLabel ??= GetNode<Label>("Currency/RemoveCurrencyLabel");
        CurrencyTimer ??= GetNode<Timer>("Currency/Timer");

        // Sync currency with Player
        if (Player.MainPlayer != null)
        {
            CurrencyValue.Text = Player.MainPlayer.Currency.ToString();
        }
        else
        {
            CurrencyValue.Text = "0";
        }
    }

    public static bool AddCurrency(int amount, bool visual = true)
    {
        if (amount == 0) return false;

        float current = Currency;
        if (current + amount < 0)
        {
            if (visual) CurrencyChangeEffect(0);
            return false;
        }

        if (visual) CurrencyChangeEffect(amount);
        Currency += amount;

        if (Instance != null && GodotObject.IsInstanceValid(Instance.CurrencyValue))
        {
            Instance.CurrencyValue.Text = Currency.ToString();
        }

        return true;
    }

    public void LoadGrid()
    {
        var shipEditor = (Node)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("ShipEditor");
        var options = (Node)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("Options");
        if (shipEditor == null) return;

        bool devMode = options != null && (bool)options.Get("DEVELOPMENT_MODE");
        var toolsDict = shipEditor.Get("tools").AsGodotDictionary();

        foreach (var key in toolsDict.Keys)
        {
            var tool = (GodotObject)toolsDict[key];
            bool isDebug = (bool)tool.Get("debug");
            if (isDebug && !devMode) continue;

            var newSlot = SlotScene.Instantiate();
            Grid.AddChild(newSlot);
            newSlot.Call("set_tool", tool);
        }
    }

    public static async void CurrencyChangeEffect(int amount)
    {
        if (Instance == null || !GodotObject.IsInstanceValid(Instance)) return;

        if (amount == 0)
        {
            if (GodotObject.IsInstanceValid(Instance.CurrencyTimer) && GodotObject.IsInstanceValid(Instance.CurrencyValue))
            {
                Instance.CurrencyTimer.Start(1);
                Instance.CurrencyValue.AddThemeColorOverride("font_color", Colors.Red);
                await Instance.ToSignal(Instance.CurrencyTimer, Timer.SignalName.Timeout);

                if (GodotObject.IsInstanceValid(Instance.CurrencyValue))
                {
                    Instance.CurrencyValue.RemoveThemeColorOverride("font_color");
                }
            }
            return;
        }

        Label baseLabel = amount > 0 ? Instance.AddCurrencyLabel : Instance.RemoveCurrencyLabel;
        if (!GodotObject.IsInstanceValid(baseLabel)) return;

        var label = (Label)baseLabel.Duplicate();
        label.Text = amount > 0 ? $"+{amount}" : amount.ToString();
        label.Visible = true;
        Instance.CurrencyNode.AddChild(label);

        Vector2 startPosition = label.Position;
        float duration = 1.0f;
        label.Modulate = Colors.White;

        var tween = label.CreateTween();
        tween.Parallel().TweenProperty(label, "position", startPosition + new Vector2(0, -50), duration);
        tween.Parallel().TweenProperty(label, "modulate", new Color(1, 1, 1, 0), duration)
            .SetEase(Tween.EaseType.In);

        await Instance.ToSignal(tween, Tween.SignalName.Finished);

        if (GodotObject.IsInstanceValid(label))
        {
            label.QueueFree();
        }
    }
}