using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ShipSaveFile : Resource
{
    [Export] public int Id { get; set; }
    [Export] public string Path { get; set; } = string.Empty;
    [Export] public Vector2 Position { get; set; }
    [Export] public Vector2 OldPosition { get; set; }
    [Export] public Vector2 Velocity { get; set; }
    [Export] public float Rotation { get; set; }

    [Export] public Array<Vector2I> DestroyedWalls { get; set; } = new Array<Vector2I>();
    [Export] public Array<Vector2I> OpenedDoors { get; set; } = new Array<Vector2I>();
    [Export] public Array<int> PickedupItems { get; set; } = new Array<int>();

    public static Array<ShipSaveFile> Save()
    {
        var files = new Array<ShipSaveFile>();
        if (Ship.Ships == null) return files;

        var world = World.Instance ?? (World)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("World");

        foreach (var ship in Ship.Ships)
        {
            if (!GodotObject.IsInstanceValid(ship)) continue;

            var file = new ShipSaveFile
            {
                Id = ship.Id,
                Path = ship.Path == "%player_ship_new" ? "%player_ship_old" : ship.Path,
                Position = world != null ? world.GetDistanceFromCenter(ship.GlobalPosition) : ship.GlobalPosition,
                OldPosition = (Vector2)ship.Get("_old_position"),
                Velocity = ship.LinearVelocity,
                Rotation = (float)ship.Rotation,
                DestroyedWalls = new Array<Vector2I>(ship.DestroyedWalls),
                OpenedDoors = new Array<Vector2I>(ship.OpenedDoors),
                PickedupItems = new Array<int>(ship.PickedupItems)
            };

            files.Add(file);
        }

        return files;
    }

    public void Load(System.Collections.IEnumerable npcs = null, System.Collections.IEnumerable items = null)
    {
        var itemPresets = new Godot.Collections.Array();
        var npcPresets = new Godot.Collections.Array();

        if (npcs != null)
        {
            foreach (var rawNpc in npcs)
            {
                if (rawNpc is GodotObject npcObj)
                {
                    int shipId = (int)npcObj.Get("ship_id");
                    if (Id == shipId)
                    {
                        int npcId = (int)npcObj.Get("id");
                        string nickname = (string)npcObj.Get("nickname");
                        var rolesVariant = npcObj.Get("roles");

                        var rolesArray = new Godot.Collections.Array();
                        if (rolesVariant.VariantType == Variant.Type.Array)
                        {
                            rolesArray = rolesVariant.AsGodotArray();
                        }
                        else
                        {
                            rolesArray.Add(rolesVariant);
                        }

                        var colors = npcObj.Get("colors");
                        if (colors.VariantType == Variant.Type.Nil)
                        {
                            colors = npcObj.Get("skin");
                        }

                        int hair = (int)npcObj.Get("hair");
                        npcPresets.Add(new NPCPreset(npcId, nickname, rolesArray, colors.AsColorArray(), hair));
                    }
                }
            }
        }

        if (items != null)
        {
            foreach (var rawItem in items)
            {
                if (rawItem is GodotObject itemObj)
                {
                    int shipId = (int)itemObj.Get("ship_id");
                    if (Id == shipId)
                    {
                        int itemId = (int)itemObj.Get("id");
                        int typeId = (int)itemObj.Get("type");
                        int shipSlotId = (int)itemObj.Get("ship_slot_id");

                        var itemType = Item.GetTypeById(typeId);
                        itemPresets.Add(new ItemPreset(itemId, itemType, shipSlotId));
                    }
                }
            }
        }

        var customObjectSpawn = CustomObjectSpawn.Create(npcPresets, itemPresets);

        var ship = ShipManager.SpawnShip(Position, Path, customObjectSpawn, fromSave: true);
        if (ship == null) return;

        ship.Id = Id;
        ship.Rotation = Rotation;
        ship.LinearVelocity = Velocity;
        ship.CallDeferred("apply_changes", DestroyedWalls, OpenedDoors);
        ship.Set("_old_position", OldPosition);
    }
}