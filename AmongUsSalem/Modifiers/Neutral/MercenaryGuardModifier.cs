using MiraAPI.Events;
using MiraAPI.Modifiers;
using ObjectWorkshop.Events.TouEvents;

namespace ObjectWorkshop.Modifiers.Neutral;

public sealed class MercenaryGuardModifier(PlayerControl mercenary) : BaseModifier
{
    public override string ModifierName => "Mercenary Guard";
    public override bool HideOnUi => true;
    public PlayerControl Mercenary { get; } = mercenary;

    public override void OnActivate()
    {
        base.OnActivate();

        var touAbilityEvent = new TouAbilityEvent(AbilityType.MercenaryGuard, Mercenary, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
}