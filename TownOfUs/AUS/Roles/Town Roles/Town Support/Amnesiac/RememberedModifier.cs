namespace AmongUsSalem.Modifiers;

public sealed class RememberedModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Remembered";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
}