namespace AmongUsSalem.Modifiers.Neutral;

public sealed class GuardianAngelTargetModifier(byte gaId) : PlayerTargetModifier(gaId)
{
    public override string ModifierName => "Guardian Angel Target";
}