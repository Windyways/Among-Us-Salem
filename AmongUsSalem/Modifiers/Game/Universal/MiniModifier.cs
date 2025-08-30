using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using ObjectWorkshop.Options.Modifiers;
using ObjectWorkshop.Options.Modifiers.Universal;
using ObjectWorkshop.Utilities.Appearances;
using UnityEngine;

namespace ObjectWorkshop.Modifiers.Game.Universal;

public sealed class MiniModifier : UniversalGameModifier, IVisualAppearance
{
    public override string ModifierName => TouLocale.Get(TouNames.Mini, "Mini");
    public override LoadableAsset<Sprite>? ModifierIcon => TouModifierIcons.Mini;

    public override ModifierFaction FactionType => ModifierFaction.UniversalVisibility;
    public override Color FreeplayFileColor => new Color32(180, 180, 180, 255);

    public VisualAppearance GetVisualAppearance()
    {
        var appearance = Player.GetDefaultAppearance();
        appearance.Speed = OptionGroupSingleton<MiniOptions>.Instance.MiniSpeed;
        appearance.Size = new Vector3(0.49f, 0.49f, 1f);
        return appearance;
    }

    public string GetAdvancedDescription()
    {
        return
            $"You are smaller than regular players, and you also move {Math.Round(OptionGroupSingleton<MiniOptions>.Instance.MiniSpeed, 2)}x faster than regular players.";
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override string GetDescription()
    {
        return
            $"You are smaller than the average player, moving {Math.Round(OptionGroupSingleton<MiniOptions>.Instance.MiniSpeed, 2)}x faster.";
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<UniversalModifierOptions>.Instance.MiniChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<UniversalModifierOptions>.Instance.MiniAmount;
    }

    public override void OnActivate()
    {
        Player.RawSetAppearance(this);
    }

    public override void OnDeactivate()
    {
        Player?.ResetAppearance(fullReset: true);
    }
}