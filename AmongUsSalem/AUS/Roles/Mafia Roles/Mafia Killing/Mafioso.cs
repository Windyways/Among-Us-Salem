using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement.Roles;

#region Mafioso
#endregion
public sealed class Mafioso(IntPtr cppPtr)
    : ImpostorRole(cppPtr), IAUSRole, IWikiDiscoverable
{
    public string RoleName => TouLocale.Get(TouNames.Mafioso, "Mafioso");
    public string revealText => "placeholder.";
    public string RoleDescription => "Placeholder.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;

    public Faction RoleFaction => Faction.Mafia;
    public Color RoleColor => AUSColors.Mafia;
    public Alignment Alignment => Alignment.MafiaKilling;
    public Attack Attack => Attack.Basic;
    public Defense Defense => Defense.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = false,
        Icon = AUSAssets.MafiosoRoleCard,
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

#region Mafioso_Attack
#endregion
public sealed class Mafioso_Attack : AmongUsSalemRoleButton<Mafioso, PlayerControl>
{
    public override string Name => "Attack";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Mafia;
    public override float Cooldown => OptionGroupSingleton<Mafioso_Options>.Instance.Cooldown;
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

#region Mafioso_Options
#endregion
public sealed class Mafioso_Options : AbstractOptionGroup<Mafioso>
{
    public override string GroupName => TouLocale.Get(TouNames.Mafioso, "Mafioso");

    [ModdedNumberOption("<color=#dd0000>Mafioso</color> <color=#4a86e8>Attack</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}