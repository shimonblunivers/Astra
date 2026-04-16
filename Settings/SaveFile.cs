using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class SaveFile : Resource
{
    private dynamic N(string path) => GetNode(path);



    [Export] PlayerSaveFile player_save_file;
    [Export] int main_station_id;

    [Export] Array NPC_save_files = new Array();
    [Export] Array item_save_files = new Array();
    [Export] Array quest_save_files = new Array();
    [Export] Array task_save_files = new Array();
    [Export] Array ship_save_files = new Array();

    public static string save_name = "last_save";


    public void initialize_files()
    {
    DirAccess.make_dir_recursive_absolute("user://saves/ships");
    DirAccess.make_dir_recursive_absolute("user://saves/worlds");

    }

    public static string get_save_path()
    {
    return path + "/save_file.tres"

    }

    public void save_world()
    {

    GD.Print("Saving...");

    UIManager.instance.saving_screen();

    player_save_file = PlayerSaveFile.save();
    main_station_id = ShipManager.main_station.id;
    ship_save_files = ShipSaveFile.save();
    NPC_save_files = NPCSaveFile.save();
    item_save_files = ItemSaveFile.save();
    quest_save_files = QuestSaveFile.save();
    task_save_files = TaskSaveFile.save();

    if (DirAccess.dir_exists_absolute("user://saves/ships/%player_ship_new"))
    {
        }
    if DirAccess.dir_exists_absolute("user://saves/ships/%player_ship_old"): delete_directory("user://saves/ships/%player_ship_old");
    DirAccess.rename_absolute("user://saves/ships/%player_ship_new", "user://saves/ships/%player_ship_old");
    if DirAccess.dir_exists_absolute("user://saves/ships/%player_ship_old"): Player.main_player.owned_ship.path = "%player_ship_old";
    if (!dev)
    {
        }
    DirAccess.make_dir_recursive_absolute("user://saves/worlds/" + save_name + "/");
    return ResourceSaver.save(this, SaveFile.get_save_path())
    }
    else
    {
        }
    return ResourceSaver.save(this, SaveFile.get_save_path(FIRST_SAVE_GAME_PATH))

    }

    public void load_world()
    {
    if (FileAccess.file_exists(SaveFile.get_save_path()))
    {
        }
    World.save_file = ResourceLoader.load(SaveFile.get_save_path());
    World.save_file.CallDeferred("_load");
    }
    else
    {
        }
    World.instance.new_world();
    // World.save_file = ResourceLoader.load(get_save_path(FIRST_SAVE_GAME_PATH))
    // World.save_file.call_deferred("_load")


    }

    public void _load()
    {
    World.reset_values();

    GD.Print("Loading...");

    for ship in ship_save_files: ship.load(NPC_save_files, item_save_files);
    ShipManager.main_station = Ship.get_ship(main_station_id);
    ShipManager.main_station.freeze = true;

    player_save_file.load();
    Player.main_player.owned_ship.linear_damp = 0;

    for quest : QuestSaveFile in quest_save_files: quest.load();
    for task : TaskSaveFile in task_save_files: task.load();

    }

    public bool delete_directory(string path)
    {
    dynamic dir = DirAccess.open(path);
    if (dir)
    {
        }
    dir.ListDirBegin();
    dynamic file_name = dir.GetNext();
    while (file_name != "")
    {
        }
    if (dir.CurrentIsDir())
    {
        if !delete_directory(path + "/" + file_name): return false;
        }
    else
    {
        dir.remove(path + "/" + file_name);
        }
    file_name = dir.GetNext();
    dir.remove(path);
    return true
    }
    else: return false;
    }

}