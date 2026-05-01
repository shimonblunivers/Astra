using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class ItemSaveFile : Resource
{
    private dynamic N(string path) => GetNode(path);


    [Export] Vector2 position;
    [Export] int id;
    [Export] ItemType type;
    [Export] int ship_id;
    [Export] int ship_slot_id;


    public static void save()
    {
    Array files = new Array();

    foreach (var item in Item.items)
    {
        dynamic file = new ItemSaveFile();

        file.id = item.id;
        file.Position = item.GlobalPosition;
        file.type = item.type;
        file.ship_id = item.ship.id;
        file.ship_slot_id = item.ship_slot_id;

        files.append(file);

        return files

        }
    }

    public void load()
    {
    // pass
    }

}