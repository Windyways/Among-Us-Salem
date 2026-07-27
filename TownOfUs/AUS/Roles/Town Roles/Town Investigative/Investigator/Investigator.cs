using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace AmongUsSalem.Roles;

public sealed class Investigator(IntPtr cppPtr) : CrewmateRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Investigator";
    public string revealText => "gathers information about people.";
    public string RoleDescription => "Town Of Salem 2";
    public string RoleLongDescription => "You are a private eye who secretly gathers information.";
    public Color RoleColor { get; set; } = RoleColors.Town;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction Faction { get; set; } = Faction.Town;
    public Alignment Alignment => Alignment.TownInvestigative;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.InvestigatorRoleCard
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var info = ICustomAURole.SetNewTabText(this);

        // Only show info if we have data
        if (Information.Count > 0)
        {
            info.AppendLine();
            foreach (var investigated in Information)
            {
                string playerName = investigated.Item1.Name();

                string noCrime = "";
                string tres = investigated.Item2 ? "<color=#dddd00>Trespassing</color>" : "";
                string murd = investigated.Item3 ? "<color=#FF0000>Murder</color>" : "";

                if (!investigated.Item2 && !investigated.Item3) noCrime = "<color=#00ff00>No Crime</color>";
                info.AppendLine($"{playerName} - {noCrime} {murd} {tres}");
            }
        }
        else
        {
            info.AppendLine();
            info.AppendLine("No investigative results yet.");
        }

        return info;
    }

    public string GetAdvancedDescription()
    {
        return
            $"Attack: {Attack}\n" +
            $"Defense: {Defense}\n" +
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can check if a player has killed at Night or visited opposing factions. \n" +
            "Hang every criminal and evildoer." +
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return
            $"- Enchanters, Warlocks, Soul Collectors, and Framers can make your target appear to have Murder.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Investigate",
            "You will see if your target has killed during the same Night or visited an opposing faction during the game.",
            AUSAssets.Investigator_Investigate),
    ];

    public static bool IsMurder(PlayerControl target)
    {
        // if (target.HasModifier<IllusionedModifier>()) return false; // ???

        if (target.HasModifier<WarlockFramedModifier>()) return true;
        if (target.HasModifier<FramedModifier>()) return true;

        return target.HasModifier<MurderModifier>();
    }

    public static bool IsTrespassing(PlayerControl target)
    {
        return target.HasModifier<TrespassingModifier>();
    }

    public static string Info(PlayerControl player, PlayerControl target)
    {
        player.AddModifier<TI>();

        string feedback = string.Empty;
        if (IsTrespassing(target))
        {
            target.AddModifier<IncriminatingEvidence>();
            feedback += $"You discovered evidence that {target.Name()} is Trespassing!";
        }
        if (IsMurder(target))
        {
            target.AddModifier<IncriminatingEvidence>();
            feedback += $"\nYou found evidence of Murder in {target.Name()}'s house!";
        }

        if (feedback != string.Empty) return feedback;
        return $"You didn't find any evidence of a Crime in {target.Name()}'s house.";
    }

    public List<(PlayerControl, bool, bool)> Information = new List<(PlayerControl, bool, bool)>();
    public void Function(PlayerControl target, int Button)
    {
        if (Button is 1)
        {
            Information.Add((target, Investigator.IsTrespassing(target), Investigator.IsMurder(target)));
            Player.Notify(Investigator.Info(Player, target), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.InvestigatorRoleCard.LoadAsset());
        }
    }
}

public sealed class Investigator_Investigate : TownOfUsRoleButton<Investigator, PlayerControl>
{
    public override string Name => "Investigate";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Town;
    public override float Cooldown => OptionGroupSingleton<Investigator_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Investigator_Investigate;

    protected override void OnClick() => VisitingMechanic.CheckVisit(Player, Target, 1, false, true);
    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance);
    }
}

public sealed class Investigator_Options : AbstractOptionGroup<Investigator>
{
    public override string GroupName => "Investigator";

    [ModdedNumberOption("Investigator Investigate Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}