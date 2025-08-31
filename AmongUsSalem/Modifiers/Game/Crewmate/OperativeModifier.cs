using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using AmongUsSalem.Buttons.Modifiers;
using AmongUsSalem.Modifiers.Game.Universal;
using AmongUsSalem.Options.Modifiers;
using AmongUsSalem.Options.Modifiers.Crewmate;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Modifiers.Game.Crewmate;

public sealed class OperativeModifier : TouGameModifier
{
    public override string ModifierName => TouLocale.Get(TouNames.Operative, "Operative");
    public override string IntroInfo => "You can also use security systems on-the-go.";
    public override LoadableAsset<Sprite>? ModifierIcon => TouModifierIcons.Operative;
    public override Color FreeplayFileColor => new Color32(140, 255, 255, 255);

    public override ModifierFaction FactionType => ModifierFaction.CrewmateUtility;

    public string GetAdvancedDescription()
    {
        return
            "Use cameras at anytime with a limited battery charge."
            + MiscUtils.AppendOptionsText(GetType());
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override string GetDescription()
    {
        return "Utilize the Cameras from anywhere";
    }

    public override void OnActivate()
    {
        base.OnActivate();

        if (!Player.AmOwner)
        {
            return;
        }

        CustomButtonSingleton<SecurityButton>.Instance.AvailableCharge =
            OptionGroupSingleton<OperativeOptions>.Instance.StartingCharge;
    }

    public static void OnRoundStart()
    {
        CustomButtonSingleton<SecurityButton>.Instance.AvailableCharge +=
            OptionGroupSingleton<OperativeOptions>.Instance.RoundCharge;
    }

    public static void OnTaskComplete()
    {
        CustomButtonSingleton<SecurityButton>.Instance.AvailableCharge +=
            OptionGroupSingleton<OperativeOptions>.Instance.TaskCharge;
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<CrewmateModifierOptions>.Instance.OperativeChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<CrewmateModifierOptions>.Instance.OperativeAmount;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsCrewmate() &&
               !role.Player.GetModifierComponent().HasModifier<SatelliteModifier>(true) &&
               !role.Player.GetModifierComponent().HasModifier<ButtonBarryModifier>(true);
    }
}