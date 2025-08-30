using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using ObjectWorkshop.Roles.Crewmate;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Crewmate;

public sealed class WardenFortifyButton : ObjectWorkshopRoleButton<WardenRole, PlayerControl>
{
    public override string Name => "Fortify";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Warden;
    public override float Cooldown => 0.001f + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.FortifySprite;

    public override bool CanUse()
    {
        return base.CanUse() && Role is { Fortified: null };
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<ObjectWorkshopPlugin>.Error("Warden Fortify: Target is null");
            return;
        }

        WardenRole.RpcWardenFortify(PlayerControl.LocalPlayer, Target);
    }
}