using Godot;

[GlobalClass]
public partial class ObjectTileMapLayer : TileMapLayer
{
    private static readonly PackedScene BuilderScene = GD.Load<PackedScene>("res://Ship/Objects/Builder/Builder.tscn");
    private static readonly PackedScene HelmScene = GD.Load<PackedScene>("res://Ship/Objects/Helm/Helm.tscn");
    private static readonly PackedScene NpcScene = GD.Load<PackedScene>("res://Character/NPC/NPC.tscn");

    public Ship Ship { get; set; } = null;

    public bool LoadShip(Ship targetShip, string path, CustomObjectSpawn customObjectSpawn, bool fromSave = false)
    {
        Ship = targetShip;
        Clear();

        string userPath = $"user://saves/ships/{path}/objects.dat";
        string defaultPath = $"res://DefaultSave/ships/{path}/objects.dat";
        string resolvedPath;

        if (!FileAccess.FileExists(userPath))
        {
            if (!FileAccess.FileExists(defaultPath))
            {
                return false;
            }
            resolvedPath = defaultPath;
        }
        else
        {
            resolvedPath = userPath;
        }

        using (var saveFile = FileAccess.Open(resolvedPath, FileAccess.ModeFlags.Read))
        {
            if (saveFile == null)
            {
                return false;
            }

            while (saveFile.GetPosition() < saveFile.GetLength())
            {
                float posX = saveFile.GetFloat();
                float posY = saveFile.GetFloat();
                int sourceId = saveFile.Get16();
                int atlasX = (int)saveFile.GetFloat();
                int atlasY = (int)saveFile.GetFloat();
                int altTile = saveFile.Get16();

                var tilePos = new Vector2I((int)posX, (int)posY);
                var atlasCoords = new Vector2I(atlasX, atlasY);

                SetCell(tilePos, sourceId, atlasCoords, altTile);
            }
        }

        return ReplaceInteractiveTiles(customObjectSpawn, fromSave);
    }

    private bool ReplaceInteractiveTiles(CustomObjectSpawn customObjectSpawn, bool fromSave)
    {
        int npcIndex = 0;
        int itemIndex = 0;
        int itemSlot = 0;

        var rng = new RandomNumberGenerator();
        rng.Randomize();

        var rawNpcPresets = customObjectSpawn != null ? (Godot.Collections.Array)customObjectSpawn.Get("npc_presets") : null;
        var rawItemPresets = customObjectSpawn != null ? (Godot.Collections.Array)customObjectSpawn.Get("item_presets") : null;

        foreach (Vector2I cellPos in GetUsedCells())
        {
            TileData cell = GetCellTileData(cellPos);
            if (cell == null) continue;

            Vector2 tilePosition = MapToLocal(cellPos) * Constants.TileScale;
            string type = cell.GetCustomData("type").AsString();

            var objectTilesNode = (Node2D)Ship?.Get("object_tiles");

            switch (type)
            {
                case "helm":
                    if (HelmScene != null)
                    {
                        var helmObject = HelmScene.Instantiate<Helm>();
                        helmObject.Init(Ship, cellPos);
                        helmObject.Position = tilePosition;
                        objectTilesNode?.AddChild(helmObject);
                    }
                    break;

                case "builder":
                    if (BuilderScene != null)
                    {
                        var builderObject = BuilderScene.Instantiate<Builder>();
                        builderObject.Init(Ship, cellPos);
                        builderObject.Position = tilePosition;
                        objectTilesNode?.AddChild(builderObject);
                    }
                    break;

                case "npc":
                    if (NpcScene != null)
                    {
                        var npcObject = NpcScene.Instantiate<NPC>();
                        npcObject.Set("spawn_point", tilePosition);
                        npcObject.Call("spawn");

                        if (rawNpcPresets != null && rawNpcPresets.Count > 0 && npcIndex < rawNpcPresets.Count)
                        {
                            var presetObj = rawNpcPresets[npcIndex].AsGodotObject();
                            if (presetObj != null)
                            {
                                int id = (int)presetObj.Get("id");
                                string nickname = (string)presetObj.Get("nickname");
                                var roles = presetObj.Get("roles");
                                var colors = presetObj.Get("colors");
                                int hair = (int)presetObj.Get("hair");

                                npcObject.Call("init", id, nickname, roles, colors, hair);
                            }
                            npcIndex++;
                        }
                        else if (!fromSave)
                        {
                            npcObject.Call("init");
                        }

                        npcObject.Set("ship", Ship);

                        var passengersNode = (Node)Ship?.Get("passengers_node");
                        passengersNode?.AddChild(npcObject);

                        var passengersList = (Godot.Collections.Array)Ship?.Get("passengers");
                        passengersList?.Add(npcObject);
                    }
                    break;

                case "item":
                    float scaling = 4f * Constants.TileScale;
                    Vector2 offset = new Vector2(
                        scaling - rng.Randf() * scaling * 2f,
                        scaling - rng.Randf() * scaling * 2f
                    );
                    Vector2 spawnGlobalPos = ToGlobal(tilePosition) + offset;

                    if (rawItemPresets != null && rawItemPresets.Count > 0 && itemIndex < rawItemPresets.Count)
                    {
                        int startingIndex = itemIndex;

                        for (int i = 0; i < rawItemPresets.Count; i++)
                        {
                            var presetObj = rawItemPresets[i].AsGodotObject();
                            if (presetObj == null) continue;

                            int shipSlotId = (int)presetObj.Get("ship_slot_id");
                            if (shipSlotId == itemSlot)
                            {
                                int itemTypeId = (int)presetObj.Get("type");
                                int itemId = (int)presetObj.Get("id");

                                Item.Spawn(itemTypeId, spawnGlobalPos, itemId, Ship, itemSlot);
                                itemIndex++;
                                break;
                            }
                        }

                        if (startingIndex == itemIndex)
                        {
                            for (int i = 0; i < rawItemPresets.Count; i++)
                            {
                                var presetObj = rawItemPresets[i].AsGodotObject();
                                if (presetObj == null) continue;

                                int shipSlotId = (int)presetObj.Get("ship_slot_id");
                                if (shipSlotId < 0)
                                {
                                    int itemTypeId = (int)presetObj.Get("type");
                                    int itemId = (int)presetObj.Get("id");

                                    Item.Spawn(itemTypeId, spawnGlobalPos, itemId, Ship, itemSlot);
                                    itemIndex++;
                                    break;
                                }
                            }
                        }
                    }
                    else if (!fromSave)
                    {
                        Item.Spawn(Item.RandomItem(), spawnGlobalPos, -1, Ship, itemSlot);
                    }

                    itemSlot++;
                    break;
            }
        }

        return true;
    }
}