using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class ItemType : Resource
{
    private dynamic N(string path) => GetNode(path);


    [Export] Texture2D texture;
    [Export] string name;
    [Export] string nickname;
    [Export] int worth = 0;
    [Export] float rarity = 1;
    [Export] free_spawn := true;
    [Export] dynamic shape = Shape2D;


    public void create()
    {
    Item.types[name] = this;
    }

}