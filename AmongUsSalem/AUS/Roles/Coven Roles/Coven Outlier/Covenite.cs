using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement.Roles;

#region Covenite
#endregion
public sealed class Covenite(IntPtr cppPtr)
    : CovenRole(cppPtr), IWikiDiscoverable, ICustomAURole, ICovenRole
{
    public string RoleName { get; set; } = "Covenite";
    public string revealText => "is devoted to the Necronomicon.";
    public string RoleDescription => "Kill via Necronomicon.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.Coven;
    public Color RoleColor { get; set; } = AUSColors.Coven;
    public Alignment Alignment => Alignment.CovenOutlier;
    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public NecronomiconPriority NecronomiconPriority => NecronomiconPriority.Covenite;
    public bool Necronomicon { get; set; }

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.CoveniteRoleCard,
        CanUseSabotage = OptionGroupSingleton<CovenOptions>.Instance.CanSabotage,
        CanUseVent = OptionGroupSingleton<CovenOptions>.Instance.CanVent,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ICustomAURole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#B545FF>Covenite</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#B545FF>Coven</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#B545FF>Coven</color> <color=#1e45d4>Outlier</color>" +
            "\n<color=#fdbc00>Goal:</color> Kill all who would oppose the Coven." +
            $"\n\nAttributes:" +
            "\nWith the Necronomicon, you may choose to Basic attack someone." +
            MiscUtils.AppendOptionsText(GetType());
    }

    public bool WinConditionMet()
    {
        var aliveCoven = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.Is(Faction.Coven));
        if (aliveCoven == 0) return false;
        
        var result = MiscUtils.GetAlivePlayersToEnd().Count <= aliveCoven && MiscUtils.KillersAliveCount() == aliveCoven;
        return result || AmongUsSalem.Patches.LogicGameFlowPatches.EndGameEarlyCheck(this);
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet();
    }
}

#region Covenite_Attack
#endregion
public sealed class Covenite_Attack : AmongUsSalemRoleButton<Covenite, PlayerControl>
{
    public override string Name => "Attack";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Coven;
    public override float Cooldown => OptionGroupSingleton<CovenOptions>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.NecronomiconButton;

    public override void ClickHandler()
    {
        if (Target != null && Timer <= 0)
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

        if (Player.CanKill(Target)) MiscUtils.RpcApplyDeathReason(Player, Target, DeathReasonShow.KilledByTheCoven);
        else
        {
            MiscUtils.ShowNotification(MessageTexts.TooMuchDefense(Player, Target), Color.white);
            MiscUtils.AddFakeChat(Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Neutral, "General Info"), MessageTexts.TooMuchDefense(Player, Target));
        }
        MiscUtils.PostSuccessfulVisit(Player, Target, true, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, predicate: x =>
            (x.Data.Role is ICustomAURole customRole && customRole.Faction != Role.Faction));
    }

    public override bool CanUse()
    {
        return base.CanUse() && Player.Data.Role is ICovenRole coven && coven.Necronomicon;
    }
}