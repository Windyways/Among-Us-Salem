using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement.Roles;

#region Covenite
#endregion
public sealed class Covenite(IntPtr cppPtr)
    : NeutralRole(cppPtr), IWikiDiscoverable, IAUSRole, ICovenRole
{
    public string RoleName { get; set; } = TouLocale.Get(TouNames.Covenite, "Covenite");
    public string revealText => "placeholder.";
    public string RoleDescription => "Placeholder.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction RoleFaction { get; set; } = Faction.Coven;
    public Color RoleColor { get; set; } = AUSColors.Coven;
    public Alignment Alignment => Alignment.CovenOutlier;
    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;
    
    public DeathReasonShow deathReasonShow { get; set; } = DeathReasonShow.Alive;

    public NecronomiconPriority NecronomiconPriority => NecronomiconPriority.Covenite;
    public bool Necronomicon { get; set; }

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.CoveniteRoleCard,
        CanUseSabotage = true,
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
        
        var result = Helpers.GetAlivePlayers().Count <= aliveCoven && MiscUtils.KillersAliveCount() == aliveCoven;
        return result;
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
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Coven;
    public override float Cooldown => OptionGroupSingleton<Covenite_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.NecronomiconButton;

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

        if (Player.CanKill(Target)) MiscUtils.RpcApplyDeathReason(Player, Target, DeathReasonShow.KilledByTheCoven);
        else MiscUtils.ShowNotification(MessageTexts.TooMuchDefense(Player, Target), Color.white);
        MiscUtils.PostSuccessfulVisit(Player, Target, true, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        if (target == null) return base.IsTargetValid(target);
        return base.IsTargetValid(target) &&
            !(target.Data.Role is IAUSRole ausrole && ausrole.RoleFaction == Role.RoleFaction);
    }

    public override bool CanUse()
    {
        return base.CanUse() && Player.Data.Role is ICovenRole coven && coven.Necronomicon;
    }
}

#region Covenite_Options
#endregion
public sealed class Covenite_Options : AbstractOptionGroup<Covenite>
{
    public override string GroupName => TouLocale.Get(TouNames.Covenite, "Covenite");

    [ModdedNumberOption("<color=#B545FF>Covenite</color> <color=#4a86e8>Attack</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}