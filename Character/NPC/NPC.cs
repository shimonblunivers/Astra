using Godot;
using System.Collections.Generic;
using Godot.Collections;

[GlobalClass]
public partial class NPC : Character
{
    public enum Roles
    {
        Civilian,
        None,
        Trusted,
        Captain,
        Addicted
    }

    public static readonly string[] Names = new string[]
    {
        "Kevin", "Lukáš", "Tomáš", "Jan", "Pavel", "Martin", "Jakub", "Michal",
        "Jiří", "Adam", "David", "Marek", "Petr", "Ondřej", "Filip", "Richard",
        "Robert", "Václav", "Matěj", "Aleš", "Daniel", "Josef", "Karel", "Vojtěch",
        "František", "Eduard", "Viktor", "Igor", "Radim", "Radek", "Lukas", "Dominik",
        "Jakub", "Rudolf", "Lukáš", "Emanuel", "Štěpán", "Jaroslav", "Michael", "Zdeněk",
        "Aleš", "Patrik", "Tom", "Albert", "Viktor", "Jiří", "Denis", "Pavel",
        "Igor", "Eduard", "Jan", "Luboš", "Šimon", "Teo", "Honza", "Vašek"
    };

    public static List<NPC> Npcs { get; } = new List<NPC>();

    public Vector2 Difference { get; set; } = Vector2.Zero;
    public Ship Ship { get; set; }
    public List<Roles> RoleList { get; set; } = new List<Roles>();

    public bool Interactable { get; set; } = false;
    public bool Hovering { get; set; } = false;
    public int Id { get; set; }

    public Variant Skin { get; set; }
    public Variant Hair { get; set; }

    private int _activeQuestId = -1;
    public int ActiveQuestId
    {
        get => _activeQuestId;
        set
        {
            if (value == -1)
            {
                ReloadMissions();
            }
            else
            {
                foreach (var npc in Npcs)
                {
                    if (npc.SelectedQuestId == value && npc != this)
                    {
                        npc.ReloadMissions();
                    }
                }
            }
            _activeQuestId = value;
            UpdateNametagColor();
        }
    }

    private int _selectedQuestId = -1;
    public int SelectedQuestId
    {
        get => _selectedQuestId;
        set
        {
            if (_activeQuestId != -1 || QuestManager.Instance?.IsObjective(this) == true)
            {
                _selectedQuestId = -1;
            }
            else
            {
                _selectedQuestId = value;
            }
            UpdateNametagColor();
        }
    }

    // Node references (assigned in _Ready)
    private Node _dialogManager;
    private Node2D _sprites;
    private Timer _timer;
    private Label _nametag;
    private AudioStreamPlayer2D _finishedQuestAudio;
    private Area2D _area;

    public override void _Ready()
    {
        _dialogManager = GetNode("DialogManager");
        _sprites = GetNode<Node2D>("Sprite");
        _timer = GetNode<Timer>("Timer");
        _nametag = GetNode<Label>("Nametag");
        _finishedQuestAudio = GetNode<AudioStreamPlayer2D>("FinishedQuest");
        _area = GetNode<Area2D>("Area");

        LegsOffset = Legs != null ? Legs.Position : Vector2.Zero;

        if (Skin.VariantType != Variant.Type.Nil && Skin.AsGodotArray().Count >= 5)
        {
            var arr = Skin.AsGodotArray();
            _sprites.Call("set_skin", arr[0], arr[1], arr[2], arr[3], arr[4]);
        }

        if (Hair.VariantType != Variant.Type.Nil && Hair.AsGodotArray().Count >= 2)
        {
            var arr = Hair.AsGodotArray();
            var hairNode = _sprites.GetNode<Sprite2D>("Hair");
            hairNode.Frame = (int)arr[0];
            hairNode.FlipH = (bool)arr[1];
        }

        Skin = _sprites.Get("skin");
        ReloadMissions();
    }

    public static int GetUid()
    {
        int checkId = 0;
        while (true)
        {
            if (GetNpc(checkId) == null)
            {
                return checkId;
            }
            checkId++;
        }
    }

    public static NPC GetNpc(int targetId)
    {
        foreach (var npc in Npcs)
        {
            if (npc.Id == targetId) return npc;
        }
        return null;
    }

    public void Init(int targetId = -1, string targetNickname = null, List<Roles> newRoles = null, Variant? newSkin = null, Variant? newHair = null)
    {
        RoleList = newRoles ?? new List<Roles> { Roles.Civilian };
        Skin = newSkin ?? default;
        Hair = newHair ?? default;

        if (string.IsNullOrEmpty(targetNickname))
        {
            targetNickname = Names[GD.Randi() % Names.Length];
        }

        if (targetId != -1 && GetNpc(targetId) == null)
        {
            Id = targetId;
        }
        else
        {
            Id = GetUid();
        }

        if (Id == 0 && Skin.VariantType == Variant.Type.Nil)
        {
            targetNickname = "Captain " + targetNickname;
            RoleList.Add(Roles.Captain);
        }

        Nickname = targetNickname;

        if (OS.IsDebugBuild())
        {
            Nickname += " " + Id;
        }

        if (_nametag != null)
        {
            _nametag.Text = Nickname;
        }
        Name = $"NPC_{Nickname}_{Id}";

        if (!Npcs.Contains(this))
        {
            Npcs.Add(this);
        }
    }

