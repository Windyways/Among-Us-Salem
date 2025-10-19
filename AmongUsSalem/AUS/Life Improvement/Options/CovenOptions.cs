using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;

namespace AmongUsSalem.LifeImprovement;

public sealed class CovenOptions : AbstractOptionGroup
{
    public override string GroupName => "Coven Options";
    public override uint GroupPriority => 1;

    [ModdedToggleOption("Enable Necro Passing")]
    public bool EnableNecroPassing { get; set; } = true;


    [ModdedNumberOption("Necronomicon Attack Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("Coven Vision", 0.25f, 5f, 0.25f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float Vision { get; set; } = 1.5f;

    [ModdedToggleOption("Coven Can Vent")]
    public bool CanVent { get; set; } = true;

    [ModdedToggleOption("Coven Can Sabotage")]
    public bool CanSabotage { get; set; } = true;
}