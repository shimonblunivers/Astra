using Godot;
using System;

[GlobalClass]
public partial class UIManager : CanvasLayer
{
    [Export] public RichTextLabel HealthLabel { get; set; }
    [Export] public RichTextLabel CurrencyLabel { get; set; }
    [Export] public Control DeathScreen { get; set; }
    [Export] public RichTextLabel QuestLabelNode { get; set; }
    [Export] public RichTextLabel MainStationLabelNode { get; set; }
    [Export] public Control Inventory { get; set; }

    [Export] public Control LoadingScreenNode { get; set; }
    [Export] public Timer LoadingScreenTimer { get; set; }
    [Export] public ProgressBar LoadingScreenBar { get; set; }
    [Export] public Control LoadingScreenBackground { get; set; }

    [Export] public Sprite2D QuestArrow { get; set; }
    [Export] public Label QuestArrowDistanceLabel { get; set; }

    [Export] public Control SavingScreenNode { get; set; }
    [Export] public Label AddCurrencyLabel { get; set; }
    [Export] public Label RemoveCurrencyLabel { get; set; }
    [Export] public Control CurrencyNode { get; set; }

    [Export] public CanvasItem FloatingDebug { get; set; }
    [Export] public Label PlayerPositionDebug { get; set; }

    [Export] public Vector2 InventoryOpenPosition { get; set; } = new Vector2(0, 0);
    [Export] public Vector2 InventoryClosedPosition { get; set; } = new Vector2(0, -500);

    public static RichTextLabel QuestLabel { get; set; }
    public static RichTextLabel MainStationLabel { get; set; }
    public static Label StaticAddCurrencyLabel { get; set; }
    public static Label StaticRemoveCurrencyLabel { get; set; }
    public static Control StaticCurrencyNode { get; set; }
    public static UIManager Instance { get; private set; }

    public static bool LoadingMute { get; set; } = false;

    public bool InventoryOpen { get; set; } = false;

    private bool _vfxMuted = false;
    private int _sfxBusIndex = -1;

    public override void _Ready()
    {
        Instance = this;

        HealthLabel ??= GetNodeOrNull<RichTextLabel>("HUD/HealthBar/Value");
        CurrencyLabel ??= GetNodeOrNull<RichTextLabel>("HUD/Currency/Value");
        DeathScreen ??= GetNodeOrNull<Control>("HUD/DeathScreen");
        QuestLabelNode ??= GetNodeOrNull<RichTextLabel>("HUD/Inventory/QuestLog/RichTextLabel");
        MainStationLabelNode ??= GetNodeOrNull<RichTextLabel>("HUD/Inventory/QuestLog/MainStationLabel");
        Inventory ??= GetNodeOrNull<Control>("HUD/Inventory");

        LoadingScreenNode ??= GetNodeOrNull<Control>("HUD/LoadingScreen");
        LoadingScreenTimer ??= GetNodeOrNull<Timer>("HUD/LoadingScreen/Timer");
        LoadingScreenBar ??= GetNodeOrNull<ProgressBar>("HUD/LoadingScreen/ProgressBar");
        LoadingScreenBackground ??= GetNodeOrNull<Control>("HUD/LoadingScreen/Background");

        QuestArrow ??= GetNodeOrNull<Sprite2D>("HUD/QuestArrow/Arrow");
        QuestArrowDistanceLabel ??= GetNodeOrNull<Label>("HUD/QuestArrow/Arrow/Distance");

        SavingScreenNode ??= GetNodeOrNull<Control>("HUD/SavingScreen");
        CurrencyNode ??= GetNodeOrNull<Control>("HUD/Currency");
        AddCurrencyLabel ??= GetNodeOrNull<Label>("HUD/Currency/AddCurrencyLabel");
        RemoveCurrencyLabel ??= GetNodeOrNull<Label>("HUD/Currency/RemoveCurrencyLabel");

        FloatingDebug ??= GetNodeOrNull<CanvasItem>("Debug/Floating");
        PlayerPositionDebug ??= GetNodeOrNull<Label>("Debug/PlayerPosition");

        QuestLabel = QuestLabelNode;
        MainStationLabel = MainStationLabelNode;
        StaticCurrencyNode = CurrencyNode;
        StaticAddCurrencyLabel = AddCurrencyLabel;
        StaticRemoveCurrencyLabel = RemoveCurrencyLabel;

        _sfxBusIndex = AudioServer.GetBusIndex("SFX");

        var options = Options.Instance ?? (Options)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("Options");
        bool devMode = options != null && options.DevelopmentMode;

        if (Player.MainPlayer != null)
        {
            if (HealthLabel != null) HealthLabel.Text = ((int)Player.MainPlayer.Health).ToString();
            if (CurrencyLabel != null) CurrencyLabel.Text = ((int)Player.MainPlayer.Currency).ToString();
            Player.MainPlayer.CurrencyUpdated += OnPlayerCurrencyUpdatedSignal;
        }

        if (LoadingScreenNode != null) LoadingScreenNode.Visible = !devMode;
        if (FloatingDebug != null) FloatingDebug.Visible = devMode;
        if (PlayerPositionDebug != null) PlayerPositionDebug.Visible = devMode;
    }

