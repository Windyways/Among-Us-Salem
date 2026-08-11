using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using System.Collections;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace AmongUsSalem.Roles;

public sealed class Werewolf(IntPtr cppPtr) : NeutralRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Werewolf";
    public string revealText => "howls at the moon.";
    public string RoleDescription => "Town Of Salem 2";
    public string RoleLongDescription => "You are a regular townie who transforms under the full moon.";
    public Color RoleColor { get; set; } = RoleColors.Werewolf;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.Werewolf;
    public Alignment Alignment => Alignment.NeutralKilling;

    public Attack Attack { get; set; } = Attack.Powerful;
    public Defense Defense { get; set; } = Defense.Basic;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.Powerful;
    public Defense ogDefense { get; set; } = Defense.Basic;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;
    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<Werewolf_Options>.Instance.CanVent,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        Icon = AUSAssets.WerewolfRoleCard
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
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can permanently Track a player on half moon Nights, and Rampage players at full moon Nights.\n" +
            "Kill everyone in the town. You will win with other Werewolves." + 
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Track Scent",
            "You can Track Scent a player at Night.\n" +
            "You will permanently see who your target visits throughout the game. Astral abilities are not seen visiting. You can only Track Scent on Half Moon Nights.",
            AUSAssets.Werewolf_TrackScent),

        new("Maul",
            "You can Maul a player at Night.\n" +
            "You will deal a Powerful Attack to your target and Rampage nearby players. You can only Maul on Full Moon Nights.\n" +
            "If you are RoleBlocked on a Full Moon Night, or you are visited before attacking, you will counterattack your visitor with a Powerful Attack and Rampage nearby players.",
            AUSAssets.Werewolf_Maul),
    ];

    public static bool AnyWon(GameOverReason gameOverReason)
    {
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player.IsRole<Werewolf>() && NeutralGameOver.WinConditionMet(player.Data.Role)) return true;
        }
        return false;
    }

    public bool WinConditionMet() => NeutralGameOver.WinConditionMet(this);
    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet() || AnyWon(gameOverReason);
    }

    public void Role_OnMeetingStart()
    {
        if (Player.AmOwner())
        {
            Player.AddModifier<TI>();
            if (VisitedInfo.Count > 0 || DoubleVisitedInfo.Count > 0)
            {
                foreach (var info in VisitedInfo)
                {
                    Player.Notify(Tracker.Info(NotificationType.Tracker_TargetVisited, info.Item1, info.Item2), NotifyMode.OnlyMeeting, sprite: AUSAssets.TrackerRoleCard.LoadAsset());
                    if (info.Item2.HasDied() && MeetingHud.Instance.reporterId != info.Item1.PlayerId) info.Item1.RpcAddModifier<IncriminatingEvidence>();
                }
                foreach (var info in DoubleVisitedInfo)
                {
                    Player.Notify(Tracker.Info(NotificationType.Tracker_TargetVisited, info.Item1, info.Item2.Item1, info.Item2.Item2), NotifyMode.OnlyMeeting, sprite: AUSAssets.TrackerRoleCard.LoadAsset());
                    info.Item1.RpcAddModifier<Confirmed>();
                }
            }

            DoubleVisitedInfo.Clear();
            VisitedInfo.Clear();
        }
    }

    public int PerformInteraction(PlayerControl visitor)
    {
        if (!DayNightMechanic.FullMoon() || Player.HasModifier<MauledModifier>())
            return 0;

        if (Player.AmOwner) CustomButtonSingleton<Werewolf_Maul>.Instance.ResetCooldownAndOrEffect();

        Player.RpcAddModifier<MauledModifier>(Player);
        Player.Rampage(visitor, DeathReasonShow.MauledByAWerewolf, lunge: !MeetingHud.Instance);
        return 0; // Do not Block visits.
    }

    public List<(PlayerControl, PlayerControl)> VisitedInfo = new List<(PlayerControl, PlayerControl)>();
    public List<(PlayerControl, (PlayerControl, PlayerControl))> DoubleVisitedInfo = new List<(PlayerControl, (PlayerControl, PlayerControl))>();
    public void Function(PlayerControl target, int Button)
    {
        if (Button is 1)
        {
            target.RpcAddModifier<TrackedModifier>(Player);
        }
        else if (Button is 2)
        {
            Player.RpcAddModifier<MauledModifier>(Player);
            Player.Rampage(target, DeathReasonShow.MauledByAWerewolf);
        }
    }
}

public sealed class Werewolf_TrackScent : TownOfUsRoleButton<Werewolf, PlayerControl>
{
    public override string Name => "Track Scent";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Werewolf;
    public override float Cooldown => OptionGroupSingleton<Werewolf_Options>.Instance.TrackScentCD;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Werewolf_TrackScent;

    protected override void OnClick() => VisitingMechanic.CheckVisit(Player, Target, 1, false, true);
    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance, predicate: x =>
            !x.HasModifier<TrackedModifier>(x => x.Caster == Player));
    }

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && DayNightMechanic.HalfMoon();
    }
}

public sealed class Werewolf_Maul : TownOfUsRoleButton<Werewolf, PlayerControl>
{
    public override string Name => "Maul";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Werewolf;
    public override float Cooldown => OptionGroupSingleton<Werewolf_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Werewolf_Maul;

    protected override void OnClick() => VisitingMechanic.CheckVisit(Player, Target, 2, true, true);
    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance);
    }

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && DayNightMechanic.FullMoon();
    }
}

public sealed class Werewolf_Options : AbstractOptionGroup<Werewolf>
{
    public override string GroupName => "Werewolf";

    [ModdedNumberOption("Werewolf Track Scent Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float TrackScentCD { get; set; } = 25f;

    [ModdedNumberOption("Werewolf Maul Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedToggleOption("Werewolf Can Vent")]
    public bool CanVent { get; set; } = true;
}