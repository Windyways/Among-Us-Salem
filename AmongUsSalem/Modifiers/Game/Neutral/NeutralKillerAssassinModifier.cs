using MiraAPI.GameOptions;
using AmongUsSalem.Options;
using AmongUsSalem.Roles;

namespace AmongUsSalem.Modifiers.Game.Neutral;

public sealed class NeutralKillerAssassinModifier : AssassinModifier
{
    public override string ModifierName => TouLocale.Get(TouNames.Assassin, "Assassin");

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<AssassinOptions>.Instance.NumberOfNeutralAssassins;
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<AssassinOptions>.Instance.NeutAssassinChance;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return role is IAUSRole { Alignment: Alignment.NeutralKilling };
    }
}