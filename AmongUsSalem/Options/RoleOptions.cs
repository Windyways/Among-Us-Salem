using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;

namespace ObjectWorkshop.Options;

public sealed class RoleOptions : AbstractOptionGroup
{
    public static readonly string[] OptionStrings =
    [
        "<color=#b3ffff>Crewmate</color> <color=#4a86e8>Investigative</color>",
        "<color=#b3ffff>Crewmate</color> <color=#4a86e8>Killing</color>",
        "<color=#b3ffff>Crewmate</color> <color=#4a86e8>Protective</color>",
        "<color=#b3ffff>Crewmate</color> <color=#4a86e8>Support</color>",
        "<color=#4a86e8>Random</color> <color=#b3ffff>Crewmate</color>",
        "<color=#4a86e8>Common</color> <color=#b3ffff>Crewmate</color>",

        "<color=#a9a9a9>Neutral</color> <color=#91bbff>Associative</color>",
        "<color=#a9a9a9>Neutral</color> <color=#d34a72>Evil</color>",
        "<color=#a9a9a9>Neutral</color> <color=#3562ac>Predator</color>",
        "<color=#4a86e8>Random</color> <color=#a9a9a9>Neutral</color>",
        "<color=#4a86e8>Common</color> <color=#a9a9a9>Neutral</color>",
        
        "<color=#ff5050>Infiltrator</color> <color=#4a86e8>Disruption</color>",
        "<color=#ff5050>Infiltrator</color> <color=#4a86e8>Evacuative</color>",
        "<color=#ff5050>Infiltrator</color> <color=#4a86e8>Gunsman</color>",
        "<color=#ff5050>Infiltrator</color> <color=#4a86e8>Support</color>",
        "<color=#4a86e8>Random</color> <color=#ff5050>Infiltrator</color>",
        "<color=#4a86e8>Common</color> <color=#ff5050>Infiltrator</color>",

        "Any",
        "Not <color=#ff5050>Infiltrator</color>"
    ];

    public override string GroupName => "Role";
    public override uint GroupPriority => 2;

    [ModdedToggleOption("Reduce Impostor Streak")]
    public bool LastImpostorBias { get; set; } = false;

    public ModdedNumberOption ImpostorBiasPercent { get; } =
        new("Reduction Chance", 15f, 0f, 100f, 5f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.LastImpostorBias
        };

    [ModdedToggleOption("Role List Enabled")]
    public bool RoleListEnabled { get; set; } = true;

    public ModdedEnumOption Slot1 { get; } =
        new("Slot 1", (int)RoleListOption.CommonCrewmate, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot2 { get; } =
        new("Slot 2", (int)RoleListOption.CommonCrewmate, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot3 { get; } =
        new("Slot 3", (int)RoleListOption.CommonCrewmate, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot4 { get; } =
        new("Slot 4", (int)RoleListOption.CommonInfiltrator, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot5 { get; } =
        new("Slot 5", (int)RoleListOption.CommonCrewmate, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot6 { get; } =
        new("Slot 6", (int)RoleListOption.CommonCrewmate, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot7 { get; } =
        new("Slot 7", (int)RoleListOption.CommonCrewmate, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot8 { get; } =
        new("Slot 8", (int)RoleListOption.CommonCrewmate, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot9 { get; } =
        new("Slot 9", (int)RoleListOption.CommonInfiltrator, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot10 { get; } =
        new("Slot 10", (int)RoleListOption.CommonCrewmate, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot11 { get; } =
        new("Slot 11", (int)RoleListOption.CommonCrewmate, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot12 { get; } =
        new("Slot 12", (int)RoleListOption.CommonCrewmate, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot13 { get; } =
        new("Slot 13", (int)RoleListOption.CommonCrewmate, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot14 { get; } =
        new("Slot 14", (int)RoleListOption.CommonInfiltrator, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot15 { get; } =
        new("Slot 15", (int)RoleListOption.CommonCrewmate, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedNumberOption MinNeutralBenign { get; } =
        new("Min Neutral Benign", 0f, 0f, 3f, 1f, MiraNumberSuffixes.None, "0")
        {
            Visible = () => !OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedNumberOption MaxNeutralBenign { get; } =
        new("Max Neutral Benign", 0f, 0f, 3f, 1f, MiraNumberSuffixes.None, "0")
        {
            Visible = () => !OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedNumberOption MinNeutralEvil { get; } =
        new("Min Neutral Evil", 0f, 0f, 3f, 1f, MiraNumberSuffixes.None, "0")
        {
            Visible = () => !OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedNumberOption MaxNeutralEvil { get; } =
        new("Max Neutral Evil", 0f, 0f, 3f, 1f, MiraNumberSuffixes.None, "0")
        {
            Visible = () => !OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedNumberOption MinNeutralKiller { get; } =
        new("Min Neutral Killer", 0f, 0f, 5f, 1f, MiraNumberSuffixes.None, "0")
        {
            Visible = () => !OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedNumberOption MaxNeutralKiller { get; } =
        new("Max Neutral Killer", 0f, 0f, 5f, 1f, MiraNumberSuffixes.None, "0")
        {
            Visible = () => !OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };
}

public enum RoleListOption
{
    CrewmateInvestigative,
    CrewmateKilling,
    CrewmateProtective,
    CrewmateSupport,
    RandomCrewmate,
    CommonCrewmate,

    NeutralAssociative,
    NeutralEvil,
    NeutralPredator,
    RandomNeutral,
    CommonNeutral,

    InfiltratorDisruption,
    InfiltratorEvacuative,
    InfiltratorGunsman,
    InfiltratorSupport,
    RandomInfiltrator,
    CommonInfiltrator,

    Any,
    NotInfiltrator
}