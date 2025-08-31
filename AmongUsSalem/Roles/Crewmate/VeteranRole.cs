using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using AmongUsSalem.Options.Roles.Crewmate;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Roles.Crewmate;

public sealed class VeteranRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITouCrewRole, IDoomable
{
    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;
    public override bool IsAffectedByComms => false;

    public string revealText => "";
    public int Alerts { get; set; }
    public DoomableType DoomHintType => DoomableType.Trickster;
    public string RoleName => TouLocale.Get(TouNames.Veteran, "Veteran");
    public string RoleDescription => "Alert To Kill Anyone Who Interacts With You";
    public string RoleLongDescription => "Alert to kill whoever who interacts with you.";
    public Color RoleColor => AUSColors.Veteran;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public Alignment Alignment => Alignment.None;
    public bool IsPowerCrew => Alerts > 0; // Stop end game checks if the veteran can still alert

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TouRoleIcons.Veteran,
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Impostor)
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IAUSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Crewmate Killing role that can go on alert and kill anyone who interacts with them."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Alert",
            $"When the {TouLocale.Get(TouNames.Veteran, "Veteran")} is on alert, any player who interacts with them will be instantly killed, with the exception of Pestilence and shielded players, who will ignore the attack.",
            TouCrewAssets.AlertSprite)
    ];

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        Alerts = (int)OptionGroupSingleton<VeteranOptions>.Instance.MaxNumAlerts;
    }
}