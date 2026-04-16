using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class Task : Resource
{
    private dynamic N(string path) => GetNode(path);

    // # Task that the player can accept from an NPC
    // TODO: class_name Task

    [Export] int id;
    [Export] string title;
    // TODO: @export_multiline var description : String

    [Export] int reward;
    [Export] Goal goal;

    [Export] int world_limit = -1;

    // # If true, the NPC won't give this mission randomly, only if the previous task is completed
    [Export] bool is_followup_task = false;
    // # If true, the mission will be finished completely only when you get back to the NPC
    [Export] bool return_to_npc = true;
    // # Required roles for NPC to give this mission
    [Export] NPC.Roles required_role;
    // # Roles that will be added to the NPC, if he gives this mission
    [Export] NPC.Roles add_role_on_accept;

    [Export] int difficulty_multiplier = 1;

    public int times_activated = 0;

}