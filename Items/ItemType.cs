using Godot;

[GlobalClass]
public partial class ItemType : Resource
{
    [Export] public Texture2D Texture { get; set; }
    [Export] public string ItemName { get; set; } = string.Empty;
    [Export] public string Nickname { get; set; } = string.Empty;
    [Export] public int Worth { get; set; } = 0;
    [Export] public float Rarity { get; set; } = 1.0f;
    [Export] public bool FreeSpawn { get; set; } = true;
    [Export] public Shape2D Shape { get; set; }

    public void Create()
    {
        if (string.IsNullOrEmpty(ItemName))
        {
            GD.PushWarning("Warning: Attempted to register ItemType with an empty or null name.");
            return;
        }

        Item.Types[ItemName] = this;
    }
}