using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class Floor : ShipPart
{
    private dynamic N(string path) => GetNode(path);



    public Sprite2D sprite;
    // onready: sprite = GetNode<Sprite2D>("Sprite2D");

    public int layer = 0;


    public void init(dynamic _ship, Vector2I _coords, float _durability = 100, float _mass = 4)
    {
    super(_ship, _coords, _durability, _mass);

    }

}