using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class Character : CharacterBody2D
{
    private dynamic N(string path) => GetNode(path);



    dynamic speed = Vector2.Zero;

    public dynamic legs;
    // onready: legs = GetNode<Node>("LegHitbox");

    [Export] bool godmode = false;

    public static readonly dynamic SPEED = 500.0;
    public static readonly dynamic RUN_SPEED_MODIFIER = 0.0;
    public static readonly dynamic TURN_SPEED = 1.0;

    public dynamic legs_offset = Vector2.Zero;

    public float max_health = 100;
    public  health = max_health;
    [Signal] public delegate void HealthUpdatedSignalEventHandler();
    [Signal] public delegate void DiedSignalEventHandler();
    public bool alive = false;
    public bool spawned = false;

    public Vector2 spawn_point = Vector2.Zero;

    public string nickname = "";

    public float max_impact_velocity = 40;


    public override void _Ready()
    {
    legs_offset = legs.Position;


    }

    public override void _PhysicsProcess(double delta)
    {
    if (global_position - Player.main_player.GlobalPosition).length() > Player.main_player.update_range: return;
    {
        }
    _in_physics(delta);

    }

    public void _in_physics(float _delta)
    {
    // pass


    }

    public void set_health(float amount)
    {
    health = amount;
    damage(0);

    }

    public void damage(float amount)
    {
    if godmode: return;
    health = max(health - amount, 0);
    if health == 0: kill();
    }
    else: health_updated_signal.emit();


    }

    public void kill()
    {
    if !alive: return;
    health = 0;
    alive = false;

    health_updated_signal.emit();
    died_signal.emit();


    }

    public void spawn()
    {
    alive = true;
    spawned = true;
    health = max_health;
    position = spawn_point;
    health_updated_signal.emit();
    }

}