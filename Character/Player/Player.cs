using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class Player : Character
{
    [Signal] public delegate void CurrencyUpdatedEventHandler();
    [Signal] public delegate void CurrencyUpdatedSignalEventHandler();

    public static Player MainPlayer { get; private set; }

    [Export] public AnimatedSprite2D AnimatedSprite { get; set; }
    [Export] public AudioStreamPlayer2D WalkSound { get; set; }
    [Export] public Camera2D Camera { get; set; }
    [Export] public PointLight2D Vision { get; set; }
    [Export] public Area2D InteractArea { get; set; }
    [Export] public Timer RespawnTimer { get; set; }
    [Export] public Node2D Pickup { get; set; }
    [Export] public Timer InvincibilityTimer { get; set; }
    [Export] public Timer LockRotationTimer { get; set; }

    private float _currency;
    public float Currency
    {
        get => _currency;
        set
        {
            _currency = value;
            EmitSignal(SignalName.CurrencyUpdated);
            EmitSignal(SignalName.CurrencyUpdatedSignal);
        }
    }

    public float currency
    {
        get => Currency;
        set => Currency = value;
    }

    private int _spriteDir = 69;
    public Ship ShipControlled { get; private set; } = null;

    [Export] public float NormalZoom { get; set; } = 1.0f;
    [Export] public float NormalVision { get; set; } = 4.0f;
    [Export] public float DrivingVision { get; set; } = 0.4f;

    [Export] public bool Suit { get; set; } = true;
    [Export] public float UseRange { get; set; } = 1000f;

    public Vector2 Acceleration { get; set; } = Vector2.Zero;
    private Vector2 _oldPosition = Vector2.Zero;

    public List<Node> HoveringControllables { get; } = new List<Node>();
    public List<Node> ControllablesInUse { get; } = new List<Node>();
    public List<Ship> PassengerOn { get; } = new List<Ship>();

    public Ship ParentShip { get; private set; } = null;
    public int DimAccelerationForFrames { get; set; } = 0;
    public Vector2 CameraDifference { get; set; } = Vector2.Zero;

    private float _damageTimer = 0f;
    private float _regenTimer = 0f;

    private Ship _ownedShip;
    public Ship OwnedShip
    {
        get => _ownedShip;
        set
        {
            if (value != null)
            {
                value.LinearDamp = 0;
            }
            _ownedShip = value;
        }
    }

    public Ship owned_ship
    {
        get => OwnedShip;
        set => OwnedShip = value;
    }

    public bool Invincible { get; set; } = false;
    private bool _lockedRotating = false;
    [Export] public float UpdateRange { get; set; } = 10000f;

    public float update_range
    {
        get => UpdateRange;
        set => UpdateRange = value;
    }

    public Ship parent_ship
    {
        get => ParentShip;
        set => ParentShip = value;
    }

    private Tween _turnTween;

    public override void _Ready()
    {
        base._Ready();
        MainPlayer = this;

        AnimatedSprite ??= GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
        WalkSound ??= GetNodeOrNull<AudioStreamPlayer2D>("Sounds/Walk");
        Camera ??= GetNodeOrNull<Camera2D>("Camera2D");
        Vision ??= GetNodeOrNull<PointLight2D>("Vision/Light");
        InteractArea ??= GetNodeOrNull<Area2D>("InteractArea");
        RespawnTimer ??= GetNodeOrNull<Timer>("RespawnTimer");
        Pickup ??= GetNodeOrNull<Node2D>("Pickup");
        InvincibilityTimer ??= GetNodeOrNull<Timer>("InvincibilityTimer");
        LockRotationTimer ??= GetNodeOrNull<Timer>("LockRotationTimer");

        SpawnPoint = new Vector2(7777, -69);
        Nickname = "Samuel";

        WaitForProcessFrameAndInit();
    }

    private async void WaitForProcessFrameAndInit()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        AnimatedSprite?.Play("Idle");

        if (World.SaveFile != null)
        {
            World.SaveFile.LoadWorld();
        }
        else if (World.Instance != null)
        {
            var saveFile = World.Instance.Get("save_file").AsGodotObject();
            saveFile?.Call("load_world");
        }
    }

    public void AddCurrency(int amount)
    {
        Currency += amount;
        UIManager.CurrencyChangeEffect(amount);
    }

    public bool Floating() => PassengerOn.Count == 0;
    public bool floating() => Floating();

    public void GetIn(Ship ship)
    {
        if (ship == null) return;
        DimAccelerationForFrames = 5;
        if (PassengerOn.Contains(ship)) return;

        PassengerOn.Add(ship);
        if (PassengerOn.Count == 1) RotateToShip();

        if (MaxImpactVelocity < (Acceleration - ship.DifferenceInPosition).Length())
        {
            Kill();
        }
    }

    public void RotateToShip()
    {
        if (_lockedRotating) return;
        if (_turnTween != null && _turnTween.IsValid()) _turnTween.Kill();

        float currentRot = Mathf.PosMod(RotationDegrees, 360f);
        float targetRot = currentRot > 180f ? 360f : 0f;
        float turnSpeed = Mathf.Max(0.01f, Mathf.Abs(RotationDegrees) / 150f);

        _turnTween = CreateTween();
        _turnTween.TweenProperty(this, "rotation_degrees", targetRot, turnSpeed);
    }

    public void GetOff(Ship ship)
    {
        if (_turnTween != null && _turnTween.IsValid()) _turnTween.Kill();
        PassengerOn.Remove(ship);
    }

    public void ChangeShip(Ship ship)
    {
        if (ship == null) return;
        ParentShip = ship;

        var passengersNode = ship.PassengersNode ?? (Node)ship.Get("passengers_node");
        if (passengersNode != null)
        {
            CallDeferred(Node.MethodName.Reparent, passengersNode);
        }

        if (!Floating())
        {
            Callable.From(ChangeShipRotate).CallDeferred();
        }
    }

    private void ChangeShipRotate()
    {
        RotateToShip();
        _lockedRotating = true;
        LockRotationTimer?.Start();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        var options = Options.Instance ?? (Options)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("Options");
        bool devMode = options != null && options.DevelopmentMode;

        if (devMode)
        {
            if (@event.IsActionPressed("teleport_to_quest"))
            {
                var questMgr = QuestManager.Instance ?? (QuestManager)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("QuestManager");
                int highlightedId = questMgr != null ? questMgr.HighlightedQuestId : -1;
                var quest = QuestManager.Instance?.GetQuest(highlightedId);

                if (quest != null)
                {
                    var target = (Node2D)quest.Call("get_target");
                    if (target != null)
                    {
                        Godmode = true;
                        Teleport(target.GlobalPosition);
                    }
                }
                else if (ShipManager.MainStation != null)
                {
                    Godmode = true;
                    Teleport(ShipManager.MainStation.GlobalPosition);
                }
            }

            if (@event.IsActionPressed("teleport_to_ship_editor"))
            {
                Teleport(new Vector2(3300, -3100));
            }

            if (@event.IsActionPressed("debug_die"))
            {
                if (World.SaveFile != null)
                {
                    World.SaveFile.SaveWorld(true);
                }
                else if (World.Instance != null)
                {
                    var saveFile = World.Instance.Get("save_file").AsGodotObject();
                    saveFile?.Call("save_world", true);
                }
            }

            if (@event.IsActionPressed("debug_spawn"))
            {
                AddCurrency(1500);
            }
        }

        if (@event.IsActionPressed("development_mode") && options != null)
        {
            options.DevelopmentMode = !options.DevelopmentMode;
            devMode = options.DevelopmentMode;

            var uiMgr = UIManager.Instance;
            if (uiMgr != null)
            {
                if (uiMgr.LoadingScreenNode != null) uiMgr.LoadingScreenNode.Visible = !devMode;
                if (uiMgr.FloatingDebug != null) uiMgr.FloatingDebug.Visible = devMode;
                if (uiMgr.PlayerPositionDebug != null) uiMgr.PlayerPositionDebug.Visible = devMode;
            }
        }

        if (Alive && @event.IsActionPressed("game_control"))
        {
            foreach (var controllable in HoveringControllables)
            {
                if (GodotObject.IsInstanceValid(controllable))
                {
                    controllable.Call("interact");
                    return;
                }
            }

            foreach (var controllable in ControllablesInUse)
            {
                if (HoveringControllables.Contains(controllable)) continue;
                if (GodotObject.IsInstanceValid(controllable))
                {
                    controllable.Set("player_in_range", this);
                    controllable.Call("interact");
                    controllable.Set("player_in_range", new Variant());
                    return;
                }
            }
        }
    }

    public void Teleport(Vector2 pos)
    {
        GlobalPosition = pos;
        if (World.Instance != null)
        {
            _oldPosition = World.Instance.GetDistanceFromCenter(GlobalPosition);
        }
    }

    public void Spawn(Vector2? pos = null, Vector2? newAcceleration = null, float? newRotation = null)
    {
        Vector2 targetPos = pos ?? SpawnPoint;

        if (!Alive) AnimatedSprite?.Play("Idle");
        Alive = true;
        Spawned = true;
        Health = MaxHealth;

        if (newRotation.HasValue)
        {
            GlobalRotation = newRotation.Value;
        }

        var objectList = ObjectList.Instance ?? (ObjectList)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("ObjectList");
        if (objectList != null)
        {
            ChangeShip((Ship)objectList.Call("get_closest_ship", GlobalPosition));
        }

        _lockedRotating = false;
        Vector2 universeCenter = World.Instance != null ? (Vector2)World.Instance.Get("_center_of_universe") : Vector2.Zero;
        GlobalPosition = targetPos - universeCenter;

        if (World.Instance != null)
        {
            _oldPosition = World.Instance.GetDistanceFromCenter(GlobalPosition);
        }

        Invincible = true;
        InvincibilityTimer?.Start();
        EmitSignal(Character.SignalName.HealthUpdated);
        UIManager.Instance?.PlayerHealthUpdatedSignal();
    }

    public void spawn(Vector2 position = default, Vector2 velocity = default, float rotation = 0f)
    {
        Spawn(position == default ? null : position, velocity, rotation);
    }

    public override void Kill()
    {
        if (!Alive || !Spawned || Invincible || Godmode) return;

        Health = 0;
        Alive = false;

        if (ShipControlled != null)
        {
            ControlShip(null);
        }

        AnimatedSprite?.Play("Death");
        EmitSignal(Character.SignalName.HealthUpdated);
        EmitSignal(Character.SignalName.Died);
        UIManager.Instance?.PlayerHealthUpdatedSignal();
        RespawnTimer?.Start();
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;
        if (Alive)
        {
            if (Floating())
            {
                _regenTimer = 0;
                _damageTimer += dt * 2;
                if (_damageTimer >= 1.0f)
                {
                    _damageTimer = 0;
                    Damage(5);
                }
            }
            else
            {
                _damageTimer = 0;
                if (Health != MaxHealth)
                {
                    _regenTimer += dt * 3;
                    if (_regenTimer >= 1.0f)
                    {
                        Damage(-1);
                        _regenTimer = 0;
                    }
                }
            }
        }

        if (Pickup != null)
        {
            Pickup.Position = (-Acceleration).Rotated(-(float)GlobalRotation);
        }
    }

    public override void _InPhysics(double delta)
    {
        float dt = (float)delta;

        if (PassengerOn.Count == 1 && PassengerOn[0] != ParentShip)
        {
            ChangeShip(PassengerOn[0]);
        }

        if (ShipControlled == null)
        {
            MovePlayer(dt);
        }
        else if (Camera != null)
        {
            Camera.Offset = CameraDifference.Rotated((float)GlobalRotation);
        }
    }

    public void ControlShip(Ship ship)
    {
        if (ship != null)
        {
            ShipControlled = ship;
            WalkSound?.Stop();
            _spriteDir = 0;
            if (AnimatedSprite != null)
            {
                AnimatedSprite.FlipH = false;
                AnimatedSprite.Play("Idle");
            }

            ship.StartControlling(this);
            ChangeView(0);
        }
        else
        {
            ChangeView(1);
            if (ShipControlled != null)
            {
                ShipControlled.StopControlling();
            }
            ShipControlled = null;
        }
    }

    public void control_ship(Ship ship) => ControlShip(ship);

    private void MovePlayer(float delta)
    {
        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        float rotationDirection = Input.GetAxis("game_turn_left", "game_turn_right");
        float running = Input.GetActionStrength("game_run");

        if (!Alive)
        {
            direction = Vector2.Zero;
            rotationDirection = 0;
        }

        Vector2 currentDist = World.Instance != null ? World.Instance.GetDistanceFromCenter(GlobalPosition) : GlobalPosition;
        Acceleration = currentDist - _oldPosition;
        _oldPosition = currentDist;

        Velocity = (direction * (Speed + RunSpeedModifier * running)).Rotated((float)GlobalRotation);

        if (ParentShip != null)
        {
            if (Floating())
            {
                Rotate(Mathf.DegToRad(TurnSpeed * rotationDirection));
                if (Legs != null)
                {
                    Legs.Position = LegsOffset - Acceleration.Rotated(-(float)GlobalRotation);
                }

                if (!Suit)
                {
                    Velocity = Vector2.Zero;
                }
                else
                {
                    Velocity *= 0.01f;
                }

                if (DimAccelerationForFrames <= 0)
                {
                    Velocity += (Acceleration - ParentShip.DifferenceInPosition) / delta;
                }
                else
                {
                    const float dimFactor = 10.0f;
                    Velocity += (Acceleration - ParentShip.DifferenceInPosition) / (delta * dimFactor);
                }
            }
            else if (Legs != null && PassengerOn.Count > 0 && PassengerOn[0] != null)
            {
                Legs.Position = LegsOffset - PassengerOn[0].DifferenceInPosition.Rotated(-(float)GlobalRotation);
            }
        }

        if (DimAccelerationForFrames > 0)
        {
            DimAccelerationForFrames--;
        }

        MoveAndSlide();

        if (!Floating() && direction != Vector2.Zero)
        {
            if (WalkSound != null && !WalkSound.Playing)
            {
                WalkSound.PitchScale = (float)GD.RandRange(0.9, 1.1);
                WalkSound.Play();
            }

            if (AnimatedSprite != null)
            {
                if (direction.X < 0)
                {
                    if (_spriteDir != 1)
                    {
                        _spriteDir = 1;
                        AnimatedSprite.FlipH = true;
                        AnimatedSprite.Play("WalkToSide");
                    }
                }
                else if (direction.X > 0)
                {
                    if (_spriteDir != 2)
                    {
                        _spriteDir = 2;
                        AnimatedSprite.FlipH = false;
                        AnimatedSprite.Play("WalkToSide");
                    }
                }
                else if (direction.Y > 0)
                {
                    if (_spriteDir != 3)
                    {
                        _spriteDir = 3;
                        AnimatedSprite.FlipH = false;
                        AnimatedSprite.Play("WalkDown");
                    }
                }
                else if (direction.Y < 0)
                {
                    if (_spriteDir != 4)
                    {
                        _spriteDir = 4;
                        AnimatedSprite.FlipH = false;
                        AnimatedSprite.Play("WalkUp");
                    }
                }
            }
        }
        else if (Alive && _spriteDir != 0)
        {
            _spriteDir = 0;
            if (AnimatedSprite != null)
            {
                AnimatedSprite.FlipH = false;
                AnimatedSprite.Play("Idle");
            }
        }
    }

    public void ChangeView(int view)
    {
        if (Camera == null) return;

        var tween = CreateTween();
        const float duration = 1.0f;

        switch (view)
        {
            case 0:
                if (ShipControlled != null)
                {
                    Rect2 baseRect = ShipControlled.GetRect();
                    Vector2 tileSize = ShipControlled.GetTileSize();

                    Rect2 shipRect = new Rect2(
                        baseRect.Position.X * tileSize.X * 5,
                        baseRect.Position.Y * tileSize.Y * 5,
                        baseRect.Size.X * tileSize.X * 5,
                        baseRect.Size.Y * tileSize.Y * 5
                    );

                    Vector2 shipCenter = shipRect.Size / 2f + shipRect.Position;
                    CameraDifference = shipCenter - Position;

                    float shipSize = (Mathf.Max(shipRect.Size.X, shipRect.Size.Y) + 2000f) * 1.666f;
                    float camSize = GetViewportRect().Size.Y * 1.25f;
                    float shipZoom = 1.0f / (shipSize / camSize);

                    tween.Parallel().TweenProperty(Camera, "zoom", new Vector2(shipZoom, shipZoom), duration)
                        .SetEase(Tween.EaseType.Out);
                    tween.Parallel().TweenProperty(Camera, "offset", CameraDifference.Rotated((float)GlobalRotation), duration)
                        .SetEase(Tween.EaseType.Out);

                    if (Vision != null)
                    {
                        tween.Parallel().TweenProperty(Vision, "texture_scale", DrivingVision, duration)
                            .SetEase(Tween.EaseType.Out);
                    }
                }
                break;

            case 1:
                CameraDifference = Vector2.Zero;
                tween.Parallel().TweenProperty(Camera, "zoom", new Vector2(NormalZoom, NormalZoom), duration)
                    .SetEase(Tween.EaseType.Out);
                tween.Parallel().TweenProperty(Camera, "offset", CameraDifference, duration)
                    .SetEase(Tween.EaseType.Out);

                if (Vision != null)
                {
                    tween.Parallel().TweenProperty(Vision, "texture_scale", NormalVision, duration)
                        .SetEase(Tween.EaseType.Out);
                }
                break;
        }
    }

    public void OnPickupAreaEntered(Area2D area)
    {
        if (area != null && !area.IsInGroup("CharacterInteractArea"))
        {
            area.GetParent()?.Set("can_pickup", true);
        }
    }

    public void OnPickupAreaExited(Area2D area)
    {
        if (area != null && !area.IsInGroup("CharacterInteractArea"))
        {
            area.GetParent()?.Set("can_pickup", false);
        }
    }

    public void DeletingShip(Ship ship)
    {
        if (ship == ParentShip && World.Instance != null)
        {
            CallDeferred(Node.MethodName.Reparent, World.Instance);
        }
    }

    public void OnHealthUpdatedSignal()
    {
        UIManager.Instance?.PlayerHealthUpdatedSignal();
    }

    public void OnRespawnTimerTimeout()
    {
        if (World.SaveFile != null)
        {
            World.SaveFile.LoadWorld();
        }
        else if (World.Instance != null)
        {
            var saveFile = World.Instance.Get("save_file").AsGodotObject();
            saveFile?.Call("load_world");
        }
    }

    public void OnInvincibilityTimerTimeout()
    {
        Invincible = false;
    }

    public void OnLockRotationTimerTimeout()
    {
        _lockedRotating = false;
    }
}