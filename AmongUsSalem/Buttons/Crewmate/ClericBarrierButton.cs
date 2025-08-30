using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using ObjectWorkshop.Modifiers.Crewmate;
using ObjectWorkshop.Options.Roles.Crewmate;
using ObjectWorkshop.Roles.Crewmate;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Crewmate;

public sealed class ClericBarrierButton : ObjectWorkshopRoleButton<ClericRole, PlayerControl>
{
    public override string Name => "Barrier";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Cleric;
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
            Logger<ObjectWorkshopPlugin>.Error($"{Name}: Target is null");
            return;
        }

        Target?.RpcAddModifier<ClericBarrierModifier>(PlayerControl.LocalPlayer);

        CustomButtonSingleton<ClericCleanseButton>.Instance.ResetCooldownAndOrEffect();
    }
}