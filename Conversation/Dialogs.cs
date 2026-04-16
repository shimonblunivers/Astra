using Godot;
using Godot.Collections;

public partial class Dialogs : GodotObject
{
    public static readonly Dictionary Conversations = new Dictionary
    {
        {
            "greeting", new Array
            {
                "Ahoj!",
                "Zdravíčko!",
                "Zdar!",
                "Ahoy!",
                "Nazdar!",
                "Zdravím!",
                "Čauves.",
                "Ahojda.",
                "Ahojky!",
                "Pěkný den přeji.",
                "Jak se máš?",
                "Jak to jde?",
                "Vítej zpátky!",
                "Nazdar, kosmonaute!",
                "Saluton, spaciano!",
                "Přeji příjemný pobyt!",
                "Co potřebuješ?",
                "Můžu ti nějak pomoct?",
                "Viděl jsi něco podezřelého?",
                "Viděl jsi něco neobvyklýho?",
                "Co si myslíš o vesmírné politice?",
                "Co je podle tebe nejlepší část vesmíru?",
            }
        },
        {
            "mission", new Dictionary
            {
                {
                    0, new Array
                    {
                        "Ahoj, něco bych potřeboval přinést..",
                        "Někde mi vypadnul čip, na kterým jsem měl recept na povidlový čaj..",
                        "Přinesl bys mi ho?",
                        0,
                        "Děkuji! Už se těším až si budu moct opět uvařit svůj čaj.",
                    }
                },
                {
                    1, new Array
                    {
                        "Ahoj!",
                        "Vypadáš jako cestovatel.. něco bych potřeboval..",
                        "Ztratil jsem čip, na kterém jsem měl rodinné fotky několika mých generací.",
                        "Nezodpovědné, že? .. Každopádně, mohl bys mi ho přinést? Odměna tě nemine!",
                        1,
                        "Děkuji ti!",
                    }
                },
                {
                    2, new Array
                    {
                        "Nazdárek!",
                        "Potřebuji tvou pomoc. Naše stanice zachytila signál z neznámého zdroje..",
                        "Mohl bys prosím zjistit kdo, nebo co, vysílá tento signál?",
                        "Pokud potkáš někoho poblíž, prověř ho, ať už je to mimozemšťan, průzkumník nebo dokonce ztracená družice.",
                        "Jakmile najdeš zdroj signálu, vrať se. Samozřejmě ti nezapomenu dát nějakou tu odměnu!",
                        2,
                        "Budu očekávat tvůj návrat!",
                    }
                },
                {
                    3, new Array
                    {
                        "Zdar, potřebuju s něčím pomoct..",
                        "Vidím, že máš odvahu pomoci cizí posádce v nouzi.",
                        "Situace není moc dobrá. Motory jsou mimo provoz a životně důležité systémy jsou vážně rozbity.",
                        "Potřebuju KL94 velikosti 10, náhradní díly by mohly být v naší sesterské lodi, ale musím tě varovat, desítka se často ztrácí.",
                        "Budeš potřebovat opatrné ruce. Až to budeš mít tak se vrať!",
                        3,
                    }
                },
                {
                    4, new Array
                    {
                        "Zdar.",
                        "Jsem ve velkým průšvihu, ukradli mi kódovanej disk s ultra-utajenejma datama.",
                        "Ten disk patří bohatýmu důstojníkovi, kterej mě pověřil, abych ho našel.",
                        "Pomohl bys mi?",
                        "Najdi mýho bráchu. On ti řekne, jak pokračovat, ale nikdo se o tom nesmí dozvědět.",
                        "Jestli se o tom dozví špatní lidi, je s náma konec.",
                        "A ohledně odměny? Máme dost energetickejch jednotek, abychom tě pořádně odměnili.",
                        "Co ty na to?",
                        4,
                        "Díky moc!",
                    }
                },
                {
                    5, new Array
                    {
                        "Ahoj.",
                        "Ztratil jsem svoji elektrickou cigaretu.",
                        "Víc dní to bez ní nedám.",
                        "Byl bys tak ochotnej a přinesl bys mi ji?",
                        5,
                        "Díkec.",
                    }
                },
                {
                    6, new Array
                    {
                        "Ahoj zachránče! To jsem já!",
                        "Nikdy ti nezapomenu, co jsi pro mě udělal..",
                        "Zase jsem něco ztratil..",
                        "Tentokrát jsem ztratil balíček bonbónů, zachránil bys mě zase?",
                        6,
                        "Díkec!",
                    }
                },
                {
                    7, new Array
                    {
                        "Ahoj, potřebuji tvou pomoc!",
                        "Naše navigační systémy byly narušeny magnetickou bouří a nemůžeme určit naši polohu.",
                        "Mohl bys najít náhradní kompas nebo zařízení schopné nám pomoct zorientovat se?",
                        "Je to naléhavé. Bez něj jsme ve vesmíru jako slepí.",
                        7,
                        "Vrať se s ním co nejdříve, čeká tě tu odměna!",
                    }
                },
            }
        },
        {
            "mission_finished", new Dictionary
            {
                {
                    -1, new Array
                    {
                        "Děkuji!",
                    }
                },
                {
                    4, new Array
                    {
                        "Ahoj. Ty budeš ten, který nám má pomoct s nalezením disku.\n\t\t\t\tBrácha mi o tobě říkal. Doufám že se nebojíš, nebude to lehké.",
                        4001,
                    }
                },
                {
                    4001, new Array
                    {
                        "Díky moc!",
                        "Ani nevíš, co pro nás tohle znamená.",
                        "Jsme ti navždy zavázáni.",
                    }
                },
                {
                    5, new Array
                    {
                        "Jsi můj zachránce!",
                        ". . .",
                        "Ty tu ještě jseš?",
                        "No.. ztratil jsem ještě něco..",
                        "Přinesl bys mi ještě žvýkačky, co jsem zapomněl na lodi vedle?",
                        5001,
                        "Díkec znovu!",
                    }
                },
                {
                    5001, new Array
                    {
                        "Ty jsi je fakt našel!",
                        "Dík zas! Jsi můj dvojnásobný zachránce.",
                    }
                },
                {
                    6, new Array
                    {
                        "Super!",
                        "Věděl jsem, že se na tebe mohu spolehnout!",
                    }
                },
            }
        },
    };

