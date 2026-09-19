using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ObjectList : Node
{
    public static ObjectList Instance { get; private set; }
    public static World WorldInstance { get; set; }

    public bool StartedGame { get; set; } = false;

    public override void _Ready()
    {
        Instance = this;

        Item.LoadItems();
        WorldInstance = World.Instance;
    }

    public static Ship GetClosestShip(Vector2 fromGlobalPos, System.Collections.Generic.IList<Ship> ships = null)
    {
        var targetShips = ships ?? Ship.Ships;
        if (targetShips == null || targetShips.Count == 0) return null;

        Ship closest = targetShips[0];
        if (closest == null) return null;

        Vector2 closestPoint = (Vector2)closest.Call("get_closest_point", fromGlobalPos);
        float closestDistSq = closestPoint.DistanceSquaredTo(fromGlobalPos);

        for (int i = 1; i < targetShips.Count; i++)
        {
            var ship = targetShips[i];
            if (ship == null) continue;

            Vector2 point = (Vector2)ship.Call("get_closest_point", fromGlobalPos);
            float distSq = point.DistanceSquaredTo(fromGlobalPos);

            if (distSq < closestDistSq)
            {
                closest = ship;
                closestDistSq = distSq;
            }
        }

        return closest;
    }

    public Godot.Collections.Array<GodotObject> GetSaveableItems()
    {
        var list = new Godot.Collections.Array<GodotObject>();

        if (Ship.Ships != null)
        {
            foreach (var ship in Ship.Ships)
            {
                if (GodotObject.IsInstanceValid(ship)) list.Add(ship);
            }
        }

        if (NPC.Npcs != null)
        {
            foreach (var npc in NPC.Npcs)
            {
                if (GodotObject.IsInstanceValid(npc)) list.Add(npc);
            }
        }

        if (Item.Items != null)
        {
            foreach (var item in Item.Items)
            {
                if (GodotObject.IsInstanceValid(item)) list.Add(item);
            }
        }

        return list;
    }
}