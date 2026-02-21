using MiraAPI.Modifiers;
using MiraAPI.PluginLoading;

namespace TownOfUs.Modifiers;

[MiraIgnore]
public abstract class PlayerTargetModifier(byte ownerId) : BaseModifier
{
    public override string ModifierName => "Target";
    public override bool HideOnUi => true;

    public byte OwnerId { get; set; } = ownerId;
}