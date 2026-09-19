using Godot;
using Godot.Collections;
using System.Collections.Generic;

[GlobalClass]
public partial class Dialogs : RefCounted
{
    public static readonly Dictionary Conversations = new Dictionary
    {
        {
            "greeting", new Array
            {
                "Hey!",
                "Hello there!",
                "Yo!",
                "Ahoy!",
                "Hiya!",
                "Greetings!",
                "Howdy do!",
                "Hello there.",
                "Hi!",
                "Have a nice day.",
                "How are you?",
                "How's it going?",
                "Welcome back!",
                "Hey there, astronaut!",
                "Saluton, spaceman!",
                "Enjoy your stay!",
                "What do you need?",
                "Can I help you with something?",
                "Seen anything suspicious?",
                "Seen anything unusual?",
                "What do you think about space politics?",
                "What's the best part of space in your opinion?"
            }
        },
        {
            "mission", new Dictionary
            {
                {
                    0, new Array
                    {
                        "Hey, I need something brought back..",
                        "I dropped a chip somewhere that had my powidl tea recipe on it..",
                        "Could you bring it to me?",
                        0,
                        "Thank you! I can't wait to make my tea again."
                    }
                },
                {
                    1, new Array
                    {
                        "Hey!",
                        "You look like a traveler.. I need something..",
                        "I lost a chip that had family photos from several generations.",
                        "Careless, right? .. Anyway, could you bring it back? There's a reward in it for you!",
                        1,
                        "Thank you!"
                    }
                },
                {
                    2, new Array
                    {
                        "Hey there!",
                        "I need your help. Our station picked up a signal from an unknown source..",
                        "Could you find out who or what is sending it?",
                        "If you meet anyone nearby, check them out - alien, explorer, or even a lost satellite.",
                        "Once you find the source, come back. Of course, I won't forget to reward you!",
                        2,
                        "I'll be waiting for your return!"
                    }
                },
                {
                    3, new Array
                    {
                        "Yo, I need help with something..",
                        "I can see you've got the courage to help a crew in distress.",
                        "The situation's bad. Engines are down and vital systems are heavily damaged.",
                        "I need the tool KL94 of size 10. Spare parts might be on our sister ship, but beware - size 10 tends to go missing.",
                        "You'll need steady hands. Come back once you have it!",
                        3
                    }
                },
                {
                    4, new Array
                    {
                        "Yo.",
                        "I'm in deep trouble, someone stole my encrypted disk with ultra-classified data.",
                        "That disk belongs to a wealthy officer who tasked me with finding it.",
                        "Will you help me?",
                        "Find my brother. He'll tell you how to proceed, but no one else can know.",
                        "If the wrong people find out, we're done for.",
                        "And the reward? We've got plenty of energy units to pay you well.",
                        "So, what do you say?",
                        4,
                        "Thanks a lot!"
                    }
                },
                {
                    5, new Array
                    {
                        "Hey.",
                        "I lost my vape.",
                        "I won't last many more days without it.",
                        "Could you be so kind and bring it back?",
                        5,
                        "Thanks."
                    }
                },
                {
                    6, new Array
                    {
                        "Hey savior! It's me!",
                        "I'll never forget what you did for me..",
                        "I lost something again..",
                        "This time it's a pack of candy..",
                        "Would you save me again..?",
                        6,
                        "Thanks!"
                    }
                },
                {
                    7, new Array
                    {
                        "Hey, I need your help!",
                        "Our navigation systems were disrupted by a magnetic storm and we can't determine our position.",
                        "Could you find a spare compass or some device that can help us navigate?",
                        "It's urgent. Without it, we're blind in space.",
                        7,
                        "Come back with it ASAP, there's a reward waiting!"
                    }
                }
            }
        },
        {
            "mission_finished", new Dictionary
            {
                {
                    -1, new Array
                    {
                        "Thank you!"
                    }
                },
                {
                    4, new Array
                    {
                        "Hey. You must be the one who's supposed to help us find the disk.",
                        "My brother told me about you. Hope you're not scared - it won't be easy.",
                        4001
                    }
                },
                {
                    4001, new Array
                    {
                        "Thank you so much!",
                        "You have no idea what this means to us.",
                        "We're forever in your debt."
                    }
                },
                {
                    5, new Array
                    {
                        "You're my savior!",
                        "Thanks!",
                        "You can.. go now..",
                        ". . . . .",
                        "You're still here?",
                        "Well.. I lost something else..",
                        "Could you bring me the gum I left on the ship next door?",
                        5001,
                        "Thanks again!"
                    }
                },
                {
                    5001, new Array
                    {
                        "You actually found them!",
                        "Thanks again! You're my double savior."
                    }
                },
                {
                    6, new Array
                    {
                        "Awesome!",
                        "I knew I could count on you!"
                    }
                }
            }
        }
    };

    /// <summary>
    /// Returns a random phrase from the given dialog key.
    /// </summary>
    public static string RandomPhrase(string dialogType)
    {
        if (!Conversations.ContainsKey(dialogType)) return string.Empty;

        var entry = Conversations[dialogType];
        if (entry.VariantType == Variant.Type.Array)
        {
            var arr = entry.AsGodotArray();
            if (arr.Count == 0) return string.Empty;
            return arr.PickRandom().AsString();
        }

        return string.Empty;
    }

    /// <summary>
    /// Returns a random available task ID matching specified roles.
    /// Returns -1 if an empty chance triggered, or -2 if no valid tasks exist.
    /// </summary>
    public static int RandomTaskId(Array roles = null, bool canReturnEmptyTask = false)
    {
        var random = new RandomNumberGenerator();
        random.Randomize();

        if (canReturnEmptyTask && random.RandiRange(0, 3) == 0)
        {
            return -1;
        }

        roles ??= new Array();

        var usableTasks = new List<GodotObject>();
        var questManager = (Node)((SceneTree)Engine.GetMainLoop()).Root.GetNodeOrNull("QuestManager");
        if (questManager == null) return -2;

        var tasksDict = questManager.Get("tasks").AsGodotDictionary();
        var activeQuests = questManager.Get("active_quests");

        foreach (var taskKey in tasksDict.Keys)
        {
            var task = (GodotObject)tasksDict[taskKey];
            bool isFollowup = (bool)task.Get("is_followup_task");
            var reqRole = task.Get("required_role");

            bool roleMatch = roles.Contains(reqRole) || (int)reqRole == (int)NPC.Roles.None;
            if (!isFollowup && roleMatch)
            {
                int timesActivated = (int)task.Get("times_activated");
                int worldLimit = (int)task.Get("world_limit");

                if (timesActivated < worldLimit || worldLimit < 0)
                {
                    int taskId = (int)task.Get("id");
                    bool isActive = false;

                    if (activeQuests.VariantType == Variant.Type.Dictionary)
                    {
                        isActive = activeQuests.AsGodotDictionary().ContainsKey(taskId);
                    }
                    else if (activeQuests.VariantType == Variant.Type.Array)
                    {
                        isActive = activeQuests.AsGodotArray().Contains(taskId);
                    }

                    if (!isActive)
                    {
                        usableTasks.Add(task);
                    }
                }
            }
        }

        if (usableTasks.Count == 0)
        {
            if (!canReturnEmptyTask)
            {
                GD.PushWarning($"Warning: No tasks available for roles: {roles}");
            }
            return -2;
        }

        int selectedIndex = random.RandiRange(0, usableTasks.Count - 1);
        return (int)usableTasks[selectedIndex].Get("id");
    }
}