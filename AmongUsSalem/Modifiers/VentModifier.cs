using MiraAPI.Modifiers;

namespace ObjectWorkshop.Modifiers;

public sealed class VentModifier : BaseModifier
{
    public override string ModifierName => "Vent";
    public override bool HideOnUi => true;

    public override bool? CanVent()
    {
        return true;
    }
}