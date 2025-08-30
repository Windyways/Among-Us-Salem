using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Roles;
using ObjectWorkshop.Options.Roles.Impostor;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Roles.Impostor;

public sealed class SwooperRole(IntPtr cppPtr) : ImpostorRole(cppPtr), IOWRole, IDoomable
{
    public string revealText => "";
    public DoomableType DoomHintType => DoomableType.Hunter;
    public string RoleName => TouLocale.Get(TouNames.Swooper, "Swooper");
    public string RoleDescription => "Turn Invisible Temporarily";
    public string RoleLongDescription => "Turn invisible and sneakily kill";
    public Color RoleColor => OWColors.Infiltrator;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<SwooperOptions>.Instance.CanVent,
        Icon = TouRoleIcons.Swooper,
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Phantom)
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IOWRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return $"The {RoleName} is an Impostor Concealing role that can temporarily turn invisible."
               + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Swoop",
            "Turn invisible to all players except Impostors.",
            TouImpAssets.SwoopSprite),
        new("Unswoop",
            "Cancel your swoop early, or let it finish fully to make yourself visible once again.",
            TouImpAssets.UnswoopSprite)
    ];
}