using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using ObjectWorkshop.Modifiers;
using ObjectWorkshop.Modifiers.Neutral;
using ObjectWorkshop.Options.Roles.Crewmate;
using ObjectWorkshop.Roles.Crewmate;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Crewmate;

public sealed class EngineerVentButton : ObjectWorkshopRoleButton<EngineerTouRole, Vent>
{
    private static readonly ContactFilter2D Filter = Helpers.CreateFilter(Constants.Usables);
    public override string Name => "Vent";
    public override string Keybind => Keybinds.VentAction;
    public override Color TextOutlineColor => OWColors.Engineer;

    public override float Cooldown =>
        OptionGroupSingleton<EngineerOptions>.Instance.VentCooldown + 0.001f + MapCooldown;

    public override float EffectDuration => OptionGroupSingleton<EngineerOptions>.Instance.VentDuration;
    public override int MaxUses => (int)OptionGroupSingleton<EngineerOptions>.Instance.MaxVents;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.EngiVentSprite;
    public override bool ShouldPauseInVent => false;
    public int ExtraUses { get; set; }

    public override Vent? GetTarget()
    {
        var vent = PlayerControl.LocalPlayer.GetNearestObjectOfType<Vent>(Distance / 4, Filter);
        if (vent == null)
        {
            vent = PlayerControl.LocalPlayer.GetNearestObjectOfType<Vent>(Distance / 3, Filter);
        }

        if (vent == null)
        {
            vent = PlayerControl.LocalPlayer.GetNearestObjectOfType<Vent>(Distance / 2, Filter);
        }

        if (vent == null)
        {
            vent = PlayerControl.LocalPlayer.GetNearestObjectOfType<Vent>(Distance, Filter);
        }

        if (vent != null && PlayerControl.LocalPlayer.CanUseVent(vent))
        {
            return vent;
        }

        return null;
    }

    public override bool CanUse()
    {
        var newTarget = GetTarget();
        if (newTarget != Target)
        {
            Target?.SetOutline(false, false);
        }

        Target = IsTargetValid(newTarget) ? newTarget : null;
        SetOutline(true);

        if (PlayerControl.LocalPlayer.HasModifier<GlitchHackedModifier>() || PlayerControl.LocalPlayer.GetModifiers<DisabledModifier>().Any(x => !x.CanUseAbilities))
        {
            return false;
        }

        return ((Timer <= 0 && !PlayerControl.LocalPlayer.inVent && Target != null) || PlayerControl.LocalPlayer.inVent) && (MaxUses == 0 || UsesLeft > 0);
    }

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
            // Logger<ObjectWorkshopPlugin>.Error($"Effect is No Longer Active");
            // Logger<ObjectWorkshopPlugin>.Error($"Cooldown is active");
        }
        else if (HasEffect)
        {
            EffectActive = true;
            Timer = EffectDuration;
            // Logger<ObjectWorkshopPlugin>.Error($"Effect is Now Active");
        }
        else
        {
            Timer = !PlayerControl.LocalPlayer.inVent ? 0.001f : Cooldown;
            // Logger<ObjectWorkshopPlugin>.Error($"Cooldown is active");
        }
    }

    protected override void OnClick()
    {
        if (!PlayerControl.LocalPlayer.inVent)
        {
            // Logger<ObjectWorkshopPlugin>.Error($"Entering Vent");
            if (Target != null)
            {
                PlayerControl.LocalPlayer.MyPhysics.RpcEnterVent(Target.Id);
                Target.SetButtons(true);
            }
            // else Logger<ObjectWorkshopPlugin>.Error($"Vent is null...");
        }
        else if (Timer != 0)
        {
            // Logger<ObjectWorkshopPlugin>.Error($"Leaving Vent");
            OnEffectEnd();
            if (!HasEffect)
            {
                EffectActive = false;
                Timer = Cooldown;
            }
        }
    }

    public override void OnEffectEnd()
    {
        if (PlayerControl.LocalPlayer.inVent)
        {
            // Logger<ObjectWorkshopPlugin>.Error($"Left Vent");
            Vent.currentVent.SetButtons(false);
            PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(Vent.currentVent.Id);
            UsesLeft--;
            if (MaxUses != 0)
            {
                Button?.SetUsesRemaining(UsesLeft);
            }
        }
    }
}