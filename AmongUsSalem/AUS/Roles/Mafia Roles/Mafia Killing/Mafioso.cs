using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement.Roles;

#region Mafioso
#endregion
public sealed class Mafioso(IntPtr cppPtr)
    : ImpostorRole(cppPtr), IWikiDiscoverable, IAUSRole
{
    public string RoleName { get; set; } = TouLocale.Get(TouNames.Mafioso, "Mafioso");
    public string revealText => "does the Godfather's dirty work.";
    public string RoleDescription => "Placeholder.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;

    public Faction RoleFaction { get; set; } = Faction.Mafia;
    public Color RoleColor { get; set; } = AUSColors.Mafia;
    public Alignment Alignment => Alignment.MafiaKilling;

    public Attack Attack { get; set; } = Attack.Basic;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.Basic;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

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
            "<color=#DD0000>Mafioso</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#DD0000>Mafia</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#DD0000>Mafia</color> <color=#1e45d4>Killing</color>" +
            "\n<color=#fdbc00>Goal:</color> Kill anyone that will not submit to the Mafia." +
            $"\n\nAttributes:" +
            "\nTBD." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Attack",
            "You can Attack a player during the round. You will kill your target.",
            AUSAssets.Mafioso_Attack)
    ];

    public static string Info()
    {
        return "You were promoted to a <b><color=#DD0000>Mafioso</color></b>!";
    }
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

        if (Player.CanKill(Target)) MiscUtils.RpcApplyDeathReason(Player, Target, DeathReasonShow.KilledByAMemberOfTheMafia);
        else MiscUtils.ShowNotification(MessageTexts.TooMuchDefense(Player, Target), Color.white);
        MiscUtils.PostSuccessfulVisit(Player, Target, true, true);
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

    [ModdedNumberOption("<color=#DD0000>Mafioso</color> <color=#4a86e8>Attack</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}