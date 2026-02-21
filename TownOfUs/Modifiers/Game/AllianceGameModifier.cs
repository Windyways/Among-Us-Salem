using MiraAPI.Modifiers.Types;
using MiraAPI.PluginLoading;

namespace TownOfUs.Modifiers.Game;

[MiraIgnore]
public abstract class AllianceGameModifier : GameModifier
{
    public virtual string IntroInfo => $"Alliance: {ModifierName}";
    public virtual string Symbol => "?";
    public virtual float IntroSize => 4f;
    public virtual int CustomAmount => GetAmountPerGame();
    public virtual int CustomChance => GetAssignmentChance();
    public virtual bool DoesTasks => true;
    public virtual bool GetsPunished => true;
    public virtual bool CrewContinuesGame => true;
    public virtual ModifierFaction FactionType => ModifierFaction.Alliance;

    public override bool HideOnUi => false;

    public override int GetAmountPerGame()
    {
        return 1;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return !role.Player.GetModifierComponent().HasModifier<AllianceGameModifier>(true);
    }
}