using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Roles.Impostor;

public sealed class EclipsalRole(IntPtr cppPtr) : ImpostorRole(cppPtr), IAUSRole, IDoomable
{
    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;
    public DoomableType DoomHintType => DoomableType.Perception;
    public string RoleName => TouLocale.Get(TouNames.Eclipsal, "Eclipsal");
    public string RoleDescription => "Block Out The Light";
    public string RoleLongDescription => "Make crewmates unable to see, slowly returning their vision to normal.";
    public Color RoleColor => AUSColors.Mafia;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public Alignment Alignment => Alignment.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TouRoleIcons.Eclipsal
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IAUSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is an Impostor Concealing role that can hinder the vision of all crewmates and neutrals alike, given that they are near the Eclipsal."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Blind",
            "Blinding players causes their fog of war to overtake their screen, only letting them see the map and prevents reporting. After a while, they will regain their vision and have vision like normal.",
            TouImpAssets.BlindSprite)
    ];
}