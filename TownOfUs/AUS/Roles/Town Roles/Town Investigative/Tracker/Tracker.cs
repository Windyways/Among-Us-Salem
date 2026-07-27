using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace AmongUsSalem.Roles;

public sealed class Tracker(IntPtr cppPtr) : CrewmateRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Tracker";
    public string revealText => "is skilled in the art of tracking.";
    public string RoleDescription => "Town Of Salem 2";
    public string RoleLongDescription => "You are a skilled hunter who stalks your prey at night.";
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
        Icon = AUSAssets.TrackerRoleCard
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var info = ICustomAURole.SetNewTabText(this);
        return info;
    }

    public string GetAdvancedDescription()
    {
        return
            $"Attack: {Attack}\n" +
            $"Defense: {Defense}\n" +
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can see who their target visits, being a threat to evils fake claiming to visit.\n" +
            "Hang every criminal and evildoer." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Track",
            "You can Track a player at Night.\n" +
            "You will see who your target visits. Astral abilities are not seen visiting.",
            AUSAssets.Tracker_Track),
    ];

    public static string Info(NotificationType type, PlayerControl player, PlayerControl target, PlayerControl target2 = null)
    {
        if (type == NotificationType.Tracker_TargetDV) return $"{player.Name()} did not visit anyone last night.";

        if (target2 != null) return $"{player.Name()} visited {target.Name()} & {target2.Name()}!";
        return $"{player.Name()} visited {target.Name()}!";
    }

    public void Role_OnMeetingStart()
    {
        if (Player.AmOwner())
        {
            Player.AddModifier<TI>();
            if (VisitedInfo.Count == 0 && DoubleVisitedInfo.Count == 0)
            {
                foreach (var tracked in ModifierUtils.GetPlayersWithModifier<TrackedModifier>(x => x.Caster == Player))
                {
                    Player.Notify(Info(NotificationType.Tracker_TargetDV, tracked, tracked), NotifyMode.OnlyMeeting, sprite: AUSAssets.TrackerRoleCard.LoadAsset());
                }
            }
            else
            {
                foreach (var info in VisitedInfo)
                {
                    Player.Notify(Info(NotificationType.Tracker_TargetVisited, info.Item1, info.Item2), NotifyMode.OnlyMeeting, sprite: AUSAssets.TrackerRoleCard.LoadAsset());
                    if (info.Item2.HasDied() && MeetingHud.Instance.reporterId != info.Item1.PlayerId) info.Item1.RpcAddModifier<IncriminatingEvidence>();
                }
                foreach (var info in DoubleVisitedInfo)
                {
                    Player.Notify(Info(NotificationType.Tracker_TargetVisited, info.Item1, info.Item2.Item1, info.Item2.Item2), NotifyMode.OnlyMeeting, sprite: AUSAssets.TrackerRoleCard.LoadAsset());
                    info.Item1.RpcAddModifier<Confirmed>();
                }
            }

            DoubleVisitedInfo.Clear();
            VisitedInfo.Clear();
        }
    }

    public List<(PlayerControl, PlayerControl)> VisitedInfo = new List<(PlayerControl, PlayerControl)>();
    public List<(PlayerControl, (PlayerControl, PlayerControl))> DoubleVisitedInfo = new List<(PlayerControl, (PlayerControl, PlayerControl))>();
    public void Function(PlayerControl target, int Button)
    {
        if (Button is 1)
        {
            target.RpcAddModifier<TrackedModifier>(Player);
        }
    }
}

public sealed class Tracker_Track : TownOfUsRoleButton<Tracker, PlayerControl>
{
    public override string Name => "Track";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Town;
    public override float Cooldown => OptionGroupSingleton<Tracker_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Tracker_Track;

    protected override void OnClick() => VisitingMechanic.CheckVisit(Player, Target, 1, false, true);
    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance, predicate: x => 
            !x.HasModifier<TrackedModifier>(x => x.Caster == Player));
    }
}

public sealed class Tracker_Options : AbstractOptionGroup<Tracker>
{
    public override string GroupName => "Tracker";

    [ModdedNumberOption("Tracker Track Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}