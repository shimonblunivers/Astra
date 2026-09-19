using Godot;

[GlobalClass]
public partial class Core : InteractableShipPart
{
    [Export] public AnimatedSprite2D Sprite { get; set; }

    public int Layer { get; set; } = 0;

    public override void Init(Ship targetShip, Vector2I coords, float durability = 200f, float mass = 5f)
    {
        base.Init(targetShip, coords, durability, mass);
    }

    public override void _Ready()
    {
        base._Ready();
        Sprite ??= GetNodeOrNull<AnimatedSprite2D>("Sprite2D");
    }

    protected override void OnInteract()
    {
        var saveFile = World.SaveFile;
        if (saveFile != null)
        {
            saveFile.SaveWorld(false);
        }
        else
        {
            GD.PushWarning("Warning: Cannot save world, World.SaveFile is null.");
        }
    }

    public void OnArea2DAreaEntered(Area2D area)
    {
        if (area != null && area.IsInGroup("PlayerArea"))
        {
            var owner = area.GetOwner();
            if (owner is Player player)
            {
                PlayerInRange = player;
                var hovering = (Godot.Collections.Array)player.Get("hovering_controllables");
                if (hovering != null && !hovering.Contains(this))
                {
                    hovering.Add(this);
                }
            }
        }
    }

    public void OnArea2DAreaExited(Area2D area)
    {
        if (area != null && area.IsInGroup("PlayerArea"))
        {
            if (PlayerInRange != null)
            {
                var hovering = (Godot.Collections.Array)PlayerInRange.Get("hovering_controllables");
                hovering?.Remove(this);
                PlayerInRange = null;
            }
        }
    }
}