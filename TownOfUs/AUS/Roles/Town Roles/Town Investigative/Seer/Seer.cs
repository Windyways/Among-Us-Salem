using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace AmongUsSalem.Roles;

public sealed class Seer(IntPtr cppPtr) : CrewmateRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Seer";
    public string revealText => "can see into the hearts of people to find out their intentions.";
    public string RoleDescription => "Town Of Salem 2";
    public string RoleLongDescription => "You are able to see into the hearts of townies to find out their intentions.";
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
        Icon = AUSAssets.SeerRoleCard
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var info = ICustomAURole.SetNewTabText(this);

        // Only show info if we have data
        if (Information.Count > 0)
        {
            info.AppendLine();
            foreach (var searched in Information)
            {
                string intuitName = searched.Item1.Item1.Name();
                string gazeName = searched.Item1.Item2.Name();

                string result = searched.Item2 ? "<color=#FF0000>Enemies!</color>" : "<color=#00FF00>Friends!</color>";
                info.AppendLine($"{intuitName} & {gazeName} - {result}");
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
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can check if two players are on opposing factions or not, being a huge threat to Coven and Mafia.\n" +
            "Hang every criminal and evildoer." +
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return
            $"- Illusionists can make your target appear innocent.\n" +
            $"- Enchanters, Warlocks, Soul Collectors, and Framers can make your target appear suspicious.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Intuit",
            "You can Intuit a player at Night.\n" +
            "You will see if your target and your Gaze target are in opposite or equal factions.\n" +
            "You can only Intuit/Gaze your target once.",
            AUSAssets.Seer_Intuit),

        new("Gaze",
            "You can Gaze a player at Night.\n" +
            "You will see if your target and your Intuit target are in opposite or equal factions.\n" +
            "You can only Intuit/Gaze your target once.",
            AUSAssets.Seer_Gaze),
    ];

    public static string Info(PlayerControl player, PlayerControl intuit, PlayerControl gaze)
    {
        player.AddModifier<TI>();
        if (IsFriends(intuit, gaze)) return $"You sense that {intuit.Name()} and {gaze.Name()} seem friendly with each other!";

        gaze.AddModifier<IncriminatingEvidence>();
        intuit.AddModifier<IncriminatingEvidence>();
        return $"You sense that {intuit.Name()} and {gaze.Name()} seem to be enemies!";
    }

    public static bool IsFriends(PlayerControl intuit, PlayerControl gaze)
    {
        var intuitAlign = GetSeerAlignment(intuit);
        var gazeAlign = GetSeerAlignment(gaze);

        // Neutral Pariah: friends with all non-town
        if (intuitAlign == SeerAlignment.NeutralPariah && gazeAlign != SeerAlignment.Town)
            return true;

        if (gazeAlign == SeerAlignment.NeutralPariah && intuitAlign != SeerAlignment.Town)
            return true;

        // Apocalypse
        if (intuitAlign == SeerAlignment.Apocalypse && gazeAlign == SeerAlignment.Apocalypse)
            return true;

        if (gazeAlign == SeerAlignment.Apocalypse && intuitAlign == SeerAlignment.Apocalypse)
            return true;

        // Unique neutrals only like themselves
        if (intuitAlign == SeerAlignment.UniqueNeutral || gazeAlign == SeerAlignment.UniqueNeutral)
            return intuit.Data.Role.NiceName == gaze.Data.Role.NiceName;

        // Same alignment
        if (intuitAlign == gazeAlign)
            return true;

        return false;
    }

    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);
        intuit = null;
        gaze = null;
        fullCooldown = true;
    }

    public static SeerAlignment GetSeerAlignment(PlayerControl player)
    {
        // Make sure modifiers have priority!
        if (player.HasModifier<IllusionedModifier>())
            return SeerAlignment.Town;

        if (player.HasModifier<FramedModifier>())
            return SeerAlignment.Mafia;

        if (player.HasModifier<WarlockFramedModifier>() || player.Is(Faction.Apocalypse))
            return SeerAlignment.Apocalypse;

        if (player.Is(Faction.Town) || player.IsRole<Jester>())
            return SeerAlignment.Town;

        if (player.Is(Faction.Mafia))
            return SeerAlignment.Mafia;

        if (player.Is(Faction.Coven))
            return SeerAlignment.Coven;

        if (player.Is(Alignment.NeutralPariah))
            return SeerAlignment.NeutralPariah;

        if (player.IsFactionNeutral())
            return SeerAlignment.UniqueNeutral;

        return SeerAlignment.None;
    }

    public PlayerControl intuit;
    public PlayerControl gaze;
    public bool fullCooldown = true;

    public List<((PlayerControl, PlayerControl), bool)> Information = new List<((PlayerControl, PlayerControl), bool)>();
    public void Function(PlayerControl target, int Button)
    {
        fullCooldown = false;
        if (Button is 1)
        {
            intuit = target;
        }
        else if (Button is 2)
        {
            gaze = target;
        }

        if (intuit != null && gaze != null)
        {
            if (VisitingMechanic.CheckVisit(Player, intuit, 1, false, true, false) &&
                VisitingMechanic.CheckVisit(Player, gaze, 2, false, true, false))
            {
                if (Player.TryGetModifier<TrackedModifier>(out var tracked)) TrackedModifier.RpcPerformDoubleInteraction(tracked.Caster, Player, intuit, gaze);
                Information.Add(((intuit, gaze), !IsFriends(intuit, gaze)));

                fullCooldown = true;
                intuit.AddModifier<ComparedModifier>(Player, gaze, IsFriends(intuit, gaze));
                gaze.AddModifier<ComparedModifier>(Player, intuit, IsFriends(intuit, gaze));
                Player.Notify(Info(Player, intuit, gaze), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.SeerRoleCard.LoadAsset());
            }

            intuit = null;
            gaze = null;
            CustomButtonSingleton<Seer_Intuit>.Instance.ResetCooldownAndOrEffect();
            CustomButtonSingleton<Seer_Gaze>.Instance.ResetCooldownAndOrEffect();
        }
    }
}

