using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace AmongUsSalem.Roles;

public sealed class Agent(IntPtr cppPtr) : ImpostorRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Agent";
    public string revealText => "is good at stalking others.";
    public string RoleDescription => "Better Town Of Salem";
    public string RoleLongDescription => "You are a shady individual that gathers information for the mafia.";
    public Color RoleColor { get; set; } = RoleColors.Mafia;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;

    public Faction Faction { get; set; } = Faction.Mafia;
    public Alignment Alignment => Alignment.MafiaSupport;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;
    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = false,
        CanUseSabotage = false,
        Icon = AUSAssets.AgentRoleCard
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
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can gather information for the Mafia by figuring out who visits where, as well as being a good way to fake claim roles that track visits.\n" +
            "Kill anyone that will not submit to the Mafia." + 
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return
            $"- You have access to Mafia chat.\n" +
            $"- If there are no capable Mafia Killing roles, you will be promoted to Mafioso.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Stalk",
            "You can Stalk a player at Night.\n" +
            "You will see who your target visits and all players that visit your target. Astral abilities bypass this.",
            AUSAssets.Agent_Stalk),
    ];

    public static string Info(NotificationType type, PlayerControl player, PlayerControl target, PlayerControl target2 = null)
    {
        if (type == NotificationType.Agent_PlayerVisitedStalked) return $"{player.Name()} visited {target.Name()} last night!";
        if (type == NotificationType.Agent_StalkedVisited)
        {
            if (target2 != null) return $"<color=#dd0000>{player.Name()} visited {target.Name()} & {target2.Name()}!</color>";
            return $"<color=#dd0000>{player.Name()} visited {target.Name()} last night!</color>";
        }

        if (type == NotificationType.Agent_NoOneVisitedStalked) return $"No one visited {target.Name()} last night.";
        if (type == NotificationType.Agent_StalkedDidntVisit) return $"<color=#dd0000>{player.Name()} did not visit anyone last night.</color>";
        return "Error.";
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
                    Player.Notify(Info(NotificationType.Agent_NoOneVisitedStalked, watched, watched), NotifyMode.OnlyMeeting, sprite: AUSAssets.AgentRoleCard.LoadAsset());
                }
            }
            else
            {
                foreach (var info in LookoutVisitedInfo)
                {
                    Player.Notify(Info(NotificationType.Agent_PlayerVisitedStalked, info.Item1, info.Item2), NotifyMode.OnlyMeeting, sprite: AUSAssets.AgentRoleCard.LoadAsset());
                    if (info.Item1.HasDied() && MeetingHud.Instance.reporterId != info.Item2.PlayerId) info.Item2.RpcAddModifier<IncriminatingEvidence>();
                }
            }

            // --- TRACKER ---
            if (TrackerVisitedInfo.Count == 0 && TrackerDoubleVisitedInfo.Count == 0)
            {
                foreach (var tracked in ModifierUtils.GetPlayersWithModifier<TrackedModifier>(x => x.Caster == Player))
                {
                    Player.Notify(Info(NotificationType.Agent_StalkedDidntVisit, tracked, tracked), NotifyMode.OnlyMeeting, sprite: AUSAssets.AgentRoleCard.LoadAsset());
                }
            }
            else
            {
                foreach (var info in TrackerVisitedInfo)
                {
                    Player.Notify(Info(NotificationType.Agent_StalkedVisited, info.Item1, info.Item2), NotifyMode.OnlyMeeting, sprite: AUSAssets.AgentRoleCard.LoadAsset());
                    if (info.Item2.HasDied() && MeetingHud.Instance.reporterId != info.Item1.PlayerId) info.Item1.RpcAddModifier<IncriminatingEvidence>();
                }
                foreach (var info in TrackerDoubleVisitedInfo)
                {
                    Player.Notify(Info(NotificationType.Agent_StalkedVisited, info.Item1, info.Item2.Item1, info.Item2.Item2), NotifyMode.OnlyMeeting, sprite: AUSAssets.AgentRoleCard.LoadAsset());
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
        if (Button is 1)
        {
            target.RpcAddModifier<WatchedModifier>(Player);
            target.RpcAddModifier<TrackedModifier>(Player);
        }
    }
}

public sealed class Agent_Stalk : TownOfUsRoleButton<Agent, PlayerControl>
{
    public override string Name => "Stalk";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Mafia;
    public override float Cooldown => OptionGroupSingleton<Agent_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Agent_Stalk;

    protected override void OnClick() => VisitingMechanic.CheckVisit(Player, Target, 1, false, true);
    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(false, Distance, predicate: x =>
            !x.HasModifier<WatchedModifier>(x => x.Caster == Player) &&
            !x.HasModifier<TrackedModifier>(x => x.Caster == Player));
    }
}

public sealed class Agent_Options : AbstractOptionGroup<Agent>
{
    public override string GroupName => "Agent";

    [ModdedNumberOption("Agent Stalk Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}