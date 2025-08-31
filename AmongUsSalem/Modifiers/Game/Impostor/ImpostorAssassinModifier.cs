using MiraAPI.GameOptions;
using AmongUsSalem.Options;
using AmongUsSalem.Roles;
using AmongUsSalem.Roles.Impostor;
using UnityEngine;

namespace AmongUsSalem.Modifiers.Game.Impostor;

public sealed class ImpostorAssassinModifier : AssassinModifier
{
    public override string ModifierName => TouLocale.Get(TouNames.Assassin, "Assassin");
    public override Color FreeplayFileColor => new Color32(255, 25, 25, 255);

    public override int GetAmountPerGame()
    {
        return 2;
    }

    public override int GetAssignmentChance()
    {
        return 100;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return role is IAUSRole { RoleName: "Assassin" };
    }
}