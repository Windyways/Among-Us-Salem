using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using ObjectWorkshop.Options.Modifiers;
using ObjectWorkshop.Roles.Crewmate;
using UnityEngine;

namespace ObjectWorkshop.Modifiers.Game.Universal;

public sealed class SixthSenseModifier : UniversalGameModifier
{
    public override string ModifierName => TouLocale.Get(TouNames.SixthSense, "Sixth Sense");
    public override LoadableAsset<Sprite>? ModifierIcon => TouModifierIcons.SixthSense;

    public override ModifierFaction FactionType => ModifierFaction.UniversalPassive;
    public override Color FreeplayFileColor => new Color32(180, 180, 180, 255);

    public string GetAdvancedDescription()
    {
        return
            "You will know when someone uses their ability on you.";
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override string GetDescription()
    {
        return "Know when someone interacts with you.";
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<UniversalModifierOptions>.Instance.SixthSenseChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<UniversalModifierOptions>.Instance.SixthSenseAmount;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role is not AurialRole;
    }
}