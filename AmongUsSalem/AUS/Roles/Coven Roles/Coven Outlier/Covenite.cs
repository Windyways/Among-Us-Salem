using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement.Roles;

#region Covenite
#endregion
public sealed class Covenite(IntPtr cppPtr)
    : ImpostorRole(cppPtr), IAUSRole, IWikiDiscoverable
{
    public string RoleName => TouLocale.Get(TouNames.Covenite, "Covenite");
    public string revealText => "placeholder.";
    public string RoleDescription => "Placeholder.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;

    public Faction RoleFaction => Faction.Coven;
    public Color RoleColor => AUSColors.Coven;
    public Alignment Alignment => Alignment.CovenOutlier;
    public Attack Attack => Attack.Basic;
    public Defense Defense => Defense.None;

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
            $"The {RoleName} is a {Alignment} role that can kill other players."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Attack",
            "You can Attack a player during the round. You will kill your target.",
            AUSAssets.Mafioso_Attack)
    ];
}

#region Covenite_Attack
#endregion
public sealed class Covenite_Attack : AmongUsSalemRoleButton<Covenite, PlayerControl>
{
    public override string Name => "Attack";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Coven;
    public override float Cooldown => OptionGroupSingleton<Covenite_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Mafioso_Attack;

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

        if (Role.Player.CanKill(Target)) Role.Player.RpcCustomMurder(Target);
        else MiscUtils.ShowNotification(MessageTexts.TooMuchDefense(Role.Player, Target), Color.white);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(false, Distance);
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