using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ItemSaveFile : Resource
{
    [Export] public Vector2 Position { get; set; }
    [Export] public int Id { get; set; }
    [Export] public ItemType Type { get; set; }
    [Export] public int ShipId { get; set; } = -1;
    [Export] public int ShipSlotId { get; set; } = -1;

    public static Array<ItemSaveFile> Save()
    {
        var files = new Array<ItemSaveFile>();

        foreach (var item in Item.Items)
        {
            if (!GodotObject.IsInstanceValid(item)) continue;

            var file = new ItemSaveFile
            {
                Id = item.Id,
                Position = item.GlobalPosition,
                Type = item.Type,
                // Fixed: Check item.Ship to prevent crash if an item has no ship
                ShipId = item.Ship != null ? (int)item.Ship.Get("id") : -1,
                ShipSlotId = item.ShipSlotId
            };

            files.Add(file);
        }

        return files;
    }

    public void Load()
    {
        Ship targetShip = null;
        if (ShipId != -1)
        {
            var shipScript = GD.Load<GDScript>("res://Ships/Ship.gd");
            if (shipScript != null)
            {
                targetShip = (Ship)shipScript.Call("get_ship", ShipId);
            }
        }

        // Recreate the item using Item.Spawn
        Item.Spawn(Type, Position, Id, targetShip, ShipSlotId);
    }
}