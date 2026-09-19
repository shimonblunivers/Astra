using Godot;

[GlobalClass]
public partial class ShipManager : Node2D
{
    private static readonly PackedScene ShipScene = GD.Load<PackedScene>("res://Ship/Ship.tscn");

    public static ShipManager Instance { get; private set; }
    public static int NumberOfShips { get; set; } = 0;

    private static Ship _mainStation;
    public static Ship MainStation
    {
        get => _mainStation;
        set
        {
            if (value != null)
            {
                value.Freeze = true;
            }
            _mainStation = value;
        }
    }

    public override void _Ready()
    {
        Instance = this;
    }

    // Dynamic caller compatibility for GDScript and World.cs
    public void randomly_generate_ships() => RandomlyGenerateShips();

    public static void RandomlyGenerateShips()
    {
        GD.Print("[ShipManager] Generating initial ships...");

        if (Player.MainPlayer != null)
        {
            var owned = SpawnShip(new Vector2(-10000f, -10000f), "_start_ship");
            Player.MainPlayer.OwnedShip = owned;

            if (owned != null)
            {
                owned.LinearDamp = 0f;
            }
        }

        GD.Print("Spawning main station..");
        MainStation = SpawnShip(Vector2.Zero, "_station", null, fromSave: false, lockRotation: true);
    }

    public static string RandomShip()
    {
        return $"_small_shuttle_{GD.RandRange(0, 4)}";
    }

    public static string GetQuestShipPath(int missionId)
    {
        return RandomShip();
    }

    public static Ship SpawnShip(Vector2 position, string path = "_station", CustomObjectSpawn customObjectSpawn = null, bool fromSave = false, bool lockRotation = false)
    {
        if (ShipScene == null) return null;

        var ship = ShipScene.Instantiate<Ship>();
        if (ship == null) return null;

        var currentInstance = Instance ?? (ShipManager)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("ShipManager");
        currentInstance?.AddChild(ship);

        GD.Print($"Spawning ship at {position} with path {path}");
        ship.LoadShip(position, path, customObjectSpawn, lockRotation, fromSave);
        return ship;
    }

    public static Ship BuildShip(Builder builder, bool forPlayer, string path = "_station")
    {
        if (ShipScene == null || builder == null) return null;

        var ship = ShipScene.Instantiate<Ship>();
        if (ship == null) return null;

        builder.PlaySound();

        var currentInstance = Instance ?? (ShipManager)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("ShipManager");
        currentInstance?.AddChild(ship);

        ship.LoadShip(builder.GetSpawnPosition(), path, null, lockRotation: true, fromSave: true);

        if (builder.Ship != null)
        {
            ship.LinearVelocity = builder.Ship.LinearVelocity;
        }

        ship.Rotation = builder.GetShipRotation();

        if (forPlayer && Player.MainPlayer != null)
        {
            if (Player.MainPlayer.OwnedShip != null)
            {
                Player.MainPlayer.Call("deleting_ship", Player.MainPlayer.OwnedShip);
                Player.MainPlayer.OwnedShip.Delete();
            }
            Player.MainPlayer.OwnedShip = ship;
        }

        if (ship.Connectors.Count > 0 && ship.Connectors[0] != null && builder.ConnectorInstance != null)
        {
            ship.Connectors[0].ConnectTo(builder.ConnectorInstance);
        }

        return ship;
    }
}