using MiraAPI.GameOptions.OptionTypes;

namespace AmongUsSalem.Options;

public sealed class AUSOptions : AbstractOptionGroup
{
    public override string GroupName => "Game Options";
    public override uint GroupPriority => 0;
    
    [ModdedToggleOption("Enable Night Timer")]
    public bool NightTimer { get; set; } = true;

    [ModdedNumberOption("Night Duration", 10f, 995f, 5f, MiraNumberSuffixes.Seconds)]
    public float NightDuration { get; set; } = 40f;

    [ModdedNumberOption("Indirect Invisibility Duration", 0f, 60f, 0.5f, MiraNumberSuffixes.Seconds)]
    public float InvisDuration { get; set; } = 5f;

    [ModdedNumberOption("Rampage Radius", 0.25f, 15f, 0.25f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float RampageRadius { get; set; } = 1f;
}