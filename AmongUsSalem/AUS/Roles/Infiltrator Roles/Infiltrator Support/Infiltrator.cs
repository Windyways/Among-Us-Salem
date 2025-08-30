using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.LifeImprovement.Roles;

#region Infiltrator
#endregion
public sealed class Infiltrator(IntPtr cppPtr)
    : ImpostorRole(cppPtr), IOWRole, IWikiDiscoverable
{
    public bool attractsMetal => true;
    public string RoleName => TouLocale.Get(TouNames.Infiltrator, "Infiltrator");
    public string revealText => "is a space invader.";
    public string RoleDescription => "Kill everyone and get majority.";
    public string RoleLongDescription => "Kill everyone to win with your Infiltrator partners!";
    public Color RoleColor => OWColors.Infiltrator;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.InfiltratorSupport;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = OWAssets.Infiltrator,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IOWRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Infiltrator Support role that can kill other players."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Attack",
            "You can Attack a player during the round. You will kill your target.",
            OWAssets.KillSprite)
    ];
}