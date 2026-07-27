using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace AmongUsSalem.Roles;

public sealed class Wildling(IntPtr cppPtr) : CovenRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Wildling";
    public string revealText => "has keen senses.";
    public string RoleDescription => "Town Of Salem 2";
    public string RoleLongDescription => "You are a creature of myth who has highly heightened senses.";
    public Color RoleColor { get; set; } = RoleColors.Coven;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.Coven;
    public Alignment Alignment => Alignment.CovenUtility;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;
    public CustomRoleConfiguration Configuration => new(this)
    {
        // CanUseSabotage = OptionGroupSingleton<CovenOptions>.Instance.CanSabotage,
        CanUseVent = OptionGroupSingleton<CovenOptions>.Instance.CanVent,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        Icon = AUSAssets.WildlingRoleCard
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ICustomAURole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return 
            $"Attack: {Attack}\n" +
            $"Defense: {Defense}\n" +
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can gather information for the Coven by figuring out who visits where, seeing whispers, as well as being a good way to fake claim roles that track visits.\n" +
            "Kill all who would oppose the Coven." + 
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return
            $"- With the Necronomicon, you will also deal a Basic Attack to your targets.\n" +
            $"- With your heightened senses, you can see whispers.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Sense",
            "You can Sense a player at Night.\n" +
            "You will see who your target visits and all players that visit your target. Astral abilities bypass this.",
            AUSAssets.Wildling_Sense),
    ];


    public bool WinConditionMet() => CovenGameOver.WinConditionMet(this);
    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet() || CovenGameOver.AnyCovenWon(gameOverReason);
    }

    public override void Initialize(PlayerControl player) // This patches the ability sprite since it's loaded on runtime.
    {
        RoleBehaviourStubs.Initialize(this, player);

        var opt = OptionGroupSingleton<Wildling_Options>.Instance;
        if (opt.SenseMode == (int)LookoutWatchMode.Camouflage) Player.RpcAddModifier<Camouflage>();
    }

    public void Role_OnMeetingStart()
    {
        if (Player.AmOwner())
        {
            Player.AddModifier<TI>();

            // --- LOOKOUT ---
            if (LookoutVisitedInfo.Count == 0)
            {
                foreach (var watched in ModifierUtils.GetPlayersWithModifier<WatchedModifier>(x => x.Caster == Player))
                {
                    Player.Notify(Lookout.Info(NotificationType.Lookout_NoOneVisited, watched, watched), NotifyMode.OnlyMeeting, sprite: AUSAssets.AgentRoleCard.LoadAsset());
                }
            }
            else
            {
                foreach (var info in LookoutVisitedInfo)
                {
                    Player.Notify(Lookout.Info(NotificationType.Lookout_Visited, info.Item1, info.Item2), NotifyMode.OnlyMeeting, sprite: AUSAssets.AgentRoleCard.LoadAsset());
                    if (info.Item1.HasDied() && MeetingHud.Instance.reporterId != info.Item2.PlayerId) info.Item2.RpcAddModifier<IncriminatingEvidence>();
                }
            }

            // --- TRACKER ---
            if (TrackerVisitedInfo.Count == 0 && TrackerDoubleVisitedInfo.Count == 0)
            {
                foreach (var tracked in ModifierUtils.GetPlayersWithModifier<TrackedModifier>(x => x.Caster == Player))
                {
                    Player.Notify(Lookout.Info(NotificationType.Tracker_TargetDV, tracked, tracked), NotifyMode.OnlyMeeting, sprite: AUSAssets.AgentRoleCard.LoadAsset());
                }
            }
            else
            {
                foreach (var info in TrackerVisitedInfo)
                {
                    Player.Notify(Tracker.Info(NotificationType.Tracker_TargetVisited, info.Item1, info.Item2), NotifyMode.OnlyMeeting, sprite: AUSAssets.AgentRoleCard.LoadAsset());
                    if (info.Item2.HasDied() && MeetingHud.Instance.reporterId != info.Item1.PlayerId) info.Item1.RpcAddModifier<IncriminatingEvidence>();
                }
                foreach (var info in TrackerDoubleVisitedInfo)
                {
                    Player.Notify(Tracker.Info(NotificationType.Tracker_TargetVisited, info.Item1, info.Item2.Item1, info.Item2.Item2), NotifyMode.OnlyMeeting, sprite: AUSAssets.AgentRoleCard.LoadAsset());
                    info.Item1.RpcAddModifier<Confirmed>();
                }
            }

            TrackerDoubleVisitedInfo.Clear();
            TrackerVisitedInfo.Clear();
            LookoutVisitedInfo.Clear();
        }
    }

    public List<(PlayerControl, PlayerControl)> LookoutVisitedInfo = new List<(PlayerControl, PlayerControl)>();
    public List<(PlayerControl, PlayerControl)> TrackerVisitedInfo = new List<(PlayerControl, PlayerControl)>();
    public List<(PlayerControl, (PlayerControl, PlayerControl))> TrackerDoubleVisitedInfo = new List<(PlayerControl, (PlayerControl, PlayerControl))>();

    public void Function(PlayerControl target, int Button)
    {
        if (Button == 1)
        {
            target.RpcAddModifier<WatchedModifier>(Player);
            target.RpcAddModifier<TrackedModifier>(Player);
            if (Player.HasNecronomicon())
            {
                if (Player.CanKill(target))
                {
                    Player.RpcCustomMurder(target);
                    VisitingMechanic.RpcAddDeathReason(target, (int)DeathReasonShow.KilledByTheCoven);
                }
                else Player.Notify(Feedback.TooMuchDefense(Player, target), NotifyMode.InstantlyAndMeeting);
            }
        }
    }
}

public sealed class Wildling_Sense : TownOfUsRoleButton<Wildling, PlayerControl>
{
    public override string Name => "Sense";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Coven;
    public override float Cooldown => Player.HasNecronomicon() ? OptionGroupSingleton<CovenOptions>.Instance.Cooldown :
        OptionGroupSingleton<Wildling_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Wildling_Sense;

    protected override void OnClick() => VisitingMechanic.CheckVisit(Player, Target, 1, Player.HasNecronomicon(), true);
    public override PlayerControl? GetTarget()
    {
        if (Player.HasNecronomicon())
            return Player.GetClosestLivingPlayer(true, Distance, predicate: x =>
                !x.Is(Faction.Coven));

        return Player.GetClosestLivingPlayer(true, Distance, predicate: x =>
            !x.HasModifier<WatchedModifier>(x => x.Caster == Player) &&
            !x.HasModifier<TrackedModifier>(x => x.Caster == Player));
    }
}

public sealed class Wildling_Options : AbstractOptionGroup<Wildling>
{
    public override string GroupName => "Wildling";

    [ModdedNumberOption("Wildling Sense Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    public ModdedEnumOption SenseMode { get; set; } = new("Wildling Sense Mode",
        (int)LookoutWatchMode.None, typeof(LookoutWatchMode), ["None", "Camouflage"]);
}