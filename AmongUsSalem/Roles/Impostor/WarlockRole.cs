using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using ObjectWorkshop.Roles.Crewmate;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Roles.Impostor;

public sealed class WarlockRole(IntPtr cppPtr)
    : ImpostorRole(cppPtr), IOWRole, IDoomable, ICrewVariant
{
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<VeteranRole>());
    public DoomableType DoomHintType => DoomableType.Relentless;
    public string RoleName => TouLocale.Get(TouNames.Warlock, "Warlock");
    public string RoleDescription => "Charge Up Your Kill Button To Multi Kill";
    public string RoleLongDescription => "Kill people in small bursts";
    public Color RoleColor => OWColors.Infiltrator;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = false,
        IntroSound = TouAudio.WarlockIntroSound,
        Icon = TouRoleIcons.Warlock
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IOWRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is an Impostor Killing role that can charge up attacks to wipe out the crew quickly."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Kill",
            "Replaces your regular kill button with three stages: On Cooldown, Uncharged, and Charged. " +
            "You cannot kill while on cooldown but can while it is charging up, however it will reset your charge. " +
            "When it is charged, you can kill in a small burst to kill multiple players in a short time.",
            TouAssets.KillSprite)
    ];

    public string revealText => "";

}