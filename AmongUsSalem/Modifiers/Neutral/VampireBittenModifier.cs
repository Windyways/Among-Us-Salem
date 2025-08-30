using MiraAPI.Modifiers;

namespace ObjectWorkshop.Modifiers.Neutral;

public sealed class VampireBittenModifier : BaseModifier
{
    public override string ModifierName => "Bitten";
    public override bool HideOnUi => true;
}