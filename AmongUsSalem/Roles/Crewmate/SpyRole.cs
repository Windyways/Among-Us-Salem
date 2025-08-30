using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using ObjectWorkshop.Buttons.Crewmate;
using ObjectWorkshop.Options.Roles.Crewmate;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Roles.Crewmate;

public sealed class SpyRole(IntPtr cppPtr) : CrewmateRole(cppPtr), IOWRole, IDoomable
{
    public string revealText => "";
    public DoomableType DoomHintType => DoomableType.Perception;
    public string RoleName => TouLocale.Get(TouNames.Spy, "Spy");
    public string RoleDescription => "Snoop Around And Find Stuff Out";
    public string RoleLongDescription => "Gain extra information on the Admin Table";
    public Color RoleColor => OWColors.Spy;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TouRoleIcons.Spy,
        IntroSound = TouAudio.SpyIntroSound
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IOWRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Crewmate Investigative role that gains extra information on the admin table. They not only see how many people are in a room, but will also see who is in every room."
            + MiscUtils.AppendOptionsText(GetType());
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner)
        {
            CustomButtonSingleton<SpyAdminTableRoleButton>.Instance.AvailableCharge =
                OptionGroupSingleton<SpyOptions>.Instance.StartingCharge.Value;
        }
    }

    public static void OnRoundStart()
    {
        CustomButtonSingleton<SpyAdminTableRoleButton>.Instance.AvailableCharge +=
            OptionGroupSingleton<SpyOptions>.Instance.RoundCharge.Value;
    }

    public static void OnTaskComplete()
    {
        CustomButtonSingleton<SpyAdminTableRoleButton>.Instance.AvailableCharge +=
            OptionGroupSingleton<SpyOptions>.Instance.TaskCharge.Value;
    }
}