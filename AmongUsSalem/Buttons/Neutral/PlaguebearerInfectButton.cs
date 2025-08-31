using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using AmongUsSalem.Modifiers.Neutral;
using AmongUsSalem.Options.Roles.Neutral;
using AmongUsSalem.Roles.Neutral;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Buttons.Neutral;

public sealed class PlaguebearerInfectButton : AmongUsSalemRoleButton<PlaguebearerRole, PlayerControl>
{
    public override string Name => "Infect";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => AUSColors.Plaguebearer;
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
            Logger<AUSPlugin>.Error("Plaguebearer Infect: Target is null");
            return;
        }

        PlaguebearerRole.RpcCheckInfected(PlayerControl.LocalPlayer, Target);
    }
}