public sealed class Seer_Intuit : TownOfUsRoleButton<Seer, PlayerControl>
{
    public override string Name => "Intuit";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Town;
    public override float Cooldown => Role.fullCooldown ? OptionGroupSingleton<Seer_Options>.Instance.Cooldown : 1;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Seer_Intuit;

    // Seer is astral when picking targets, this is handled when it has both targets set.
    protected override void OnClick() => VisitingMechanic.CheckVisit(Player, Target, 1, false, false);
    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance, predicate: x =>
            !x.HasModifier<ComparedModifier>(x => x.Caster == Player) && x != Role.gaze && x != Role.intuit && !x.HasModifier<GlobalReveal>());
    }
}

public sealed class Seer_Gaze : TownOfUsRoleButton<Seer, PlayerControl>
{
    public override string Name => "Gaze";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => RoleColors.Town;
    public override float Cooldown => Role.fullCooldown ? OptionGroupSingleton<Seer_Options>.Instance.Cooldown : 1;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Seer_Gaze;

    // Seer is astral when picking targets, this is handled when it has both targets set.
    protected override void OnClick() => VisitingMechanic.CheckVisit(Player, Target, 2, false, false);
    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance, predicate: x =>
            !x.HasModifier<ComparedModifier>(x => x.Caster == Player) && x != Role.gaze && x != Role.intuit && 
            !(x.HasModifier<GlobalReveal>(x => x.Player.Is(Faction.Town)) && !OptionGroupSingleton<Seer_Options>.Instance.RevealedTownComparable));
    }
}

public sealed class Seer_Options : AbstractOptionGroup<Seer>
{
    public override string GroupName => "Seer";

    [ModdedNumberOption("Seer Intuit & Gaze Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedToggleOption("Seer Can Compare Revealed Town")]
    public bool RevealedTownComparable { get; set; } = false;
}