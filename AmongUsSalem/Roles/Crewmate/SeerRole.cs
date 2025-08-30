using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Roles.Crewmate;

public sealed class SeerRole(IntPtr cppPtr) : CrewmateRole(cppPtr), IOWRole, IDoomable
{
    public string revealText => "";
    public override bool IsAffectedByComms => false;
    public DoomableType DoomHintType => DoomableType.Fearmonger;
    public string RoleName => TouLocale.Get(TouNames.Seer, "Seer");
    public string RoleDescription => "Reveal The Alliance Of Other Players";
    public string RoleLongDescription => "Reveal alliances of other players to find the Impostors";
    public Color RoleColor => OWColors.Seer;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TouRoleIcons.Seer,
        IntroSound = TouAudio.QuestionSound
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IOWRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return $"The {RoleName} is a Crewmate Investigative role that can reveal the alliance of other players."
               + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Reveal",
            "Reveal the faction of a player",
            TouCrewAssets.SeerSprite)
    ];
}