    public void UpdateNametagColor()
    {
        if (_nametag == null) return;

        if (QuestManager.Instance?.IsObjective(this) == true)
        {
            _nametag.AddThemeColorOverride("font_outline_color", Quest.ObjectiveOfQuestOutlineColor);
            return;
        }
        if (_activeQuestId > -1)
        {
            _nametag.AddThemeColorOverride("font_outline_color", Quest.ActiveQuestOutlineColor);
            return;
        }
        if (_selectedQuestId > -1)
        {
            _nametag.AddThemeColorOverride("font_outline_color", Quest.TalkingAboutQuestOutlineColor);
            return;
        }
        _nametag.AddThemeColorOverride("font_outline_color", Quest.DefaultOutlineColor);
    }

    public void QuestFinished()
    {
        ActiveQuestId = -1;
        _finishedQuestAudio?.Play();
    }

    public void ReloadMissions(bool forced = false)
    {
        if (!forced && _activeQuestId != -1) return;

        var rolesArray = new Godot.Collections.Array();
        foreach (var role in RoleList)
        {
            rolesArray.Add((int)role);
        }

        var dialogs = ((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("Dialogs");
        int mission = -1;
        if (dialogs != null)
        {
            mission = (int)dialogs.Call("random_task_id", rolesArray, true);
        }

        if (mission < 0)
        {
            SelectedQuestId = -1;
        }
        else
        {
            SelectedQuestId = mission;
        }

        var random = new RandomNumberGenerator();
        _timer?.Start(120 + 60 * random.RandiRange(0, 8));

        UpdateNametagColor();
    }

    public void InPhysics(double delta)
    {
        if (Ship != null && _area != null)
        {
            _area.Position = new Vector2(0, -42.5f) + (-Ship.DifferenceInPosition).Rotated(-(float)GlobalRotation);
        }
    }

    public override void _Process(double delta)
    {
        if (_nametag != null)
        {
            _nametag.Visible = Hovering && Interactable;
        }
    }

    public void OnInteractionAreaAreaEntered(Area2D area)
    {
        if (area.IsInGroup("PlayerInteractArea"))
        {
            Interactable = true;
        }
    }

    public void OnInteractionAreaAreaExited(Area2D area)
    {
        if (area.IsInGroup("PlayerInteractArea"))
        {
            Interactable = false;
            _dialogManager?.Call("end_dialog");
        }
    }

    public void OnAreaMouseEntered() => Hovering = true;
    public void OnAreaMouseExited() => Hovering = false;

    public void OnAreaInputEvent(Node viewport, InputEvent @event, int shapeIdx)
    {
        if (@event is InputEventMouseButton mouseEvent && (mouseEvent.ButtonMask & MouseButtonMask.Left) != 0)
        {
            if (!Interactable || _dialogManager == null) return;

            bool isDialogActive = (bool)_dialogManager.Get("is_dialog_active");
            if (isDialogActive)
            {
                _dialogManager.Call("advance");
            }
            else
            {
                var dialogPosition = new Vector2(0, -105);
                var dialogsNode = ((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("Dialogs");
                var questMgr = QuestManager.Instance ?? (QuestManager)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("QuestManager");

                if (questMgr != null && questMgr.IsObjective(this))
                {
                    var quest = questMgr.GetQuestByTarget(this);
                    var taskObj = quest?.Get("task").AsGodotObject();
                    int taskId = taskObj != null ? (int)taskObj.Get("id") : -1;

                    var finishedConvs = (Dictionary)dialogsNode.Get("conversations").AsGodotDictionary()["mission_finished"];

                    if (finishedConvs.ContainsKey(taskId))
                    {
                        _dialogManager.Call("start_dialog", dialogPosition, finishedConvs[taskId]);
                    }
                    else
                    {
                        _dialogManager.Call("start_dialog", dialogPosition, finishedConvs[-1]);
                    }

                    questMgr.FinishedQuestObjective(quest);
                }
                else if (SelectedQuestId >= 0)
                {
                    var activeTaskIds = questMgr != null ? (Godot.Collections.Array)questMgr.Get("active_task_ids") : null;
                    if (activeTaskIds != null && activeTaskIds.Contains(SelectedQuestId))
                    {
                        GD.Print($"Warning: Task {SelectedQuestId} is selected, but already active.");
                    }
                    else if (dialogsNode != null)
                    {
                        var missionConvs = (Dictionary)dialogsNode.Get("conversations").AsGodotDictionary()["mission"];
                        _dialogManager.Call("start_dialog", dialogPosition, missionConvs[SelectedQuestId]);
                    }
                }
                else if (SelectedQuestId == -1 && dialogsNode != null)
                {
                    var greetings = (Godot.Collections.Array)dialogsNode.Get("conversations").AsGodotDictionary()["greeting"];
                    var randomGreeting = greetings.PickRandom();
                    _dialogManager.Call("start_dialog", dialogPosition, new Godot.Collections.Array { randomGreeting });
                }
            }
        }
    }

    public void Delete()
    {
        Npcs.Remove(this);
        QueueFree();
    }

    public void OnTimerTimeout()
    {
        ReloadMissions();
    }
}