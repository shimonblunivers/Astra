using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class Goal : Resource
{
    private dynamic N(string path) => GetNode(path);

    // TODO: class_name Goal

    public enum Type
    {
        go_to_place
        talk_to_npc
        pick_up_item
    }

    [Export] Type type;

    // # Is significant only if the type is "pick_up_item".
    [Export] string item_type = "Chip";

}