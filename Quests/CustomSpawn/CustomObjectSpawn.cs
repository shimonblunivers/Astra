using Godot;
using Godot.Collections;

[GlobalClass]
public partial class CustomObjectSpawn : Resource
{
    [Export] public Array NpcPresets { get; set; } = new Array();
    [Export] public Array ItemPresets { get; set; } = new Array();

    public static CustomObjectSpawn Create(Array npcPresets = null, Array itemPresets = null)
    {
        var customObjectSpawn = new CustomObjectSpawn
        {
            NpcPresets = npcPresets ?? new Array(),
            ItemPresets = itemPresets ?? new Array()
        };

        return customObjectSpawn;
    }
}