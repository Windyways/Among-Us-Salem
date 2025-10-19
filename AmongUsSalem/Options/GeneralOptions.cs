using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;

namespace AmongUsSalem.Options;

public sealed class GeneralOptions : AbstractOptionGroup
{
    public override string GroupName => "General";
    public override uint GroupPriority => 10;

    [ModdedToggleOption("Show Faction Modifier On Role Reveal")]
    public bool TeamModifierReveal { get; set; } = true;

    public ModdedToggleOption ImpostorChat { get; set; } = new("Impostors Get A Private Meeting Chat", false)
    {
        Visible = () => false
    };

    [ModdedToggleOption("The Dead Know Everything")]
    public bool TheDeadKnow { get; set; } = true;

    [ModdedNumberOption("Game Start Cooldowns", 5f, 30f, 2.5f, MiraNumberSuffixes.Seconds, "0.#")]
    public float GameStartCd { get; set; } = 10f;

    [ModdedToggleOption("Parallel Medbay Scans")]
    public bool ParallelMedbay { get; set; } = true;
}