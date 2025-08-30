using MiraAPI.Modifiers;

namespace ObjectWorkshop.Modifiers.Crewmate;

public sealed class JailSparedModifier(byte jailorId) : BaseModifier
{
    public override string ModifierName => "Jail Immune";
    public override bool HideOnUi => true;
    public byte JailorId { get; } = jailorId;

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
}