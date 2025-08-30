using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Networking;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using ObjectWorkshop.Buttons;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Roles.Crewmate;

#region Sheriff
#endregion
public sealed class Sheriff(IntPtr cppPtr)
    : CrewmateRole(cppPtr), IOWRole, IContinueGame
{
    public string revealText => "";
    public bool continueGame => true;

    public override bool IsAffectedByComms => false;

    public string RoleName => TouLocale.Get(TouNames.Sheriff, "Sheriff");
    public string RoleDescription => "Attack a player, suicide if you shoot a Crewmate";
    public string RoleLongDescription => "Attack a player to kill them, but if they're a Crewmate, you'll suicide.";
    public Color RoleColor => OWColors.Crewmate;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        //Icon = TouRoleIcons.PlaceholderRC,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IOWRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Crewmate Killing role who can Attack a player to kill them, although would suicide instead if they target a Crewmate member."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Attack",
            "You can Attack a player during the round. You will kill your target. If your target is a Crewmate member, you will suicide.",
            TouCrewAssets.Placeholder)
    ];
}

#region Sheriff_Attack
#endregion
public sealed class Sheriff_Attack : ObjectWorkshopRoleButton<Sheriff, PlayerControl>
{
    public override string Name => "Attack";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Crewmate;
    public override float Cooldown => OptionGroupSingleton<Sheriff_Options>.Instance.Cooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.Placeholder;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Reactor.Utilities.Logger<ObjectWorkshopPlugin>.Error("Sheriff Ability: Target is null");
            return;
        }

        var player = PlayerControl.LocalPlayer;
        var sheriff = player.GetRole<Sheriff>();
        if (sheriff == null)
            return;

        player.RpcCustomMurder(Target);
    }
}

#region Sheriff_Options
#endregion
public sealed class Sheriff_Options : AbstractOptionGroup<Sheriff>
{
    public override string GroupName => TouLocale.Get(TouNames.Sheriff, "Sheriff");

    [ModdedNumberOption("<color=#b3ffff>Sheriff</color> <color=#4a86e8>Attack</color> Cooldown", 0, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25;
}