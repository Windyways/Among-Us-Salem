using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using ObjectWorkshop.Modifiers.Neutral;
using ObjectWorkshop.Options.Roles.Neutral;
using ObjectWorkshop.Roles.Neutral;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Neutral;

public sealed class PlaguebearerInfectButton : ObjectWorkshopRoleButton<PlaguebearerRole, PlayerControl>
{
    public override string Name => "Infect";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Plaguebearer;
    public override float Cooldown => OptionGroupSingleton<PlaguebearerOptions>.Instance.InfectCooldown;
    public override LoadableAsset<Sprite> Sprite => TouNeutAssets.InfectSprite;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: plr => !plr.HasModifier<PlaguebearerInfectedModifier>());
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<ObjectWorkshopPlugin>.Error("Plaguebearer Infect: Target is null");
            return;
        }

        PlaguebearerRole.RpcCheckInfected(PlayerControl.LocalPlayer, Target);
    }
}