    /// <summary>
    /// <param name="dialogType">Key from <see cref="Conversations"/>.</param>
    /// <returns>Random phrase from the given dialog type.</returns>
    /// </summary>
    public static string RandomPhrase(string dialogType)
    {
        if (!Conversations.ContainsKey(dialogType)) return "";

        var list = (Array)Conversations[dialogType];
        var random = new RandomNumberGenerator();
        return list[random.RandiRange(0, list.Count - 1)].AsString();
    }

    /// <summary>
    /// <param name="roles">Array of <see cref="NPC.Roles"/> that the task should have.</param>
    /// <param name="canReturnEmptyTask">Whether the function has a chance to return -1.</param>
    /// <returns>
    /// Random task ID that is not blocked in <see cref="QuestManager.ActiveQuests"/>.
    /// Returns -2 if no task is available.
    /// </returns>
    /// </summary>
    public static int RandomTaskId(Array roles = null, bool canReturnEmptyTask = false)
    {
        roles ??= new Array();
        var random = new RandomNumberGenerator();

        if (canReturnEmptyTask && random.RandiRange(0, 3) == 0) return -1;

        var usableTasks = new Array();
        foreach (Variant taskKey in QuestManager.Tasks.Keys)
        {
            var task = QuestManager.Tasks[taskKey];
            // Adjust property access to match your converted Task class
            if (!task.AsGodotObject<QuestTask>().IsFollowupTask
                && (roles.Contains(task.AsGodotObject<QuestTask>().RequiredRole)
                    || task.AsGodotObject<QuestTask>().RequiredRole == NPC.Roles.None))
            {
                var t = task.AsGodotObject<QuestTask>();
                if (t.TimesActivated < t.WorldLimit || t.WorldLimit < 0)
                {
                    if (!QuestManager.ActiveQuests.Contains(t.Id))
                    {
                        usableTasks.Add(task);
                    }
                }
            }
        }

        if (usableTasks.Count == 0)
        {
            if (!canReturnEmptyTask)
                GD.PrintErr("Warning: No tasks available for roles: " + roles.ToString());
            return -2;
        }

        int randomIndex = random.RandiRange(0, usableTasks.Count - 1);
        return usableTasks[randomIndex].AsGodotObject<QuestTask>().Id;
    }
}
