using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class NPCPreset : Node
{
    private dynamic N(string path) => GetNode(path);

    // TODO: class_name NPCPreset

    public int id;
    public string nickname;
    public Array roles = new Array();

    public dynamic colors;
    public dynamic hair;


    public void _init(int _id, string _nickname, Array _roles = new Array(), dynamic _colors = null, dynamic _hair = null)
    {
    id = _id;
    nickname = _nickname;
    roles = _roles;
    colors = _colors;
    hair = _hair;
    }

}