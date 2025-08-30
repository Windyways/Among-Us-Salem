using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using ObjectWorkshop.Modifiers;
using ObjectWorkshop.Modifiers.Impostor;
using ObjectWorkshop.Modifiers.Neutral;
using ObjectWorkshop.Options.Roles.Impostor;
using ObjectWorkshop.Roles.Impostor;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Impostor;

public sealed class SwooperSwoopButton : ObjectWorkshopRoleButton<SwooperRole>, IAftermathableButton
{
    public override Color TextOutlineColor => OWColors.Infiltrator;
    public override string Name => "Swoop";
    public override string Keybind => Keybinds.SecondaryAction;
    public override float Cooldown => OptionGroupSingleton<SwooperOptions>.Instance.SwoopCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<SwooperOptions>.Instance.SwoopDuration;
    public override int MaxUses => (int)OptionGroupSingleton<SwooperOptions>.Instance.MaxSwoops;
    public override LoadableAsset<Sprite> Sprite => TouImpAssets.SwoopSprite;

    public override void ClickHandler()
    {
        if (!CanUse())
        {
            return;
        }

        OnClick();
        Button?.SetDisabled();
        if (EffectActive)
        {
            Timer = Cooldown;
            EffectActive = false;
        }
        else if (HasEffect)
        {
            EffectActive = true;
            Timer = EffectDuration;
        }
        else
        {
            Timer = Cooldown;
        }
    }

    public override bool CanUse()
    {
        if (PlayerControl.LocalPlayer.HasModifier<GlitchHackedModifier>() || PlayerControl.LocalPlayer.GetModifiers<DisabledModifier>().Any(x => !x.CanUseAbilities))
        {
            return false;
        }

        return ((Timer <= 0 && !EffectActive) || (EffectActive && Timer <= EffectDuration - 2f));
    }

    protected override void OnClick()
    {
        if (!EffectActive)
        {
            PlayerControl.LocalPlayer.RpcAddModifier<SwoopModifier>();
            UsesLeft--;
            if (MaxUses != 0)
            {
                Button?.SetUsesRemaining(UsesLeft);
            }
        }
        else
        {
            OnEffectEnd();
        }
    }

    public override void OnEffectEnd()
    {
        if (!PlayerControl.LocalPlayer.HasModifier<SwoopModifier>())
        {
            return;
        }

        PlayerControl.LocalPlayer.RpcRemoveModifier<SwoopModifier>();
    }
}