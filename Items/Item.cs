using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class Item : Node2D
{
    private static readonly PackedScene ItemScene = GD.Load<PackedScene>("res://Items/Item.tscn");

    public static List<Item> Items { get; } = new List<Item>();
    public static List<int> ItemIdHistory { get; } = new List<int>();
    public static Dictionary<string, ItemType> Types { get; } = new Dictionary<string, ItemType>();

    [Export] public Area2D Area { get; set; }
    [Export] public Sprite2D Sprite { get; set; }
    [Export] public CollisionShape2D CollisionShape { get; set; }
    [Export] public Label Itemtag { get; set; }
    [Export] public AudioStreamPlayer2D PickedUpSound { get; set; }

    public Ship Ship { get; set; }
    public int ShipSlotId { get; set; } = -1;
    public bool CanPickup { get; set; } = false;
    public int Id { get; set; }
    public ItemType Type { get; set; }

    public static int GetUid()
    {
        int checkId = 0;
        while (true)
        {
            if (GetItem(checkId) == null && !ItemIdHistory.Contains(checkId))
            {
                return checkId;
            }
            checkId++;
        }
    }

    public static void LoadItems()
    {
        const string path = "res://Items/ItemTypes";
        using var dir = DirAccess.Open(path);
        if (dir == null) return;

        dir.ListDirBegin();
        string fileName = dir.GetNext();

        while (!string.IsNullOrEmpty(fileName))
        {
            if (!dir.CurrentIsDir())
            {
                if (fileName.Contains(".tres.remap"))
                {
                    fileName = fileName.TrimSuffix(".remap");
                }

                if (fileName.Contains(".tres"))
                {
                    var itemType = GD.Load<ItemType>($"{path}/{fileName}");
                    itemType?.Call("create");
                }
            }
            fileName = dir.GetNext();
        }

        dir.ListDirEnd();
    }

    public static Item GetItem(int targetId)
    {
        foreach (var item in Items)
        {
            if (item.Id == targetId) return item;
        }
        return null;
    }

    public static ItemType GetTypeById(int id)
    {
        string key = id.ToString();
        if (Types.TryGetValue(key, out var itemType))
        {
            return itemType;
        }

        // Fallback: search by id property on ItemType resource if key lookup differs
        foreach (var pair in Types)
        {
            if (pair.Value != null)
            {
                var valId = pair.Value.Get("id");
                if (valId.VariantType != Variant.Type.Nil && (int)valId == id)
                {
                    return pair.Value;
                }
            }
        }

        return RandomItem();
    }

    public static ItemType RandomItem()
    {
        if (Types.Count == 0) return null;
        var keys = new List<string>(Types.Keys);
        string randomKey = keys[(int)(GD.Randi() % keys.Count)];
        return Types[randomKey];
    }

    public static Item Spawn(int typeId, Vector2 globalCoords, int targetId = -1, Ship targetShip = null, int slotId = -1)
    {
        return Spawn(GetTypeById(typeId), globalCoords, targetId, targetShip, slotId);
    }

    public static Item Spawn(ItemType itemType, Vector2 globalCoords, int targetId = -1, Ship targetShip = null, int slotId = -1)
    {
        var newItem = ItemScene.Instantiate<Item>();
        newItem.Type = itemType;

        if (targetShip != null)
        {
            newItem.Ship = targetShip;
            ((Node)newItem.Ship.Get("items")).AddChild(newItem);
        }
        else
        {
            var objectList = (Node)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("ObjectList");
            var closestShip = (Ship)objectList?.Call("get_closest_ship", globalCoords);
            if (closestShip != null)
            {
                ((Node)closestShip.Get("items")).AddChild(newItem);
                newItem.Ship = closestShip;
            }
        }

        newItem.ShipSlotId = slotId;
        if (newItem.Ship != null)
        {
            int usedSlots = (int)newItem.Ship.Get("used_item_slots");
            newItem.Ship.Set("used_item_slots", usedSlots + 1);
        }

        newItem.GlobalPosition = globalCoords;

        if (targetId != -1 && GetItem(targetId) == null)
        {
            newItem.Id = targetId;
        }
        else
        {
            newItem.Id = GetUid();
        }

        Items.Add(newItem);
        ItemIdHistory.Add(newItem.Id);

        return newItem;
    }

    public override void _Ready()
    {
        Area ??= GetNode<Area2D>("Area2D");
        Sprite ??= GetNode<Sprite2D>("Sprite2D");
        CollisionShape ??= GetNode<CollisionShape2D>("Area2D/CollisionShape2D");
        Itemtag ??= GetNode<Label>("Itemtag");
        PickedUpSound ??= GetNode<AudioStreamPlayer2D>("PickedUpSound");

        if (Type != null)
        {
            var texture = (Texture2D)Type.Get("texture");
            var shape = (Shape2D)Type.Get("shape");
            string nickname = (string)Type.Get("nickname");

            Sprite.Texture = texture;
            CollisionShape.Shape = shape;

            if (shape is RectangleShape2D rectShape && texture != null && texture.GetWidth() > 0)
            {
                float scaleFactor = rectShape.Size.X / texture.GetWidth();
                Sprite.Scale = new Vector2(scaleFactor, scaleFactor);
                Itemtag.Position = new Vector2(Itemtag.Position.X, Itemtag.Position.Y - (rectShape.Size.Y / 2f + 12f));
            }

            Itemtag.Text = nickname;
        }

        var random = new RandomNumberGenerator();
        random.Randomize();
        float tilt = random.RandfRange(-20f, 20f);
        Sprite.RotationDegrees = tilt;
        Area.RotationDegrees = tilt;

        Callable.From(UpdateItemtagColor).CallDeferred();
    }

    public void UpdateItemtagColor()
    {
        var questManager = (Node)GetNodeOrNull("/root/QuestManager");
        bool isObjective = questManager != null && (bool)questManager.Call("is_objective", this);

        if (isObjective)
        {
            Itemtag.AddThemeColorOverride("font_outline_color", Quest.ObjectiveOfQuestOutlineColor);
            return;
        }

        Itemtag.AddThemeColorOverride("font_outline_color", Quest.DefaultOutlineColor);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Player.MainPlayer != null)
        {
            float dist = (GlobalPosition - Player.MainPlayer.GlobalPosition).Length();
            if (dist > Player.MainPlayer.UpdateRange) return;
        }

        if (Ship != null)
        {
            Vector2 diff = (Vector2)Ship.Get("difference_in_position");
            Area.Position = (-diff).Rotated(-(float)GlobalRotation);
        }
    }

    public void OnArea2DInputEvent(Node viewport, InputEvent @event, int shapeIdx)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left && CanPickup)
        {
            PickUp();
        }
    }

    public void OnArea2DMouseEntered()
    {
        Itemtag.Visible = true;
    }

    public void OnArea2DMouseExited()
    {
        Itemtag.Visible = false;
    }

    public async void PickUp()
    {
        PickedUpSound.Play();

        if (Player.MainPlayer != null)
        {
            var targetPos = Player.MainPlayer.GlobalPosition - Player.MainPlayer.Acceleration;
            float duration = Mathf.Max(0.05f, (GlobalPosition - Player.MainPlayer.GlobalPosition).Length() / 1200f);

            var tween = CreateTween();
            tween.TweenProperty(this, "global_position", targetPos, duration)
                .SetEase(Tween.EaseType.Out);

            await ToSignal(tween, Tween.SignalName.Finished);
        }

        var questManager = (Node)GetNodeOrNull("/root/QuestManager");
        if (questManager != null && (bool)questManager.Call("is_objective", this))
        {
            var quest = questManager.Call("get_quest_by_target", this);
            questManager.Call("finished_quest_objective", quest);
        }

        Visible = false;
    }

    public void Delete()
    {
        if (Ship != null)
        {
            int usedSlots = (int)Ship.Get("used_item_slots");
            Ship.Set("used_item_slots", usedSlots - 1);

            var pickedUp = (Godot.Collections.Array)Ship.Get("pickedup_items");
            pickedUp?.Add(Id);
        }

        Items.Remove(this);
        QueueFree();
    }

    public void OnPickedUpSoundFinished()
    {
        Delete();
    }
}