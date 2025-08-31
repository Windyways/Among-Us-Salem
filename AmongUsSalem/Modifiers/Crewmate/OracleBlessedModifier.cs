using MiraAPI.Events;
using MiraAPI.Modifiers;
using AmongUsSalem.Events.TouEvents;

namespace AmongUsSalem.Modifiers.Crewmate;

public sealed class OracleBlessedModifier(PlayerControl oracle) : BaseModifier
{
    public override string ModifierName => "Blessed";
    public override bool HideOnUi => true;
    public PlayerControl Oracle { get; } = oracle;

    public bool SavedFromExile { get; set; }

    public override void OnActivate()
    {
        var touAbilityEvent = new TouAbilityEvent(AbilityType.OracleBless, Oracle, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
}