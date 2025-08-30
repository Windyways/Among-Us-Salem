using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Roles;
using ObjectWorkshop.Options.Roles.Impostor;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Roles.Impostor;

public sealed class GrenadierRole(IntPtr cppPtr) : ImpostorRole(cppPtr), IOWRole, IDoomable
{
    public string revealText => "";
    public DoomableType DoomHintType => DoomableType.Protective;
    public string RoleName => TouLocale.Get(TouNames.Grenadier, "Grenadier");
    public string RoleDescription => "Hinder The Crewmates' Vision";
    public string RoleLongDescription => "Blind the crewmates to get sneaky kills";
    public Color RoleColor => OWColors.Infiltrator;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TouRoleIcons.Grenadier,
        CanUseVent = OptionGroupSingleton<GrenadierOptions>.Instance.CanVent
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IOWRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is an Impostor Concealing role that can throw down a grenade that will blind all other players"
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Flash",
            "Throw down a grenade flashing all players in it's radius.",
            TouImpAssets.FlashSprite)
    ];
}