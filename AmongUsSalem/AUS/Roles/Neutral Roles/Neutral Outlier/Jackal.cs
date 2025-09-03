using System.Text;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
using System.Collections;
using Random = System.Random;
using MiraAPI.Events.Vanilla.Player;

namespace AmongUsSalem.LifeImprovement.Roles;

#region Jackal
#endregion
public sealed class Jackal(IntPtr cppPtr)
    : NeutralRole(cppPtr), IWikiDiscoverable, IAssignableTargets, IAUSRole
{
    public string RoleName { get; set; } = TouLocale.Get(TouNames.Jackal, "Jackal");
    public string revealText => "N/A";
    public string RoleDescription => "Placeholder.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction RoleFaction { get; set; } = Faction.Neutral;
    public Color RoleColor { get; set; } = AUSColors.Neutral;
    public Alignment Alignment => Alignment.NeutralOutlier;
    public Attack Attack { get; set; } = Attack.Powerful;
    public Defense Defense { get; set; } = Defense.Basic;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.Powerful;
    public Defense ogDefense { get; set; } = Defense.Basic;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.JackalRoleCard,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IAUSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"{AUSColors.GradientColorText("404040", "b8b8b8", "Jackal")}" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#A9A9A9>Neutral</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#A9A9A9>Neutral</color> <color=#1e45d4>Outlier</color>" +
            "\n<color=#fdbc00>Goal:</color> Kill everyone in the town." +
            $"\n\nAttributes:" +
            "\nN/A." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Assassinate",
            "N/A",
            AUSAssets.Jackal_Assassinate)
    ];

    public bool WinConditionMet()
    {
        var aliveJackals = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.IsRole<Jackal>());
        var aliveRecs = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.HasModifier<JackalRecruit>());
        if (aliveJackals == 0 && aliveRecs == 0) return false;

        var result = Helpers.GetAlivePlayers().Count <= (aliveJackals + aliveRecs) && MiscUtils.KillersAliveCount() == (aliveJackals + aliveRecs);
        return result;
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet();
    }

    public void OnTargetDeath(PlayerControl target, DeathReason? reason)
    {
        if (target.HasModifier<JackalRecruit>() && Player.AmOwner())
        {
            MiscUtils.ShowNotification(Info(Type.RecruitsDied, null, null), Color.white, AUSAssets.JackalRoleCard.LoadAsset());
            MiscUtils.AddFakeChat(Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Neutral, $"{AUSColors.GradientColorText("404040", "b8b8b8", "Jackal")} Info"), Info(Type.RecruitsDied, null, null));

            RecruitsPerished = true;
        }
    }

    public int Priority { get; set; } = 1;
    public void AssignTargets()
    {
        var jackals = PlayerControl.AllPlayerControls.ToArray().Where(x => x.IsRole<Jackal>() && !x.HasDied());
        foreach (var jackal in jackals)
        {
            var targets = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Mafia) || x.Is(Alignment.NeutralKilling) || x.Is(Faction.Town) || x.Is(Faction.Coven) && !x.HasDied()).ToList();
            var notTownTargets = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Town) && (x.Is(Faction.Mafia) || x.Is(Alignment.NeutralKilling) || x.Is(Faction.Coven)) && !x.HasDied()).ToList();
            var notMafiaTargets = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Mafia) && (x.Is(Alignment.NeutralKilling) || x.Is(Faction.Town) || x.Is(Faction.Coven)) && !x.HasDied()).ToList();
            var notCovenTargets = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Coven) && (x.Is(Faction.Mafia) || x.Is(Faction.Town) || x.Is(Alignment.NeutralKilling)) && !x.HasDied()).ToList();
            var notNKTargets = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Alignment.NeutralKilling) && (x.Is(Faction.Mafia) || x.Is(Faction.Town) || x.Is(Faction.Coven)) && !x.HasDied()).ToList();


            Random rndIndex = new();
            var randomTarget = targets[rndIndex.Next(0, targets.Count)];

            PlayerControl? randomTarget2 = null;
            if (randomTarget.Is(Faction.Town)) randomTarget2 = notTownTargets[rndIndex.Next(0, notTownTargets.Count)];
            if (randomTarget.Is(Faction.Mafia)) randomTarget2 = notMafiaTargets[rndIndex.Next(0, notMafiaTargets.Count)];
            if (randomTarget.Is(Faction.Coven)) randomTarget2 = notCovenTargets[rndIndex.Next(0, notCovenTargets.Count)];
            if (randomTarget.Is(Alignment.NeutralKilling)) randomTarget2 = notNKTargets[rndIndex.Next(0, notNKTargets.Count)];

            randomTarget.RpcAddModifier<JackalRecruit>(randomTarget2);
            randomTarget2.RpcAddModifier<JackalRecruit>(randomTarget);
        }
    }

    public enum Type { RecruitsDied, AnnounceRecs, JackalsRecs }
    public static string Info(Type type, PlayerControl rec1, PlayerControl rec2)
    {
        if (type == Type.AnnounceRecs) return $"You and {rec2.GetDefaultAppearance().PlayerName} are the {AUSColors.GradientColorText("404040", "b8b8b8", "Jackal")}'s Recruits. Stay alive to win!";
        if (type == Type.JackalsRecs) return $"{rec1.GetDefaultAppearance().PlayerName} and {rec2.GetDefaultAppearance().PlayerName} are your Recruits. Stay alive to win!";
        return "A Recruit has been killed.";
    }

    public bool RecruitsPerished;
}

#region Jackal_Assassinate
#endregion
public sealed class Jackal_Assassinate : AmongUsSalemRoleButton<Jackal, PlayerControl>
{
    public override string Name => "Assassinate";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Neutral;
    public override float Cooldown => OptionGroupSingleton<Jackal_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Jackal_Assassinate;

    public override void ClickHandler()
    {
        if (Target != null)
        {
            if (MiscUtils.SuccessfulVisit(Player, Target, true, true))
            {
                base.ClickHandler();
            }
        }
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        if (Player.CanKill(Target)) MiscUtils.RpcApplyDeathReason(Player, Target, DeathReasonShow.AssassinatedByAJackal);
        else MiscUtils.ShowNotification(MessageTexts.TooMuchDefense(Player, Target), Color.white);
        MiscUtils.PostSuccessfulVisit(Player, Target, true, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    public override bool CanUse()
    {
        return base.CanUse() && Role.RecruitsPerished;
    }
}

#region Jackal_Events
#endregion
public static class Jackal_Events
{
    [RegisterEvent(400)]
    public static void PlayerDeathEventHandler(PlayerDeathEvent @event)
    {
        if (!PlayerControl.LocalPlayer.IsHost())
        {
            return;
        }
        if (@event.Player == null || !@event.Player.TryGetModifier<JackalRecruit>(out var jackalRecruit)
            || jackalRecruit.OtherRecruit == null
            || jackalRecruit.OtherRecruit.HasDied())
        {
            return;
        }
        switch (@event.DeathReason)
        {
            case DeathReason.Exile:
                jackalRecruit.OtherRecruit.RpcPlayerExile();
                break;
            case DeathReason.Kill:
                jackalRecruit.OtherRecruit.RpcCustomMurder(jackalRecruit.OtherRecruit);
                break;
        }
    }
}

#region Jackal_Options
#endregion
public sealed class Jackal_Options : AbstractOptionGroup<Jackal>
{
    public override string GroupName => TouLocale.Get(TouNames.Jackal, "Jackal");

    [ModdedNumberOption("<color=#a22929>Jackal</color> <color=#4a86e8>Assassinate</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}