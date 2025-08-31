using MiraAPI.Modifiers;

namespace AmongUsSalem.Modifiers.Crewmate;

public sealed class SeerEvilRevealModifier : BaseModifier
{
    public override string ModifierName => "SeerEvilReveal";
    public override bool HideOnUi => true;

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
}