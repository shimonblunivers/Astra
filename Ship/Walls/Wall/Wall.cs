using Godot;
using Godot.Collections;

[GlobalClass]
public partial class Wall : ShipPart
{
    private static readonly PackedScene DebrisScene = GD.Load<PackedScene>("res://Ship/Walls/Debris/Debris.tscn");

    [Export] public Sprite2D Sprite { get; set; }
    [Export] public LightOccluder2D LightOccluder { get; set; }
    [Export] public ProgressBar Hp { get; set; }
    [Export] public AnimatedSprite2D Cracks { get; set; }
    [Export] public Button DamageButton { get; set; }

    public int Layer { get; set; } = 0;

    public override void Init(Ship targetShip, Vector2I coords, float durability = 100f, float mass = 4f)
    {
        base.Init(targetShip, coords, durability, mass);
    }

    public override void _Ready()
    {
        base._Ready();

        Sprite ??= GetNodeOrNull<Sprite2D>("Sprite2D");
        LightOccluder ??= GetNodeOrNull<LightOccluder2D>("LightOccluder2D");
        Hp ??= GetNodeOrNull<ProgressBar>("HP");
        Cracks ??= GetNodeOrNull<AnimatedSprite2D>("Cracks");
        DamageButton ??= GetNodeOrNull<Button>("Button");

        if (Hp != null)
        {
            Hp.MaxValue = DurabilityMax;
            Hp.Value = DurabilityMax;
        }

        if (DamageButton != null)
        {
            DamageButton.Pressed += OnButtonPressed;
        }
    }

    public void SetTexture(Texture2D texture)
    {
        if (Sprite != null)
        {
            Sprite.Texture = texture;
        }
    }

    private void OnButtonPressed()
    {
        var options = Options.Instance ?? (Options)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("Options");
        if (options != null && options.DevelopmentMode)
        {
            Damage(25f);
        }
    }

    public void Damage(float dmg)
    {
        DurabilityCurrent -= dmg;

        float ratio = Mathf.Clamp(DurabilityCurrent / DurabilityMax, 0f, 1f);

        if (DamageButton != null)
        {
            DamageButton.TooltipText = $"{Mathf.Snapped(ratio * 100f, 1f)}%";
        }

        if (Cracks != null)
        {
            if (ratio >= 0.75f)
            {
                Cracks.Frame = 0;
            }
            else if (ratio >= 0.5f)
            {
                Cracks.Frame = 1;
            }
            else if (ratio >= 0.25f)
            {
                Cracks.Frame = 2;
            }
            else
            {
                Cracks.Frame = 3;
            }
        }

        if (Hp != null)
        {
            Hp.Value = DurabilityCurrent;
        }

        if (DurabilityCurrent <= 0f)
        {
            Destroy();
        }
    }

    public override void Destroy()
    {
        if (DebrisScene != null && Ship != null)
        {
            var debrisObject = DebrisScene.Instantiate<Debris>();

            if (!Ship.DestroyedWalls.Contains(TilemapCoords))
            {
                Ship.DestroyedWalls.Add(TilemapCoords);
            }

            if (debrisObject != null)
            {
                debrisObject.Init(Ship, TilemapCoords);
                debrisObject.Position = Position;
                GetParent()?.CallDeferred(Node.MethodName.AddChild, debrisObject);
            }

            // Handle TileMapLayer vs legacy TileMap
            var wallTileMap = Ship.WallTileMap;
            if (wallTileMap is TileMapLayer tileMapLayer)
            {
                Vector2I pos = tileMapLayer.LocalToMap(Position);
                var cells = new Array<Vector2I> { pos };
                tileMapLayer.SetCellsTerrainConnect(cells, 0, -1, false);
            }
            else if (wallTileMap is TileMap legacyTileMap)
            {
                Vector2I pos = legacyTileMap.LocalToMap(Position);
                var cells = new Array<Vector2I> { pos };
                legacyTileMap.SetCellsTerrainConnect(0, cells, 0, -1, false);
            }
        }

        base.Remove();
    }
}