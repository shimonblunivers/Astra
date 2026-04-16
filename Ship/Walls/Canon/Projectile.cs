using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class Projectile : CharacterBody2D
{
    private dynamic N(string path) => GetNode(path);


    [Export] int SPEED = 500;

    public float dir;
    public Vector2 spawn_position;
    public float spawn_rotation;
    public damage := 10;


    public override void _Ready()
    {
    global_position = spawn_position;
    global_rotation = spawn_rotation;

    }

    public override void _PhysicsProcess(double delta)
    {
    velocity = Vector2(0, -SPEED).rotated(dir);
    move_and_slide();


    }

    public void _on_area_2d_body_entered(Node2D body)
    {
    if (body.is_in_group("Wall"))
    {
        }
    body.get_parent().damage(damage);
    queue_free();
    }

}