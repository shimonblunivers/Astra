using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class Tool : Resource
{
    private dynamic N(string path) => GetNode(path);


    [Export] string name;
    [Export] string nickname;
    [Export] Texture2D texture;

    [Export] int price = 100;

    [Export] bool rotatable = false;

    [Export] bool object = false;
    [Export] string spawn_tile_on_remove = "";

    [Export] int world_limit = -1;
    [Export] Vector2I placeable_on_atlas_choords = Vector2I(-1, -1);
    [Export] int terrain_id = -1;
    [Export] Vector2I atlas_coords = Vector2I(-1, -1) # -1 means not set;

    [Export] bool debug = false;

    public int number_of_instances = 0;


    public void create()
    {
    ShipEditor.tools[name] = this;

    }

}