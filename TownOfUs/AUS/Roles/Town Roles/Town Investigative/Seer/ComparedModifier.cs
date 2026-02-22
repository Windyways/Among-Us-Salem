namespace AmongUsSalem.Modifiers;

public sealed class ComparedModifier(PlayerControl c, PlayerControl ct) : BaseModifier
{
    public override string ModifierName => "Compared";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
    public PlayerControl comparedTo => ct;
}