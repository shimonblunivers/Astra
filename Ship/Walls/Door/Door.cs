using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class Door : InteractableShipPart
{
    public enum DoorState
    {
        Closed,
        Open
    }

    [Export] public StaticBody2D Walkway { get; set; }
    [Export] public AudioStreamPlayer2D OpenSound { get; set; }
    [Export] public AudioStreamPlayer2D CloseSound { get; set; }
    [Export] public AnimatedSprite2D AnimatedSprite { get; set; }
    [Export] public Timer AutocloseTimer { get; set; }

    [Export] public Node2D MouseHitbox { get; set; }
    [Export] public Area2D DoorArea { get; set; }

    [Export] public int CollisionLayer { get; set; } = 1;
    [Export] public int OccluderLightMask { get; set; } = 1;
    [Export] public float InteractRange { get; set; } = 300f;

    public DoorState State { get; set; } = DoorState.Closed;
    public bool Obstructed { get; set; } = false;
    public bool Locked { get; set; } = false;
    public bool IsOperating { get; set; } = false;

    // Backward-compatibility string helper for GDScript
    public string state
    {
        get => State == DoorState.Open ? "open" : "closed";
        set => State = value == "open" ? DoorState.Open : DoorState.Closed;
    }

    private readonly List<Area2D> _obstructers = new List<Area2D>();

    // Cached animated occluders and collision bodies
    private LightOccluder2D _occ0Left;
    private LightOccluder2D _occ0Right;
    private LightOccluder2D _occ1Left;
    private LightOccluder2D _occ1Right;
    private LightOccluder2D _occ2Center;

    private CollisionObject2D _hit0Left;
    private CollisionObject2D _hit0Right;
    private CollisionObject2D _hit1Left;
    private CollisionObject2D _hit1Right;

    public override void Init(Ship targetShip, Vector2I coords, float durability = 100f, float mass = 3f)
    {
        var interactables = (Godot.Collections.Array)targetShip?.Get("interactables");
        interactables?.Add(this);

        base.Init(targetShip, coords, durability, mass);
    }

    public override void _Ready()
    {
        base._Ready();

        Walkway ??= GetNodeOrNull<StaticBody2D>("Hitbox/StaticBody2DWalkway");
        OpenSound ??= GetNodeOrNull<AudioStreamPlayer2D>("Sound/DoorOpen");
        CloseSound ??= GetNodeOrNull<AudioStreamPlayer2D>("Sound/DoorClose");
        AnimatedSprite ??= GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
        AutocloseTimer ??= GetNodeOrNull<Timer>("AutocloseTimer");

        MouseHitbox ??= GetNodeOrNull<Node2D>("Hitbox/Area/MouseHitbox");
        DoorArea ??= GetNodeOrNull<Area2D>("Hitbox/Area/Area2D");

        // Cache occluders
        _occ0Left = GetNodeOrNull<LightOccluder2D>("Hitbox/AnimatedOccluders/0left");
        _occ0Right = GetNodeOrNull<LightOccluder2D>("Hitbox/AnimatedOccluders/0right");
        _occ1Left = GetNodeOrNull<LightOccluder2D>("Hitbox/AnimatedOccluders/1left");
        _occ1Right = GetNodeOrNull<LightOccluder2D>("Hitbox/AnimatedOccluders/1right");
        _occ2Center = GetNodeOrNull<LightOccluder2D>("Hitbox/AnimatedOccluders/2center");

        // Cache hitboxes
        _hit0Left = GetNodeOrNull<CollisionObject2D>("Hitbox/AnimatedHitbox/0left");
        _hit0Right = GetNodeOrNull<CollisionObject2D>("Hitbox/AnimatedHitbox/0right");
        _hit1Left = GetNodeOrNull<CollisionObject2D>("Hitbox/AnimatedHitbox/1left");
        _hit1Right = GetNodeOrNull<CollisionObject2D>("Hitbox/AnimatedHitbox/1right");

        if (Direction == "vertical")
        {
            RotationDegrees = 90f;
        }

        if (AnimatedSprite != null)
        {
            AnimatedSprite.FrameChanged += OnFrameChanged;
        }

        if (MouseHitbox != null)
        {
            HitboxesToShift.Add(MouseHitbox);
        }
    }

    public void UpdateSprites()
    {
        if (AnimatedSprite == null) return;

        if (State == DoorState.Open)
        {
            AnimatedSprite.Play("open");
        }
        else
        {
            AnimatedSprite.PlayBackwards("open");
        }
    }

    public void Open()
    {
        if (Locked) return;

        IsOperating = true;
        State = DoorState.Open;

        if (OpenSound != null)
        {
            OpenSound.PitchScale = (float)GD.RandRange(0.9, 1.1);
            OpenSound.Play();
        }

        UpdateSprites();

        var openedDoors = (Godot.Collections.Array)Ship?.Get("opened_doors");
        var tilemapCoords = (Vector2I)Get("tilemap_coords");
        if (openedDoors != null && !openedDoors.Contains(tilemapCoords))
        {
            openedDoors.Add(tilemapCoords);
        }

        AutocloseTimer?.Start();
    }

    public void Close()
    {
        IsOperating = true;
        State = DoorState.Closed;

        if (CloseSound != null)
        {
            CloseSound.PitchScale = (float)GD.RandRange(0.9, 1.1);
            CloseSound.Play();
        }

        UpdateSprites();

        Walkway?.SetCollisionLayerValue(CollisionLayer, true);

        var openedDoors = (Godot.Collections.Array)Ship?.Get("opened_doors");
        var tilemapCoords = (Vector2I)Get("tilemap_coords");
        openedDoors?.Remove(tilemapCoords);
    }

    private void OnFrameChanged()
    {
        if (AnimatedSprite == null) return;

        switch (AnimatedSprite.Frame)
        {
            case 3: // OPEN
                IsOperating = State != DoorState.Open;

                SetOccluderMask(_occ0Left, 0);
                SetOccluderMask(_occ0Right, 0);
                SetOccluderMask(_occ1Left, 0);
                SetOccluderMask(_occ1Right, 0);
                SetOccluderMask(_occ2Center, 0);

                SetCollisionLayerActive(_hit0Left, false);
                SetCollisionLayerActive(_hit0Right, false);
                SetCollisionLayerActive(_hit1Left, false);
                SetCollisionLayerActive(_hit1Right, false);
                break;

            case 2:
                SetOccluderMask(_occ0Left, OccluderLightMask);
                SetOccluderMask(_occ0Right, OccluderLightMask);
                SetOccluderMask(_occ1Left, 0);
                SetOccluderMask(_occ1Right, 0);
                SetOccluderMask(_occ2Center, 0);

                if (State == DoorState.Open)
                {
                    Walkway?.SetCollisionLayerValue(CollisionLayer, false);
                    SetCollisionLayerActive(_hit0Left, true);
                    SetCollisionLayerActive(_hit0Right, true);
                    SetCollisionLayerActive(_hit1Left, false);
                    SetCollisionLayerActive(_hit1Right, false);
                }
                break;

            case 1:
                SetOccluderMask(_occ0Left, OccluderLightMask);
                SetOccluderMask(_occ0Right, OccluderLightMask);
                SetOccluderMask(_occ1Left, OccluderLightMask);
                SetOccluderMask(_occ1Right, OccluderLightMask);
                SetOccluderMask(_occ2Center, 0);

                if (State == DoorState.Open)
                {
                    SetCollisionLayerActive(_hit0Left, true);
                    SetCollisionLayerActive(_hit0Right, true);
                    SetCollisionLayerActive(_hit1Left, true);
                    SetCollisionLayerActive(_hit1Right, true);
                }
                break;

            case 0: // CLOSED
                SetOccluderMask(_occ0Left, OccluderLightMask);
                SetOccluderMask(_occ0Right, OccluderLightMask);
                SetOccluderMask(_occ1Left, OccluderLightMask);
                SetOccluderMask(_occ1Right, OccluderLightMask);
                SetOccluderMask(_occ2Center, OccluderLightMask);

                IsOperating = State != DoorState.Closed;
                break;
        }
    }

    protected override void OnInteract()
    {
        if (Player.MainPlayer == null || Player.MainPlayer.GlobalPosition.DistanceTo(GlobalPosition) > InteractRange)
        {
            return;
        }

        if (IsOperating) return;

        if (State == DoorState.Open)
        {
            if (Obstructed) return;
            Close();
        }
        else
        {
            Open();
        }
    }

    public void OnArea2DAreaEntered(Area2D area)
    {
        if (area != null && area.IsInGroup("CharacterInteractArea"))
        {
            Obstructed = true;
            if (!_obstructers.Contains(area))
            {
                _obstructers.Add(area);
            }
        }
    }

    public void OnArea2DAreaExited(Area2D area)
    {
        if (area != null && area.IsInGroup("CharacterInteractArea"))
        {
            _obstructers.Remove(area);
            if (_obstructers.Count == 0)
            {
                Obstructed = false;
            }
        }
    }

    public void OnMouseHitboxInputEvent(Node viewport, InputEvent @event, int shapeIdx)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
        {
            bool playerAlive = Player.MainPlayer != null && (bool)Player.MainPlayer.Get("alive");
            if (playerAlive)
            {
                Interact();
            }
        }
    }

    public void OnAutocloseTimerTimeout()
    {
        if (State == DoorState.Open)
        {
            if (!Obstructed)
            {
                Close();
            }
            else
            {
                AutocloseTimer?.Start();
            }
        }
    }

    private static void SetOccluderMask(LightOccluder2D occluder, int mask)
    {
        if (occluder != null)
        {
            occluder.OccluderLightMask = mask;
        }
    }

    private void SetCollisionLayerActive(CollisionObject2D body, bool active)
    {
        body?.SetCollisionLayerValue(CollisionLayer, active);
    }
}