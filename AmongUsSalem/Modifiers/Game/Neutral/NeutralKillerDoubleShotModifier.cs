using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using AmongUsSalem.Options.Modifiers;
using AmongUsSalem.Roles;

namespace AmongUsSalem.Modifiers.Game.Neutral;

public sealed class NeutralKillerDoubleShotModifier : DoubleShotModifier
{
    public override string ModifierName => TouLocale.Get(TouNames.DoubleShot, "Double Shot");
    public override bool ShowInFreeplay => true;
    public bool IsHiddenFromList => true;
    // YES this is scuffed, a better solution will be used at a later time
    public uint FakeTypeId => ModifierManager.GetModifierTypeId(ModifierManager.Modifiers.FirstOrDefault(x => x.ModifierName == "Double Shot")!.GetType()) ?? throw new InvalidOperationException("Modifier is not registered.");

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<NeutralModifierOptions>.Instance.DoubleShotChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<NeutralModifierOptions>.Instance.DoubleShotAmount;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        if (
            role is IAUSRole { Alignment: Alignment.NeutralKilling }
            && role.Player.GetModifierComponent().HasModifier<NeutralKillerAssassinModifier>(true)
            && !role.Player.GetModifierComponent().HasModifier<TouGameModifier>(true)
        )
        {
            return true;
        }

        return false;
    }
}