using Godot;

[GlobalClass]
public partial class Helm : InteractableShipPart
{
    public bool Controlled { get; set; } = false;

    public override void Init(Ship targetShip, Vector2I coords, float durability = 10f, float mass = 1f)
    {
        // Flooring division handles negative tile coordinates properly
        Vector2I adjustedCoords = new Vector2I(
            Mathf.FloorToInt((float)coords.X / 4f),
            Mathf.FloorToInt((float)coords.Y / 4f)
        );

        base.Init(targetShip, adjustedCoords, durability, mass);
    }

    protected override void OnInteract()
    {
        if (PlayerInRange == null) return;

        var controllablesInUse = (Godot.Collections.Array)PlayerInRange.Get("controllables_in_use");

        if (Controlled)
        {
            PlayerInRange.Call("control_ship", (Ship)null);
            controllablesInUse?.Remove(this);
        }
        else
        {
            PlayerInRange.Call("control_ship", Ship);
            controllablesInUse?.Add(this);
        }

        Controlled = !Controlled;
    }

    public void OnAreaAreaEntered(Area2D area)
    {
        if (area.IsInGroup("PlayerArea"))
        {
            var owner = area.GetOwner();
            if (owner is Player player)
            {
                PlayerInRange = player;
                var hovering = (Godot.Collections.Array)player.Get("hovering_controllables");
                hovering?.Add(this);
            }
        }
    }

    public void OnAreaAreaExited(Area2D area)
    {
        if (area.IsInGroup("PlayerArea"))
        {
            if (PlayerInRange != null)
            {
                var hovering = (Godot.Collections.Array)PlayerInRange.Get("hovering_controllables");
                hovering?.Remove(this);

                // Do not clear the player reference if they are actively controlling the helm
                if (!Controlled)
                {
                    PlayerInRange = null;
                }
            }
        }
    }
}