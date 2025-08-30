using MiraAPI.GameOptions;
using ObjectWorkshop.Options;
using ObjectWorkshop.Roles;
using ObjectWorkshop.Roles.Impostor;
using UnityEngine;

namespace ObjectWorkshop.Modifiers.Game.Impostor;

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
        return role is IOWRole { RoleName: "Assassin" };
    }
}