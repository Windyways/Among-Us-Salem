using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using AmongUsSalem.Buttons.Neutral;
using AmongUsSalem.Events.TouEvents;
using AmongUsSalem.Options.Roles.Neutral;
using AmongUsSalem.Utilities.Appearances;

namespace AmongUsSalem.Modifiers.Neutral;

public sealed class GlitchMimicModifier(PlayerControl target) : ConcealedModifier, IVisualAppearance
{
    public override float Duration => OptionGroupSingleton<GlitchOptions>.Instance.MimicDuration;
    public override string ModifierName => "Mimic";
    public override bool HideOnUi => true;
    public override bool AutoStart => true;
    public override bool VisibleToOthers => true;
    public bool VisualPriority => true;

    public VisualAppearance GetVisualAppearance()
    {
        return new VisualAppearance(target.GetDefaultModifiedAppearance(), AmongUsSalemAppearances.Mimic);
    }

    public override void OnActivate()
    {
        Player.RawSetAppearance(this);
        var touAbilityEvent = new TouAbilityEvent(AbilityType.GlitchMimic, Player, target);
        MiraEventManager.InvokeEvent(touAbilityEvent);
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }

    public override void OnDeactivate()
    {
        CustomButtonSingleton<GlitchMimicButton>.Instance.SetTimer(OptionGroupSingleton<GlitchOptions>.Instance
            .MimicCooldown);
        Player.ResetAppearance();
        var touAbilityEvent = new TouAbilityEvent(AbilityType.GlitchUnmimic, Player, target);
        MiraEventManager.InvokeEvent(touAbilityEvent);
    }
}