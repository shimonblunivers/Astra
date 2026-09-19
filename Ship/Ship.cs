using Godot;
using Godot.Collections;
using System.Collections.Generic;

[GlobalClass]
public partial class Ship : RigidBody2D
{
    public static List<Ship> Ships { get; } = new List<Ship>();

    [Export] public Node2D WallTileMap { get; set; }
    [Export] public ObjectTileMapLayer ObjectTileMap { get; set; }
    [Export] public Node2D WallTiles { get; set; }
    [Export] public Node2D ObjectTiles { get; set; }
    [Export] public Node2D ItemsNode { get; set; }
    [Export] public CollisionPolygon2D Hitbox { get; set; }
    [Export] public CanvasItem Visual { get; set; }
    [Export] public CollisionShape2D AreaHitbox { get; set; }
    [Export] public Node2D PassengersNode { get; set; }
    [Export] public Timer TimerNode { get; set; }

    // Snake_case aliases for GDScript interop
    public Node2D wall_tiles => WallTiles;
    public Node2D object_tiles => ObjectTiles;
    public Node2D passengers_node => PassengersNode;
    public Array<Node2D> passengers => Passengers;

    public int Id { get; set; }
    public Vector2[] Polygon { get; set; }
    public Vector2 DockPosition { get; set; } = new Vector2(100, 100);

    public Array<Node2D> Passengers { get; } = new Array<Node2D>();
    public int UsedItemSlots { get; set; } = 0;
    public Player ControlledBy { get; set; } = null;

    public Vector2 Acceleration { get; set; } = Vector2.Zero;
    public float RotationSpeed { get; set; } = 0f;

    public Vector4 ThrustPower { get; set; } = Vector4.Zero;

    public List<List<Thruster>> Thrusters { get; } = new List<List<Thruster>>
    {
        new List<Thruster>(),
        new List<Thruster>(),
        new List<Thruster>(),
        new List<Thruster>()
    };

    public Array<InteractableShipPart> Interactables { get; } = new Array<InteractableShipPart>();
    public string Path { get; set; } = string.Empty;

    public Array<Vector2I> DestroyedWalls { get; } = new Array<Vector2I>();
    public Array<Vector2I> OpenedDoors { get; } = new Array<Vector2I>();
    public Array<int> PickedupItems { get; } = new Array<int>();

    public bool Spawning { get; set; } = true;
    public float ComfortableRotationDegrees { get; set; } = 0f;
    public List<Connector> Connectors { get; } = new List<Connector>();
    public bool FromSave { get; set; } = false;

    private Vector2 _oldPosition = Vector2.Zero;
    public Vector2 DifferenceInPosition { get; private set; } = Vector2.Zero;

    public static Ship GetShip(int id)
    {
        foreach (var ship in Ships)
        {
            if (ship != null && ship.Id == id)
            {
                return ship;
            }
        }
        return null;
    }

    public void SetAngle(int angle)
    {
        ComfortableRotationDegrees = angle;
        if (NPC.Npcs == null) return;

        foreach (var npc in NPC.Npcs)
        {
            if (npc != null && (Ship)npc.Get("ship") == this)
            {
                npc.RotationDegrees = angle;
            }
        }
    }

    public override void _Ready()
    {
        Ships.Add(this);
        _oldPosition = Position;

        WallTileMap ??= GetNodeOrNull<Node2D>("WallTileMap");
        ObjectTileMap ??= GetNodeOrNull<ObjectTileMapLayer>("ObjectTileMap");
        WallTiles ??= GetNodeOrNull<Node2D>("WallTiles");
        ObjectTiles ??= GetNodeOrNull<Node2D>("ObjectTiles");
        ItemsNode ??= GetNodeOrNull<Node2D>("Items");
        Hitbox ??= GetNodeOrNull<CollisionPolygon2D>("Hitbox");
        Visual ??= GetNodeOrNull<CanvasItem>("Visual");
        AreaHitbox ??= GetNodeOrNull<CollisionShape2D>("Area/AreaHitbox");
        PassengersNode ??= GetNodeOrNull<Node2D>("Passengers");
        TimerNode ??= GetNodeOrNull<Timer>("Timer");
    }

