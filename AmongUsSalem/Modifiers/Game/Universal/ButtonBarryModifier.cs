using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Utilities.Assets;
using ObjectWorkshop.Buttons.Modifiers;
using ObjectWorkshop.Options.Modifiers;
using ObjectWorkshop.Options.Modifiers.Universal;
using ObjectWorkshop.Options.Roles.Crewmate;
using ObjectWorkshop.Options.Roles.Neutral;
using ObjectWorkshop.Roles.Crewmate;
using ObjectWorkshop.Roles.Neutral;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Modifiers.Game.Universal;

public sealed class ButtonBarryModifier : UniversalGameModifier
{
    public override string ModifierName => TouLocale.Get(TouNames.ButtonBarry, "Button Barry");
    public override LoadableAsset<Sprite>? ModifierIcon => TouModifierIcons.ButtonBarry;
    public override Color FreeplayFileColor => new Color32(180, 180, 180, 255);

    public int Priority { get; set; } = 5;
    public override ModifierFaction FactionType => ModifierFaction.UniversalUtility;

    public string GetAdvancedDescription()
    {
        return "You can button from anywhere on the map."
               + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Button",
            $"You can trigger an emergency meeting from across the map, which you may do {OptionGroupSingleton<ButtonBarryOptions>.Instance.MaxNumButtons} time(s) per game.",
            TouAssets.BarryButtonSprite)
    ];

    public override string GetDescription()
    {
        return "You can call a meeting\n from anywhere on the map.";
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<UniversalModifierOptions>.Instance.ButtonBarryAmount != 0 ? 1 : 0;
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<UniversalModifierOptions>.Instance.ButtonBarryChance;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        if (role is SwapperRole && !OptionGroupSingleton<SwapperOptions>.Instance.CanButton)
        {
            return false;
        }

        if (role is JesterRole && !OptionGroupSingleton<JesterOptions>.Instance.CanButton)
        {
            return false;
        }

        if (role is ExecutionerRole && !OptionGroupSingleton<ExecutionerOptions>.Instance.CanButton)
        {
            return false;
        }

        return base.IsModifierValidOn(role);
    }

    public static void OnRoundStart()
    {
        CustomButtonSingleton<BarryButton>.Instance.Usable = true;
    }
}