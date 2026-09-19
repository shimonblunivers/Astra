using Godot;
using Godot.Collections;
using System.IO;
using FileAccess = Godot.FileAccess;

[GlobalClass]
public partial class SaveFile : Resource
{
    public const string SAVE_GAME_PATH = "user://saves/worlds/";
    public const string FIRST_SAVE_GAME_PATH = "res://DefaultSave/worlds/";

    [Export] public PlayerSaveFile PlayerSaveFile { get; set; }
    [Export] public int MainStationId { get; set; }

    [Export] public Array<NPCSaveFile> NpcSaveFiles { get; set; } = new Array<NPCSaveFile>();
    [Export] public Array<ItemSaveFile> ItemSaveFiles { get; set; } = new Array<ItemSaveFile>();
    [Export] public Array<QuestSaveFile> QuestSaveFiles { get; set; } = new Array<QuestSaveFile>();
    [Export] public Array<TaskSaveFile> TaskSaveFiles { get; set; } = new Array<TaskSaveFile>();
    [Export] public Array<ShipSaveFile> ShipSaveFiles { get; set; } = new Array<ShipSaveFile>();

    public static string SaveName { get; set; } = "last_save";

    public void InitializeFiles()
    {
        DirAccess.MakeDirRecursiveAbsolute("user://saves/ships");
        DirAccess.MakeDirRecursiveAbsolute("user://saves/worlds/last_save");

        // Copy default save files if target user save file doesn't exist
        string userSave = GetSavePath();
        if (!FileAccess.FileExists(userSave))
        {
            CopyDirectory("res://DefaultSave", "user://saves");
        }
    }

    private static void CopyDirectory(string sourceDir, string targetDir)
    {
        using var dir = DirAccess.Open(sourceDir);
        if (dir == null) return;

        DirAccess.MakeDirRecursiveAbsolute(targetDir);
        dir.ListDirBegin();

        string item = dir.GetNext();
        while (!string.IsNullOrEmpty(item))
        {
            if (item != "." && item != "..")
            {
                string srcPath = $"{sourceDir}/{item}";
                string dstPath = $"{targetDir}/{item}";

                if (dir.CurrentIsDir())
                {
                    CopyDirectory(srcPath, dstPath);
                }
                else
                {
                    // Copy file if not already existing
                    if (!FileAccess.FileExists(dstPath))
                    {
                        using var srcFile = FileAccess.Open(srcPath, FileAccess.ModeFlags.Read);
                        if (srcFile != null)
                        {
                            byte[] buffer = srcFile.GetBuffer((long)srcFile.GetLength());
                            using var dstFile = FileAccess.Open(dstPath, FileAccess.ModeFlags.Write);
                            dstFile?.StoreBuffer(buffer);
                        }
                    }
                }
            }
            item = dir.GetNext();
        }
        dir.ListDirEnd();
    }

    public static string GetSavePath(string path = null)
    {
        string basePath = path ?? (SAVE_GAME_PATH + SaveName);
        return $"{basePath}/save_file.tres";
    }

    public Error SaveWorld(bool dev = false)
    {
        GD.Print("Saving...");

        var uiManager = (Node)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("UIManager");
        var uiInstance = (Node)uiManager?.Get("instance");
        uiInstance?.Call("saving_screen");

        PlayerSaveFile = PlayerSaveFile.Save();

        var shipManager = (Node)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("ShipManager");
        var mainStation = (Node)shipManager?.Get("main_station");
        MainStationId = mainStation != null ? (int)mainStation.Get("id") : -1;

        ShipSaveFiles = ShipSaveFile.Save();
        NpcSaveFiles = NPCSaveFile.Save();
        ItemSaveFiles = ItemSaveFile.Save();
        QuestSaveFiles = QuestSaveFile.Save();
        TaskSaveFiles = TaskSaveFile.Save();

        const string newShipDir = "user://saves/ships/%player_ship_new";
        const string oldShipDir = "user://saves/ships/%player_ship_old";

        if (DirAccess.DirExistsAbsolute(newShipDir))
        {
            if (DirAccess.DirExistsAbsolute(oldShipDir))
            {
                DeleteDirectory(oldShipDir);
            }

            DirAccess.RenameAbsolute(newShipDir, oldShipDir);

            if (DirAccess.DirExistsAbsolute(oldShipDir) && Player.MainPlayer != null)
            {
                var ownedShip = (Node)Player.MainPlayer.Get("owned_ship");
                ownedShip?.Set("path", "%player_ship_old");
            }
        }

        if (!dev)
        {
            DirAccess.MakeDirRecursiveAbsolute($"user://saves/worlds/{SaveName}/");
            return ResourceSaver.Save(this, GetSavePath());
        }
        else
        {
            return ResourceSaver.Save(this, GetSavePath(FIRST_SAVE_GAME_PATH));
        }
    }

    public void LoadWorld()
    {
        string path = GetSavePath();

        if (FileAccess.FileExists(path))
        {
            var loadedFile = GD.Load<SaveFile>(path);
            World.SaveFile = loadedFile;
            loadedFile.CallDeferred(nameof(InternalLoad));
        }
        else
        {
            if (World.Instance != null)
            {
                World.Instance.NewWorld();
            }
        }
    }

    private void InternalLoad()
    {
        World.ResetValues();

        GD.Print("Loading...");

        foreach (var shipSave in ShipSaveFiles)
        {
            shipSave?.Load(NpcSaveFiles, ItemSaveFiles);
        }

        var shipManager = (Node)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("ShipManager");
        if (shipManager != null && MainStationId != -1)
        {
            var station = Ship.GetShip(MainStationId);
            if (station != null)
            {
                shipManager.Set("main_station", station);
                station.Set("freeze", true);
            }
        }

        PlayerSaveFile?.Load();
        if (Player.MainPlayer != null)
        {
            var ownedShip = (RigidBody2D)Player.MainPlayer.Get("owned_ship");
            if (ownedShip != null)
            {
                ownedShip.LinearDamp = 0;
            }
        }

        foreach (var taskSave in TaskSaveFiles)
        {
            taskSave?.Load();
        }

        foreach (var questSave in QuestSaveFiles)
        {
            questSave?.Load();
        }
    }

    public static bool DeleteDirectory(string path)
    {
        string globalPath = ProjectSettings.GlobalizePath(path);
        try
        {
            if (Directory.Exists(globalPath))
            {
                Directory.Delete(globalPath, true);
                return true;
            }
        }
        catch (System.Exception ex)
        {
            GD.PushError($"Failed to delete directory {path}: {ex.Message}");
        }

        return false;
    }
}