using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class CustomObjectSpawn : Resource
{
    private dynamic N(string path) => GetNode(path);


    public npc_presets := [];
    public item_presets := [];


    public static CustomObjectSpawn create(Array _npc_presets = new Array(), Array _item_presets = new Array())
    {
    CustomObjectSpawn custom_object_spawn = new CustomObjectSpawn();
    custom_object_spawn.npc_presets = _npc_presets;
    custom_object_spawn.item_presets = _item_presets;
    return custom_object_spawn
    }

}