    public void LoadShip(Vector2 position, string path, CustomObjectSpawn customObjectSpawn, bool lockRotation = false, bool fromSave = false)
    {
        GlobalPosition = position;
        _oldPosition = position;
        Path = path;
        Name = $"{path}-{Id}";

        GD.Print($"Loading ship: {Name}");

        FromSave = fromSave;

        GD.Print("Loading walls..");
        if (WallTileMap is WallTileMapLayer wallLayer)
        {
            wallLayer.LoadShip(this, path);
        }
        else
        {
            WallTileMap?.Call("LoadShip", this, path);
            WallTileMap?.Call("load_ship", this, path);
        }

        GD.Print("Loading objects..");
        if (ObjectTileMap != null)
        {
            ObjectTileMap.LoadShip(this, path, customObjectSpawn, fromSave);
        }
        else
        {
            GetNodeOrNull<Node>("ObjectTileMap")?.Call("LoadShip", this, path, customObjectSpawn, fromSave);
            GetNodeOrNull<Node>("ObjectTileMap")?.Call("load_ship", this, path, customObjectSpawn, fromSave);
        }

        var shipManager = (Node)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("ShipManager");
        if (shipManager != null)
        {
            Id = (int)shipManager.Get("number_of_ships");
            shipManager.Set("number_of_ships", Id + 1);
        }

        foreach (var thrusterList in Thrusters)
        {
            foreach (var thruster in thrusterList)
            {
                thruster.SetStatus(false);
            }
        }

        if (!lockRotation)
        {
            var rng = new RandomNumberGenerator();
            rng.Randomize();
            Rotation = rng.RandfRange(0f, Mathf.Tau);
        }
    }

    public ShipPart GetTile(Vector2I coords)
    {
        if (WallTiles == null) return null;

        foreach (Node child in WallTiles.GetChildren())
        {
            if (child is ShipPart part && part.TilemapCoords == coords)
            {
                return part;
            }
        }
        return null;
    }

    public Vector2 GetClosestPoint(Vector2 point1)
    {
        if (Polygon == null || Polygon.Length == 0)
        {
            return GlobalPosition;
        }

        Vector2 closest = Polygon[0].Rotated((float)GlobalRotation) + GlobalPosition;
        float closestDistSq = closest.DistanceSquaredTo(point1);

        for (int i = 1; i < Polygon.Length; i++)
        {
            Vector2 p = Polygon[i].Rotated((float)GlobalRotation) + GlobalPosition;
            float distSq = p.DistanceSquaredTo(point1);

            if (distSq < closestDistSq)
            {
                closest = p;
                closestDistSq = distSq;
            }
        }

        return closest;
    }

    public void SetConnector(int connectorIndex, Connector connector)
    {
        if (connectorIndex >= 0 && connectorIndex < Connectors.Count)
        {
            Connectors[connectorIndex]?.ConnectTo(connector);
        }
    }

    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        if (Player.MainPlayer != null)
        {
            float updateRange = (float)Player.MainPlayer.Get("update_range");
            if (GlobalPosition.DistanceTo(Player.MainPlayer.GlobalPosition) > updateRange)
            {
                return;
            }
        }

        Acceleration = Vector2.Zero;
        RotationSpeed = 0f;

        if (ControlledBy != null)
        {
            Control();
        }

        UpdateThrusters();
        UpdateSideThrusters();

        if (ControlledBy != null)
        {
            Vector2 worldAcceleration = Acceleration.Rotated((float)GlobalRotation);
            if (worldAcceleration != Vector2.Zero)
            {
                state.ApplyCentralImpulse(worldAcceleration);
            }

            if (!Mathf.IsZeroApprox(RotationSpeed))
            {
                state.ApplyTorqueImpulse(RotationSpeed);
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Spawning && !FromSave)
        {
            foreach (var exception in GetCollisionExceptions())
            {
                RemoveCollisionExceptionWith(exception);
            }

            var collision = MoveAndCollide(Vector2.Zero);
            if (collision == null)
            {
                Spawning = false;
            }
            else
            {
                AddCollisionExceptionWith((Node)collision.GetCollider());
                float depth = collision.GetDepth();
                float angle = collision.GetAngle();
                MoveAndCollide(new Vector2(depth * Mathf.Cos(angle) * 10f, 10f * depth * Mathf.Sin(angle)));
            }
        }

        DifferenceInPosition = Position - _oldPosition;
        _oldPosition = Position;
    }

    public void Control()
    {
        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        float rotationDirection = Input.GetAxis("game_turn_left", "game_turn_right");

        float rotationPower = 4000f * Mass;

        bool isAlive = ControlledBy != null && (bool)ControlledBy.Get("alive");
        if (!isAlive)
        {
            direction = Vector2.Zero;
        }

        Vector2 accel = Vector2.Zero;

        if (direction.X < 0) accel.X -= ThrustPower.X;
        else if (direction.X > 0) accel.X += ThrustPower.Z;

        if (direction.Y < 0) accel.Y -= ThrustPower.Y;
        else if (direction.Y > 0) accel.Y += ThrustPower.W;

        Acceleration = accel;

        int totalThrusters = Thrusters[0].Count + Thrusters[1].Count + Thrusters[2].Count + Thrusters[3].Count;
        if (totalThrusters == 0)
        {
            RotationSpeed = 0f;
        }
        else
        {
            RotationSpeed = (rotationPower * rotationDirection) + (rotationDirection * totalThrusters * rotationPower / 2f);
        }
    }

