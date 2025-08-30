using MiraAPI.Modifiers;

namespace ObjectWorkshop.Modifiers.Crewmate;

public sealed class SeerGoodRevealModifier : BaseModifier
{
    public override string ModifierName => "SeerGoodReveal";
    public override bool HideOnUi => true;

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
}