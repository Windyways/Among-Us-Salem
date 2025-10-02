using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Roles;
using Reactor.Utilities;
using AmongUsSalem.Modules;
using UnityEngine;

namespace AmongUsSalem.Roles.Neutral;

public class NeutralGhostRole(IntPtr cppPtr) : RoleBehaviour(cppPtr), IAUSRole
{
    private Minigame _hauntMenu = null!;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;
    public override bool IsDead => true;
    public override bool IsAffectedByComms => false;

    public void Awake()
    {
        var crewGhost = RoleManager.Instance.GetRole(RoleTypes.CrewmateGhost).Cast<CrewmateGhostRole>();
        _hauntMenu = crewGhost.HauntMenu;
        Ability = crewGhost.Ability;
    }

    public virtual string RoleName => Player != null ? Player.GetRoleWhenAlive().NiceName : "Neutral Ghost";
    public virtual string RoleDescription => Player != null ? Player.GetRoleWhenAlive().Blurb : string.Empty;
    public virtual string RoleLongDescription => Player != null ? Player.GetRoleWhenAlive().BlurbLong : string.Empty;
    public virtual Color RoleColor => Player != null ? Player.GetRoleWhenAlive().TeamColor : AUSColors.Neutral;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public virtual Alignment Alignment => Alignment.None;

    public virtual CustomRoleConfiguration Configuration => new(this)
    {
        TasksCountForProgress = false,
        HideSettings = true,
        RoleHintType = Player != null && Player.GetRoleWhenAlive() is ICustomRole custom
            ? custom.Configuration.RoleHintType
            : RoleHintType.None
    };

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;
    Color IAUSRole.RoleColor { get; set; } = AUSColors.Town;
    public Faction Faction { get; set; } = Faction.Neutral;
    string IAUSRole.RoleName { get; set; } = "Neutral Ghost";

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = new StringBuilder();
        if (Player.GetRoleWhenAlive() is IAUSRole touRole)
        {
            stringB = IAUSRole.SetDeadTabText(touRole);
            if (touRole.MetWinCon)
            {
                stringB.Append("<b>You have already won.</b>");
            }
            else
            {
                stringB.Append("<b>You are dead.</b>");
            }
        }
        else
        {
            stringB.Append("<b>You are dead.</b>");
        }

        return stringB;
    }

    public virtual bool WinConditionMet()
    {
        var role = Player.GetRoleWhenAlive();

        return role is IAUSRole tRole && tRole.WinConditionMet();
    }

    public override void AppendTaskHint(Il2CppSystem.Text.StringBuilder taskStringBuilder)
    {
        // remove default task hint
    }

    public override bool CanUse(IUsable console)
    {
        if (!GameManager.Instance.LogicUsables.CanUse(console, Player))
        {
            return false;
        }

        var console2 = console.TryCast<Console>()!;
        return console2 == null || console2.AllowImpostor;
    }

    // reimplement haunt minigame
    public override void UseAbility()
    {
        if (HudManager.Instance.Chat.IsOpenOrOpening)
        {
            return;
        }

        if (Minigame.Instance)
        {
            if (Minigame.Instance.TryCast<HauntMenuMinigame>())
            {
                Minigame.Instance.Close();
            }

            return;
        }

        var minigame = Instantiate(_hauntMenu, HudManager.Instance.AbilityButton.transform, false);
        minigame.transform.SetLocalZ(-5f);
        minigame.Begin(null);
        HudManager.Instance.AbilityButton.SetDisabled();
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        var role = Player.GetRoleWhenAlive();

        var win = role.DidWin(gameOverReason);

        Logger<AUSPlugin>.Message($"NeutralGhostRole.DidWin - role: {role.NiceName} DidWin: {win}");

        return win;
    }
}