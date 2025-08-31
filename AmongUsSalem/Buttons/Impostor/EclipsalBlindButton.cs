using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using AmongUsSalem.Modifiers.Impostor;
using AmongUsSalem.Options.Roles.Impostor;
using AmongUsSalem.Roles.Impostor;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Buttons.Impostor;

public sealed class EclipsalBlindButton : AmongUsSalemRoleButton<EclipsalRole>, IAftermathableButton
{
    public override string Name => "Blind";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => AUSColors.Mafia;
    public override float Cooldown => OptionGroupSingleton<EclipsalOptions>.Instance.BlindCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<EclipsalOptions>.Instance.BlindDuration;
    public override LoadableAsset<Sprite> Sprite => TouImpAssets.BlindSprite;

    protected override void OnClick()
    {
        OverrideName("Unblinding");
        var blindRadius = OptionGroupSingleton<EclipsalOptions>.Instance.BlindRadius;
        var blindedPlayers =
            Helpers.GetClosestPlayers(PlayerControl.LocalPlayer, blindRadius * ShipStatus.Instance.MaxLightRadius);

        foreach (var player in blindedPlayers.Where(x => !x.HasDied() && !x.IsImpostor()))
        {
            player.RpcAddModifier<EclipsalBlindModifier>(PlayerControl.LocalPlayer);
        }
        // PlayerControl.LocalPlayer.RpcAddModifier<EclipsalBlindModifier>(PlayerControl.LocalPlayer);
    }

    public override void OnEffectEnd()
    {
        OverrideName("Blind");
    }
}