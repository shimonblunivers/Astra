using Godot;

[GlobalClass]
public partial class ItemPreset : RefCounted
{
    [Export] public int Id { get; set; }
    [Export] public ItemType Type { get; set; }
    [Export] public int ShipSlotId { get; set; }

    public ItemPreset()
    {
    }

    public ItemPreset(int id, ItemType type, int shipSlotId)
    {
        Id = id;
        Type = type;
        ShipSlotId = shipSlotId;
    }
}