    public static async void CurrencyChangeEffect(int amount)
    {
        if (amount == 0 || StaticCurrencyNode == null) return;

        Label template = amount > 0 ? StaticAddCurrencyLabel : StaticRemoveCurrencyLabel;
        if (template == null) return;

        var label = (Label)template.Duplicate();
        label.Text = amount > 0 ? $"+{amount}" : amount.ToString();
        label.Visible = true;
        StaticCurrencyNode.AddChild(label);

        Vector2 startPosition = label.Position;
        label.Modulate = Colors.White;

        var tween = label.CreateTween();
        const float duration = 1.0f;
        tween.Parallel().TweenProperty(label, "position", startPosition + new Vector2(0, -50), duration);
        tween.Parallel().TweenProperty(label, "modulate", new Color(1, 1, 1, 0), duration).SetEase(Tween.EaseType.In);

        await label.ToSignal(tween, Tween.SignalName.Finished);
        label.QueueFree();
    }

    public void PlayerHealthUpdatedSignal()
    {
        if (Player.MainPlayer == null) return;

        if (HealthLabel != null)
        {
            HealthLabel.Text = ((int)Player.MainPlayer.Health).ToString();
        }

        if (DeathScreen != null)
        {
            bool alive = (bool)Player.MainPlayer.Get("alive");
            DeathScreen.Visible = !alive;
        }
    }

    public void OnPlayerCurrencyUpdatedSignal()
    {
        if (Player.MainPlayer != null && CurrencyLabel != null)
        {
            CurrencyLabel.Text = ((int)Player.MainPlayer.Currency).ToString();
        }
    }

    public async void LoadingScreen(float time = 1.6f)
    {
        var options = Options.Instance ?? (Options)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("Options");
        if (options != null && options.DevelopmentMode) return;

        if (LoadingScreenNode == null) return;

        LoadingScreenNode.Visible = true;

        if (_sfxBusIndex >= 0 && AudioServer.IsBusMute(_sfxBusIndex))
        {
            _vfxMuted = true;
        }
        else
        {
            _vfxMuted = false;
            LoadingMute = true;
        }

        LoadingScreenNode.Modulate = Colors.White;
        LoadingScreenTimer?.Start(time);

        if (LoadingScreenTimer != null)
        {
            await ToSignal(LoadingScreenTimer, Timer.SignalName.Timeout);
        }

        var tween = CreateTween();
        tween.TweenProperty(LoadingScreenNode, "modulate", new Color(1, 1, 1, 0), time / 2f);
        tween.Finished += ClearLoadingScreen;
    }

    private void ClearLoadingScreen()
    {
        if (LoadingScreenNode != null)
        {
            LoadingScreenNode.Visible = false;
        }

        if (!_vfxMuted)
        {
            LoadingMute = false;
        }
    }

    public void SavingScreen(float time = 1.6f)
    {
        if (SavingScreenNode == null) return;

        SavingScreenNode.Visible = true;
        SavingScreenNode.Modulate = Colors.White;

        var tween = CreateTween();
        tween.TweenProperty(SavingScreenNode, "modulate", Colors.White, time / 2f);
        tween.TweenProperty(SavingScreenNode, "modulate", new Color(1, 1, 1, 0), time / 2f);
        tween.Finished += ClearSavingScreen;
    }

