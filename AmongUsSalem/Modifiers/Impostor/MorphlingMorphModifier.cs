using MiraAPI.Events;
using MiraAPI.GameOptions;
using AmongUsSalem.Events.TouEvents;
using AmongUsSalem.Options.Roles.Impostor;
using AmongUsSalem.Utilities.Appearances;

namespace AmongUsSalem.Modifiers.Impostor;

public sealed class MorphlingMorphModifier(PlayerControl target) : ConcealedModifier, IVisualAppearance
{
    public override float Duration => OptionGroupSingleton<MorphlingOptions>.Instance.MorphlingDuration;
    public override string ModifierName => "Morph";
    public override bool HideOnUi => true;
    public override bool AutoStart => true;
    public override bool VisibleToOthers => true;
    public bool VisualPriority => true;

    public VisualAppearance GetVisualAppearance()
    {
        return new VisualAppearance(target.GetDefaultModifiedAppearance(), AmongUsSalemAppearances.Morph);
    }

    public override void OnActivate()
    {
        Player.RawSetAppearance(this);

        var touAbilityEvent = new TouAbilityEvent(AbilityType.MorphlingMorph, Player, target);
        MiraEventManager.InvokeEvent(touAbilityEvent);
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }

    public override void OnDeactivate()
    {
        Player.ResetAppearance();

        var touAbilityEvent = new TouAbilityEvent(AbilityType.MorphlingUnmorph, Player, target);
        MiraEventManager.InvokeEvent(touAbilityEvent);
    }
}