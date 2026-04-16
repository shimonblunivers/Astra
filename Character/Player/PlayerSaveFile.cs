using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class PlayerSaveFile : Resource
{
    private dynamic N(string path) => GetNode(path);


    [Export] Vector2 position;
    [Export] float rotation;
    [Export] Vector2 acceleration;
    [Export] int currency;
    [Export] bool suit;
    [Export] int health;
    [Export] int owned_ship_id;

    [Export] int highlighted_quest_id;


    public static PlayerSaveFile save()
    {
    dynamic file = new PlayerSaveFile();

    file.Position = World.instance.get_distance_from_center(Player.main_player.GlobalPosition);
    file.rotation = Player.main_player.global_rotation;
    file.acceleration = Player.main_player.acceleration;

    file.health = Player.main_player.health;
    file.currency = Player.main_player.currency;
    file.suit = Player.main_player.suit;

    file.owned_ship_id = Player.main_player.owned_ship.id;

    file.highlighted_quest_id = QuestManager.highlighted_quest_id;

    return file

    }

    public void load()
    {

    dynamic player = Player.main_player;
    player.spawn(position, acceleration, rotation);

    player.set_health(health);
    player.suit = suit;
    player.currency = currency;
    player.currency_updated_signal.emit();

    player.owned_ship = Ship.get_ship(owned_ship_id);

    QuestManager.highlighted_quest_id = highlighted_quest_id;
    }

}