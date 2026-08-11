using MiraAPI.Modifiers.Types;
using MiraAPI.PluginLoading;

namespace TownOfUs.Modifiers;

[MiraIgnore]
public abstract class ExcludedGameModifier : GameModifier
{
    public override string ModifierName => "Excluded From Haunt Menu";
    public override bool HideOnUi => true;

    public override int GetAmountPerGame()
    {
        return 0;
    }

    public override int GetAssignmentChance()
    {
        return 0;
    }
}