using MiraAPI.GameOptions;
using AmongUsSalem.Options;
using AmongUsSalem.Roles;

namespace AmongUsSalem.Modifiers.Game.Neutral;

public sealed class NeutralKillerAssassinModifier : AssassinModifier
{
    public override string ModifierName => TouLocale.Get(TouNames.Assassin, "Assassin");

    public override int GetAmountPerGame()
    {
        return 0;
    }

    public override int GetAssignmentChance()
    {
        return 0;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return role is IAUSRole { Alignment: Alignment.NeutralKilling };
    }
}