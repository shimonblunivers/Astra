using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class Canon : Node2D
{
    private dynamic N(string path) => GetNode(path);


    dynamic main = get_tree().get_root();
    dynamic projectile_scene = GD.Load<PackedScene>("res://Ship/Walls/Canon/Projectile.tscn");


    public override void _Ready()
    {
    // pass

    }

    public override void _PhysicsProcess(double delta)
    {
    rotation_degrees += 1;

    }

    public void shoot()
    {
    dynamic instance = projectile_scene.Instantiate();
    instance.dir = global_rotation;
    instance.spawn_position = global_position;
    instance.spawn_rotation = Mathf.DegToRad(global_rotation_degrees - 90);
    main.add_child.CallDeferred(instance);

    }

    public override void _Input(InputEvent @event)
    {
    if (event is InputEventMouseButton)
    {
        }
    shoot();
    }

}