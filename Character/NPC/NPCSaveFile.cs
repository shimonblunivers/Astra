using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class NPCSaveFile : Resource
{
    private dynamic N(string path) => GetNode(path);



    [Export] string nickname;
    [Export] Vector2 position;
    [Export] int id;
    [Export] Array roles = new Array();
    [Export] Array skin = new Array();
    [Export] Array hair = new Array();
    [Export] int ship_id;


    public static void save()
    {
    Array files = new Array();

    foreach (var npc in NPC.npcs)
    {
        }
    dynamic file = new NPCSaveFile();
    file.nickname = npc.nickname;
    file.Position = npc.Position;
    file.id = npc.id;
    if npc.roles == null: file.roles = NPC.Roles.CIVILIAN;
    }
    else: file.roles = npc.roles;
    file.skin = npc.sprites.skin;
    file.hair = [npc.sprites.hair_node.Frame, npc.sprites.hair_node.FlipH];
    file.ship_id = npc.ship.id;

    files.append(file);

    return files

    }

    public void load()
    {
    // pass
    }

}