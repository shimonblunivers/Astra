using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class NPCSprite : Node2D
{
    private dynamic N(string path) => GetNode(path);


    public dynamic skin_node;
    // onready: skin_node = GetNode<Node>("Skin");
    public dynamic eyes_node;
    // onready: eyes_node = GetNode<Node>("Eyes");
    public dynamic hair_node;
    // onready: hair_node = GetNode<Node>("Hair");
    public dynamic torso_node;
    // onready: torso_node = GetNode<Node>("Torso");
    public dynamic legs_node;
    // onready: legs_node = GetNode<Node>("Legs");
    public dynamic boots_node;
    // onready: boots_node = GetNode<Node>("Boots");

    public Array skin = new Array();


    public void set_skin(dynamic a = _random_color(), dynamic b = _random_color(), dynamic c = _random_color(), dynamic d = _random_color(), dynamic e = _random_color())
    {
    skin_node.set_modulate(a);
    eyes_node.set_modulate(b);
    hair_node.set_modulate(c);
    torso_node.set_modulate(d);
    legs_node.set_modulate(e);
    boots_node.set_modulate(Color.BLACK);

    skin = [a, b, c, d, e];


    }

    public override async Task _Ready()
    {
    random := new RandomNumberGenerator();
    hair_node.Frame = random.randi_range(0, 6);
    hair_node.FlipH = random.randi_range(0, 1) == 0;
    set_skin();
    eyes_node.stop();
    N("Timer").set_wait_time(random.GD.RandRange(0, 2));
    N("Timer").start();
    await ToSignal(N("Timer"), Timer.SignalName.Timeout);
    eyes_node.play("default");

    }

    public Color _random_color()
    {
    random := new RandomNumberGenerator();
    return Color(random.randfn() * 0.75, random.randfn() * 0.75, random.randfn() * 0.75, 1)
    }

}