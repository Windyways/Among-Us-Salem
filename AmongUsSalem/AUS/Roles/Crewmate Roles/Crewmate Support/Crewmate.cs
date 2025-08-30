using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using Color = UnityEngine.Color;

namespace ObjectWorkshop.Roles.Crewmate;

#region Crewmate
#endregion
public sealed class Crewmate(IntPtr cppPtr) 
    : CrewmateRole(cppPtr), IOWRole, IWikiDiscoverable
{
    public string RoleName => TouLocale.Get(TouNames.Crewmate, "Crewmate");
    public string revealText => "is a member of The Skeld.";
    public string RoleDescription => "Find and vote out the Infiltrators.";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => OWColors.Crewmate;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateSupport;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = OWAssets.Crewmate,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IOWRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Crewmate Utility role thats goal is to vote out all the Infiltrators." +
            MiscUtils.AppendOptionsText(GetType());
    }
}