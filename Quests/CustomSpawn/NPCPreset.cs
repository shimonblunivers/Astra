using Godot;
using Godot.Collections;

[GlobalClass]
public partial class NPCPreset : RefCounted
{
    [Export] public int Id { get; set; }
    [Export] public string Nickname { get; set; } = string.Empty;
    [Export] public Array Roles { get; set; } = new Array();

    [Export] public Variant Colors { get; set; }
    [Export] public Variant Hair { get; set; }

    public NPCPreset()
    {
    }

    public NPCPreset(int id, string nickname, Array roles = null, Variant colors = default, Variant hair = default)
    {
        Id = id;
        Nickname = nickname ?? string.Empty;
        Roles = roles ?? new Array();
        Colors = colors;
        Hair = hair;
    }
}