using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Hud;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using AmongUsSalem.Buttons.Impostor;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Roles.Impostor;

public sealed class VenererRole(IntPtr cppPtr) : ImpostorRole(cppPtr), ITOURole, IDoomable
{
    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;
    public DoomableType DoomHintType => DoomableType.Trickster;
    public string RoleName => TouLocale.Get(TouNames.Venerer, "Venerer");
    public string RoleDescription => "With Each Kill Your Ability Becomes Stronger";
    public string RoleLongDescription => "Kill players to unlock ability perks";
    public Color RoleColor => AUSColors.Mafia;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public Alignment Alignment => Alignment.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TouRoleIcons.Venerer
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IAUSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is an Impostor Concealing role that can kill players to gain new abilities, preventing others from catching them! However, the ability will be used immediately as they receive it, which will stack up."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Camouflage",
            "Stage 1 of the abilities.\n" +
            "You will appear as a gray bean for all players, allowing you to sneak away from kills.",
            TouImpAssets.CamouflageSprite),
        new("Sprint",
            "Stage 2 of the abilities.\n" +
            "You will gain the speed of the Flash while hidden from camo.",
            TouImpAssets.SprintSprite),
        new("Freeze",
            "The Final Stage of the abilities.\n" +
            "You will slow down players around you in a radius, as well as being fast and hidden from camo.",
            TouImpAssets.FreezeSprite)
    ];

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        CustomButtonSingleton<VenererAbilityButton>.Instance.UpdateAbility(VenererAbility.None);
    }
}

public enum VenererAbility
{
    None,
    Camouflage,
    Sprint,
    Freeze
}