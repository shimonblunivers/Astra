using Godot;
using System;
using System.Threading.Tasks;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public partial class NPC : Character
{
    private dynamic N(string path) => GetNode(path);


    public dynamic difference = Vector2.Zero;



    public Ship ship;

    public Array roles = new Array();

    public enum Roles
    {
        CIVILIAN
        NONE
        TRUSTED
        CAPTAIN
        ADDICTED
    }

    public static Array names = [;
    // TODO: "Kevin",
    // TODO: "Lukáš",
    // TODO: "Tomáš",
    // TODO: "Jan",
    // TODO: "Pavel",
    // TODO: "Martin",
    // TODO: "Jakub",
    // TODO: "Michal",
    // TODO: "Jiří",
    // TODO: "Adam",
    // TODO: "David",
    // TODO: "Marek",
    // TODO: "Petr",
    // TODO: "Ondřej",
    // TODO: "Filip",
    // TODO: "Richard",
    // TODO: "Robert",
    // TODO: "Václav",
    // TODO: "Matěj",
    // TODO: "Aleš",
    // TODO: "Daniel",
    // TODO: "Josef",
    // TODO: "Karel",
    // TODO: "Vojtěch",
    // TODO: "František",
    // TODO: "Eduard",
    // TODO: "Viktor",
    // TODO: "Igor",
    // TODO: "Radim",
    // TODO: "Radek",
    // TODO: "Lukas",
    // TODO: "Dominik",
    // TODO: "Jakub",
    // TODO: "Rudolf",
    // TODO: "Lukáš",
    // TODO: "Emanuel",
    // TODO: "Štěpán",
    // TODO: "Jaroslav",
    // TODO: "Michael",
    // TODO: "Zdeněk",
    // TODO: "Aleš",
    // TODO: "Patrik",
    // TODO: "Tom",
    // TODO: "Albert",
    // TODO: "Viktor",
    // TODO: "Jiří",
    // TODO: "Denis",
    // TODO: "Pavel",
    // TODO: "Igor",
    // TODO: "Eduard",
    // TODO: "Jan",
    // TODO: "Luboš",
    // TODO: "Šimon",
    // TODO: "Teo",
    // TODO: "Honza",
    // TODO: "Vašek",
    // TODO: ]

    public static Array npcs = new Array();

    public bool interactable = false;

    public bool hovering = false;

    public int active_quest_id = -1 :;
    // TODO: set (value):
    // TODO: if value == -1:
    // TODO: reload_missions()
    // TODO: else:
    // TODO: for npc in NPC.npcs:
    // TODO: if npc.selected_quest_id == value && npc != self:
    // TODO: npc.reload_missions()
    // TODO: active_quest_id = value
    // TODO: update_nametag_color()

    public dynamic selected_quest_id = -1 : # IS TALKING ABOUT QUEST?;
    // TODO: set (value):
    // TODO: if (active_quest_id != -1 || QuestManager.is_objective(self)):
    // TODO: selected_quest_id = -1
    // TODO: else:
    // TODO: selected_quest_id = value
    // TODO: update_nametag_color()

    public int id;

    // TODO: ✅ Add dialog

    // TODO: ✅ Add missions

    public dynamic skin = null;
    public dynamic hair = null;

    // # Updates [member selected_quest_id] with a new mission. [br]
    // # [forced]: the mission will be updated even if the NPC is currently talking about a quest. [br]

    public static int get_uid()
    {
    int _id = 0;
    while (true)
    {
        }
    if (NPC.get_npc(_id) == null)
    {
        }
    return _id
    _id += 1;
    return 0

    }

    public static NPC get_npc(int _id)
    {
    for npc in npcs: if npc.id == _id: return npc;
    return null

    }

    public void init(int _id = -1, string _nickname = names.pick_random(), dynamic _skin = null, dynamic _hair = null)
    {
    roles = _roles;
    skin = _skin;
    hair = _hair;

    if (_id != -1 && NPC.get_npc(_id) == null)
    {
        }
    id = _id;
    }
    else
    {
        }
    id = NPC.get_uid();

    if (id == 0 && _skin == null)
    {
        }
    _nickname = "Kapitán " + _nickname;
    roles.append(Roles.CAPTAIN);
    nickname = _nickname;

    if OS.is_debug_build(): nickname += " " + str(id);

    N("Nametag").Text = nickname;
    name = "NPC_" + nickname + "_" + str(id);

    npcs.append(this);


    }

    public void update_nametag_color()
    {
    if (QuestManager.is_objective(this))
    {
        }
    N("Nametag").add_theme_color_override("font_outline_color", Quest.objective_of_quest_outline_color);
    return
    if (active_quest_id > -1)
    {
        }
    N("Nametag").add_theme_color_override("font_outline_color", Quest.active_quest_outline_color);
    return
    if (selected_quest_id > -1)
    {
        }
    N("Nametag").add_theme_color_override("font_outline_color", Quest.talking_about_quest_outline_color);
    return
    N("Nametag").add_theme_color_override("font_outline_color", Quest.default_outline_color);


    }

    public void quest_finished()
    {
    active_quest_id = -1;
    N("FinishedQuest").play();

    }

    public void reload_missions()
    {
    if !forced && active_quest_id != -1: return;

    dynamic mission = Dialogs.random_task_id(roles, true);

    if (mission < 0)
    {
        }
    selected_quest_id = -1;
    }
    else
    {
        }
    selected_quest_id = mission;

    random := new RandomNumberGenerator();
    timer.start(120 + 60 * random.randi_range(0, 8));

    update_nametag_color();

    }

    public override void _Ready()
    {
    legs_offset = legs.Position;

    if (skin != null)
    {
        }
    sprites.set_skin(skin[0], skin[1], skin[2], skin[3], skin[4]);

    if (hair != null)
    {
        }
    sprites.hair_node.Frame = hair[0];
    sprites.hair_node.FlipH = hair[1];

    skin = sprites.skin;
    reload_missions();


    }

    public void _in_physics(dynamic _delta)
    {
    N("Area").Position = Vector2(0, -42.5) + (-ship.difference_in_position).rotated(-global_rotation);

    }

    public void _on_interaction_area_area_entered(Area2D area)
    {
    if (area.is_in_group("PlayerInteractArea"))
    {
        }
    interactable = true;

    }

    public void _on_interaction_area_area_exited(Area2D area)
    {
    if (area.is_in_group("PlayerInteractArea"))
    {
        }
    interactable = false;
    dialog_manager.end_dialog();

    }

    public void _on_area_mouse_entered()
    {
    hovering = true;
    }

    public void _on_area_mouse_exited()
    {
    hovering = false;

    }

    public override void _Process(double delta)
    {
    N("Nametag").Visible = hovering && interactable;

    }

    public void _on_area_input_event(Node _viewport, InputEvent event, int _shape_idx)
    {
    if (event is InputEventMouseButton && event.ButtonMask == 1)
    {
        }
    if (interactable)
    {
        }
    if (dialog_manager.is_dialog_active)
    {
        dialog_manager.advance();
        }
    else
    {
        dynamic dialog_position = Vector2(0, -105);

        if (QuestManager.is_objective(this))
        {
            }
        if (QuestManager.get_quest_by_target(this).task.id in Dialogs.conversations["mission_finished"].keys())
        {
            }
        dialog_manager.start_dialog(dialog_position, Dialogs.conversations["mission_finished"][QuestManager.get_quest_by_target(this).task.id]);
        }
        else
        {
            }
        dialog_manager.start_dialog(dialog_position, Dialogs.conversations["mission_finished"][-1]);

        QuestManager.Finished_quest_objective(QuestManager.get_quest_by_target(this));

        }
        else if (selected_quest_id >= 0)
        {
            }
        if (selected_quest_id in QuestManager.active_task_ids)
        {
            }
        GD.Print("Warning: Task " + str(selected_quest_id) + " is selected, but already active.");
        }
        else
        {
            }
        dialog_manager.start_dialog(dialog_position, Dialogs.conversations["mission"][selected_quest_id]);
        }
        else if (selected_quest_id == -1)
        {
            }
        dialog_manager.start_dialog(dialog_position, [Dialogs.conversations["greeting"].pick_random()]);

        }
    }

    public void delete()
    {
    npcs.erase(this);
    queue_free();

    }

    public void _on_timer_timeout()
    {
    reload_missions();
    }

}