    public void UpdateSideThrusters()
    {
        foreach (var thrusterList in Thrusters)
        {
            foreach (var thruster in thrusterList)
            {
                thruster.SideThrusters(RotationSpeed);
            }
        }
    }

    public void ApplyChanges(Array<Vector2I> destroyedWalls = null, Array<Vector2I> openedDoors = null)
    {
        if (openedDoors != null)
        {
            foreach (Vector2I coords in openedDoors)
            {
                if (GetTile(coords) is Door door)
                {
                    door.Open();
                }
            }
        }

        if (destroyedWalls != null)
        {
            foreach (Vector2I coords in destroyedWalls)
            {
                if (GetTile(coords) is Wall wall)
                {
                    wall.Destroy();
                }
            }
        }
    }

    public void UpdateThrusters()
    {
        UpdateThrusterSublist(Thrusters[0], Acceleration.X < 0);
        UpdateThrusterSublist(Thrusters[2], Acceleration.X > 0);
        UpdateThrusterSublist(Thrusters[1], Acceleration.Y < 0);
        UpdateThrusterSublist(Thrusters[3], Acceleration.Y > 0);
    }

    private static void UpdateThrusterSublist(List<Thruster> thrusters, bool shouldRun)
    {
        foreach (var thruster in thrusters)
        {
            if (thruster.Running != shouldRun)
            {
                thruster.SetStatus(shouldRun);
            }
        }
    }

    public Rect2 GetRect()
    {
        return WallTileMap != null ? (Rect2)WallTileMap.Call("get_rect") : new Rect2();
    }

    public Vector2 GetTileSize()
    {
        if (WallTileMap == null) return Vector2.Zero;

        var tileSet = (TileSet)WallTileMap.Get("tile_set");
        Vector2 baseSize = tileSet != null ? (Vector2)tileSet.TileSize : Vector2.Zero;
        return baseSize * WallTileMap.Scale;
    }

    public void StartControlling(Player player)
    {
        ControlledBy = player;
    }

    public void StopControlling()
    {
        ControlledBy = null;
    }

    public void OnAreaAreaEntered(Area2D area)
    {
        if (area.IsInGroup("PlayerInteractArea"))
        {
            var body = area.GetParent() as Node2D;
            if (body == null) return;

            bool spawned = (bool)body.Get("spawned");
            if (!spawned || Passengers.Contains(body)) return;

            Passengers.Add(body);
            body.Call("get_in", this);
        }
    }

    public void OnAreaAreaExited(Area2D area)
    {
        if (area.IsInGroup("PlayerInteractArea"))
        {
            var body = area.GetParent() as Node2D;
            if (body == null) return;

            bool spawned = (bool)body.Get("spawned");
            if (!spawned || !Passengers.Contains(body)) return;

            Passengers.Remove(body);
            body.Call("get_off", this);
        }
    }

    public void Delete()
    {
        if (Hitbox != null)
        {
            Hitbox.Disabled = true;
        }

        foreach (var connector in Connectors)
        {
            connector?.ConnectTo(null);
        }

        if (Player.MainPlayer != null && (Ship)Player.MainPlayer.Get("parent_ship") == this)
        {
            if (World.Instance != null)
            {
                Player.MainPlayer.Reparent(World.Instance);
            }
        }

        var shipManager = (Node)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("ShipManager");
        if (shipManager != null)
        {
            int count = (int)shipManager.Get("number_of_ships");
            shipManager.Set("number_of_ships", Mathf.Max(0, count - 1));
        }

        Ships.Remove(this);
        QueueFree();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (ControlledBy == null) return;

        if (@event.IsActionPressed("game_dock_ship"))
        {
            bool connected = false;
            foreach (var connector in Connectors)
            {
                if (connector != null && connector.ConnectedTo != null)
                {
                    connector.ConnectTo(null);
                    connected = true;
                }
            }

            if (!connected && Connectors.Count > 0 && Connectors[0] != null && Connectors[0].ConnectorsInRange.Count > 0)
            {
                Connectors[0].ConnectTo(Connectors[0].ConnectorsInRange[0]);
            }
        }
    }

    public override void _Draw()
    {
        if (Visual != null && !Visual.Visible) return;

        if (Polygon != null)
        {
            foreach (Vector2 point in Polygon)
            {
                DrawCircle(point, 16f, Colors.Red);
            }
        }

        if (WallTileMap != null)
        {
            var testPoly = WallTileMap.Get("test_polygon").AsVector2Array();
            if (testPoly != null)
            {
                foreach (Vector2 point in testPoly)
                {
                    DrawCircle(point + new Vector2(0, -200), 16f, Colors.Blue);
                }
            }
        }
    }
}