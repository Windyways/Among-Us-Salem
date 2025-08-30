using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;

namespace ObjectWorkshop.Options;

public sealed class GeneralOptions : AbstractOptionGroup
{
    public override string GroupName => "General";
    public override uint GroupPriority => 1;

    [ModdedToggleOption("Show Faction Modifier On Role Reveal")]
    public bool TeamModifierReveal { get; set; } = true;

    [ModdedToggleOption("Impostors Don't Know Each Other")]
    public bool FFAImpostorMode { get; set; } = false;

    public ModdedToggleOption ImpsKnowRoles { get; set; } = new("Impostors Know Each Other's Roles", true)
    {
        Visible = () => !OptionGroupSingleton<GeneralOptions>.Instance.FFAImpostorMode
    };

    public ModdedToggleOption ImpostorChat { get; set; } = new("Impostors Get A Private Meeting Chat", true)
    {
        Visible = () => !OptionGroupSingleton<GeneralOptions>.Instance.FFAImpostorMode
    };

    [ModdedToggleOption("The Dead Know Everything")]
    public bool TheDeadKnow { get; set; } = true;

    [ModdedNumberOption("Game Start Cooldowns", 5f, 30f, 2.5f, MiraNumberSuffixes.Seconds, "0.#")]
    public float GameStartCd { get; set; } = 10f;

    [ModdedToggleOption("Parallel Medbay Scans")]
    public bool ParallelMedbay { get; set; } = true;

    [ModdedEnumOption("Disable Meeting Skip Button", typeof(SkipState))]
    public SkipState SkipButtonDisable { get; set; } = SkipState.No;

    [ModdedToggleOption("First Death Shield Next Game")]
    public bool FirstDeathShield { get; set; } = true;
}

public enum ModReveal
{
    Alliance,
    Universal,
    Neither
}

public enum SkipState
{
    No,
    Emergency,
    Always
}