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
    public string RoleName => TouLocale.Get(TouNames.Covenite, "Covenite");
    public string revealText => "placeholder.";
    public string RoleDescription => "Placeholder.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction RoleFaction => Faction.Coven;
    public Color RoleColor => AUSColors.Coven;
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
        UseVanillaKillButton = false,
        Icon = AUSAssets.CoveniteRoleCard,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IAUSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#ab42ef>Covenite</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color> <color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#ab42ef>Coven</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#ab42ef>Coven</color> <color=#1e45d4>Outlier</color>" +
            "\n<color=#fdbc00>Goal:</color> Kill all who would oppose the Coven." +
            $"\n\nAttributes:" +
            "\nTBD" +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Attack",
            "You can Attack a player during the round. You will kill your target.",
            AUSAssets.Mafioso_Attack)
    ];

    public bool WinConditionMet()
    {
        var alivePlayers = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied());
        var aliveCoven = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.Is(Faction.Coven));
        return alivePlayers == aliveCoven;
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
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Necronomicon;

    public override void ClickHandler()
    {
        if (Target != null)
        {
            if (MiscUtils.SuccessfulVisit(Role.Player, Target, true, true))
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

        if (Role.Player.CanKill(Target)) MiscUtils.RpcApplyDeathReason(Role.Player, Target, DeathReasonShow.KilledByTheCoven);
        else MiscUtils.ShowNotification(MessageTexts.TooMuchDefense(Role.Player, Target), Color.white);
        MiscUtils.PostSuccessfulVisit(Role.Player, Target, true, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(false, Distance);
    }

    public override bool CanUse()
    {
        return base.CanUse() && Role.Player.Data.Role is ICovenRole coven && coven.Necronomicon;
    }
}

#region Covenite_Options
#endregion
public sealed class Covenite_Options : AbstractOptionGroup<Covenite>
{
    public override string GroupName => TouLocale.Get(TouNames.Covenite, "Covenite");

    [ModdedNumberOption("<color=#ab42ef>Covenite</color> <color=#ab42ef>Attack</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}