using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using ObjectWorkshop.Modifiers.Neutral;
using ObjectWorkshop.Options.Modifiers.Alliance;
using ObjectWorkshop.Options.Roles.Neutral;
using ObjectWorkshop.Roles.Neutral;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Neutral;

public sealed class ArsonistDouseButton : ObjectWorkshopRoleButton<ArsonistRole, PlayerControl>
{
    public override string Name => "Douse";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Arsonist;
    public override float Cooldown => OptionGroupSingleton<ArsonistOptions>.Instance.DouseCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TouNeutAssets.DouseButtonSprite;

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<ObjectWorkshopPlugin>.Error("Arsonist Attack: Target is null");
            return;
        }

        Target.RpcAddModifier<ArsonistDousedModifier>(PlayerControl.LocalPlayer.PlayerId);

        CustomButtonSingleton<ArsonistIgniteButton>.Instance.SetTimer(CustomButtonSingleton<ArsonistIgniteButton>
            .Instance.Cooldown);
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        return base.IsTargetValid(target) && !target!.HasModifier<ArsonistDousedModifier>();
    }

    public override PlayerControl? GetTarget()
    {
        if (!OptionGroupSingleton<LoversOptions>.Instance.LoversKillEachOther && PlayerControl.LocalPlayer.IsLover())
        {
            return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, false, x => !x.IsLover() && !x.HasModifier<ArsonistDousedModifier>());
        }
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: x => !x.HasModifier<ArsonistDousedModifier>());
    }
}