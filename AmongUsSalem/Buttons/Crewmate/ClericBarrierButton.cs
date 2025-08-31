using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using AmongUsSalem.Modifiers.Crewmate;
using AmongUsSalem.Options.Roles.Crewmate;
using AmongUsSalem.Roles.Crewmate;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Buttons.Crewmate;

public sealed class ClericBarrierButton : AmongUsSalemRoleButton<ClericRole, PlayerControl>
{
    public override string Name => "Barrier";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => AUSColors.Cleric;
    public override float Cooldown => OptionGroupSingleton<ClericOptions>.Instance.BarrierCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<ClericOptions>.Instance.BarrierDuration;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.BarrierSprite;

    public override bool IsTargetValid(PlayerControl? target)
    {
        return base.IsTargetValid(target) && !target!.HasModifier<ClericBarrierModifier>();
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<AUSPlugin>.Error($"{Name}: Target is null");
            return;
        }

        Target?.RpcAddModifier<ClericBarrierModifier>(PlayerControl.LocalPlayer);

        CustomButtonSingleton<ClericCleanseButton>.Instance.ResetCooldownAndOrEffect();
    }
}