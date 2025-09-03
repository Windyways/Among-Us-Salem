using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;

namespace AmongUsSalem.LifeImprovement;

public sealed class AUSOptions : AbstractOptionGroup
{
    public override string GroupName => "Game Options";
    public override uint GroupPriority => 0;

    [ModdedToggleOption("Enable Necro Passing")]
    public bool EnableNecroPassing { get; set; } = true;
}