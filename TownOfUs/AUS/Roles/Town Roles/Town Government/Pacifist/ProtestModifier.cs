namespace AmongUsSalem.Modifiers;

public sealed class ProtestModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Protest";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
}