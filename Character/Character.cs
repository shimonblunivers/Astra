using Godot;
using System;

[GlobalClass]
public partial class Character : CharacterBody2D
{
    [Signal] public delegate void HealthUpdatedEventHandler();
    [Signal] public delegate void DiedEventHandler();

    [Export] public CollisionShape2D Legs { get; set; }
    [Export] public bool Godmode { get; set; } = false;

    public const float SPEED = 500.0f;
    public const float RUN_SPEED_MODIFIER = 0.0f;
    public const float TURN_SPEED = 1.0f;

    public float Speed { get; set; } = SPEED;
    public float RunSpeedModifier { get; set; } = RUN_SPEED_MODIFIER;
    public float TurnSpeed { get; set; } = TURN_SPEED;

    public Vector2 LegsOffset { get; set; } = Vector2.Zero;

    public float MaxHealth { get; set; } = 100.0f;
    public float Health { get; set; } = 100.0f;

    public bool Alive { get; set; } = false;
    public bool Spawned { get; set; } = false;

    public Vector2 SpawnPoint { get; set; } = Vector2.Zero;
    public string Nickname { get; set; } = string.Empty;
    public float MaxImpactVelocity { get; set; } = 40.0f;

    public override void _Ready()
    {
        Legs ??= GetNodeOrNull<CollisionShape2D>("LegHitbox");
        if (Legs != null)
        {
            LegsOffset = Legs.Position;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        // Fixed: Check Player.MainPlayer != null to avoid crash before player spawns
        if (Player.MainPlayer != null && this != Player.MainPlayer)
        {
            float dist = (GlobalPosition - Player.MainPlayer.GlobalPosition).Length();
            if (dist > Player.MainPlayer.UpdateRange)
            {
                return;
            }
        }

        _InPhysics(delta);
    }

    public virtual void _InPhysics(double delta)
    {
    }

    public void SetHealth(float amount)
    {
        Health = Mathf.Clamp(amount, 0f, MaxHealth);
        if (Health <= 0f)
        {
            Kill();
        }
        else
        {
            EmitSignal(SignalName.HealthUpdated);
        }
    }

    public void Damage(float amount)
    {
        if (Godmode || !Alive) return;

        Health = Mathf.Max(Health - amount, 0f);

        if (Health <= 0f)
        {
            Kill();
        }
        else
        {
            EmitSignal(SignalName.HealthUpdated);
        }
    }

    public virtual void Kill()
    {
        if (!Alive) return;

        Health = 0f;
        Alive = false;

        EmitSignal(SignalName.HealthUpdated);
        EmitSignal(SignalName.Died);
    }

    public virtual void Spawn()
    {
        Alive = true;
        Spawned = true;
        Health = MaxHealth;
        Position = SpawnPoint;
        EmitSignal(SignalName.HealthUpdated);
    }
}