    private void ClearSavingScreen()
    {
        if (SavingScreenNode != null)
        {
            SavingScreenNode.Visible = false;
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("game_toggle_inventory"))
        {
            InventoryOpen = !InventoryOpen;
            if (Inventory == null) return;

            var tween = CreateTween();
            const float duration = 0.5f;
            Vector2 targetPos = InventoryOpen ? InventoryOpenPosition : InventoryClosedPosition;
            var ease = InventoryOpen ? Tween.EaseType.Out : Tween.EaseType.In;

            tween.TweenProperty(Inventory, "position", targetPos, duration).SetEase(ease);
        }
    }

    public void OnQuestMetaClicked(Variant meta)
    {
        string metaStr = meta.AsString();
        var questManager = QuestManager.Instance ?? (QuestManager)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("QuestManager");

        if (metaStr.Contains("cancel"))
        {
            string idStr = metaStr.Replace("cancel", string.Empty).Trim('_', ' ', ':');
            if (int.TryParse(idStr, out int cancelId))
            {
                var quest = QuestManager.Instance?.GetQuest(cancelId);
                quest?.Delete();
            }

            if (questManager != null)
            {
                questManager.HighlightedQuestId = -1;
            }
        }
        else if (metaStr.Contains("main_ship"))
        {
            if (questManager != null)
            {
                questManager.HighlightedQuestId = -1;
                questManager.HighlightMainStation = !questManager.HighlightMainStation;
            }
        }
        else if (int.TryParse(metaStr, out int questId))
        {
            if (questManager != null)
            {
                if (questManager.HighlightedQuestId == questId)
                {
                    questManager.HighlightedQuestId = -1;
                    questManager.HighlightMainStation = false;
                }
                else
                {
                    questManager.HighlightedQuestId = questId;
                    questManager.HighlightMainStation = false;
                }
            }
        }

        QuestManager.Instance?.UpdateQuestLog();
    }

    public override void _Process(double delta)
    {
        if (_sfxBusIndex >= 0)
        {
            if (LoadingMute != AudioServer.IsBusMute(_sfxBusIndex))
            {
                AudioServer.SetBusMute(_sfxBusIndex, LoadingMute);
            }

            if (!LoadingMute && Player.MainPlayer != null)
            {
                bool isFloating = Player.MainPlayer.Floating();
                if (isFloating != AudioServer.IsBusMute(_sfxBusIndex))
                {
                    AudioServer.SetBusMute(_sfxBusIndex, isFloating);
                }
            }
        }

        var options = Options.Instance ?? (Options)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("Options");
        if (options != null && options.DevelopmentMode && Player.MainPlayer != null && World.Instance != null)
        {
            bool isFloating = Player.MainPlayer.Floating();
            if (FloatingDebug != null) FloatingDebug.Visible = isFloating;

            if (PlayerPositionDebug != null)
            {
                Vector2 pos = World.Instance.GetDistanceFromCenter(Player.MainPlayer.GlobalPosition);
                Vector2 cou = (Vector2)World.Instance.Get("_center_of_universe");
                PlayerPositionDebug.Text = $"PPU: X: {Mathf.Round(pos.X)}, Y: {Mathf.Round(pos.Y)}\nCOU: X: {Mathf.Round(cou.X)}, Y: {Mathf.Round(cou.Y)}\n";
            }
        }

        if (LoadingScreenTimer != null && LoadingScreenTimer.TimeLeft > 0)
        {
            float ratio = (float)((LoadingScreenTimer.WaitTime - LoadingScreenTimer.TimeLeft) / LoadingScreenTimer.WaitTime);

            if (LoadingScreenBar != null)
            {
                LoadingScreenBar.Value = (ratio - 0.042f) * (float)LoadingScreenBar.MaxValue;
            }

            if (LoadingScreenBackground != null)
            {
                float scale = 0.65f + ratio * 10f;
                LoadingScreenBackground.Scale = new Vector2(scale, scale);
                LoadingScreenBackground.Rotation = ratio * 2f;
            }
        }
    }
}