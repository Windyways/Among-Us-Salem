using MiraAPI.Modifiers;

namespace ObjectWorkshop.Modifiers.Impostor;

public sealed class BlackmailSparedModifier(byte blackMailerId) : BaseModifier
{
    public override string ModifierName => "Blackmailed";
    public override bool HideOnUi => true;

    public byte BlackMailerId { get; } = blackMailerId;
}