using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class Debris : ShipPart
{
    private dynamic N(string path) => GetNode(path);




    public Sprite2D sprite;
    // onready: sprite = GetNode<Sprite2D>("Sprite2D");
    public AudioStreamPlayer2D spawn_sound;
    // onready: spawn_sound = GetNode<AudioStreamPlayer2D>("Sounds/Spawn");


    // Called when the node enters the scene tree for the first time.

    public void init(dynamic _ship, Vector2I _coords, float _durability = -1, float _mass = 4)
    {
    super(_ship, _coords, _durability, _mass);

    }

    public override void _Ready()
    {
    _random := new RandomNumberGenerator();
    sprite.RotationDegrees = 90 * _random.randi_range(0, 3);
    spawn_sound.PitchScale = GD.RandRange(0.9, 1.1);
    spawn_sound.play();


    }

}