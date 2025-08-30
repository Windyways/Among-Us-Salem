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

public sealed class MorphlingMorphButton : ObjectWorkshopRoleButton<MorphlingRole>, IAftermathableButton
{
    public override string Name => "Morph";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Infiltrator;
    public override float Cooldown => OptionGroupSingleton<MorphlingOptions>.Instance.MorphlingCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<MorphlingOptions>.Instance.MorphlingDuration;
    public override int MaxUses => (int)OptionGroupSingleton<MorphlingOptions>.Instance.MaxMorphs;
    public override LoadableAsset<Sprite> Sprite => TouImpAssets.MorphSprite;

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

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && Role is not { Sampled: null };
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
            PlayerControl.LocalPlayer.RpcAddModifier<MorphlingMorphModifier>(Role.Sampled!);
            OverrideName("Unmorph");
            UsesLeft--;
            if (MaxUses != 0)
            {
                Button?.SetUsesRemaining(UsesLeft);
            }
        }
        else
        {
            PlayerControl.LocalPlayer.RpcRemoveModifier<MorphlingMorphModifier>();
            OverrideName("Morph");
        }
    }

    public override void OnEffectEnd()
    {
        base.OnEffectEnd();

        PlayerControl.LocalPlayer.RpcRemoveModifier<MorphlingMorphModifier>();
        OverrideName("Morph");
    }
}