using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;

namespace AmongUsSalem.Options;

public sealed class RoleOptions : AbstractOptionGroup
{
    public static readonly string[] OptionStrings =
    [
        "<color=#06e00c>Town</color> <color=#4a86e8>Investigative</color>",
        "<color=#06e00c>Town</color> <color=#4a86e8>Killing</color>",
        "<color=#06e00c>Town</color> <color=#4a86e8>Protective</color>",
        "<color=#06e00c>Town</color> <color=#4a86e8>Outlier</color>",
        "<color=#06e00c>Town</color> <color=#4a86e8>Power</color>",
        "<color=#06e00c>Town</color> <color=#4a86e8>Support</color>",
        "<color=#06e00c>Town</color> <color=#4a86e8>Utility</color>",
        "<color=#4a86e8>Random</color> <color=#06e00c>Town</color>",
        "<color=#4a86e8>Common</color> <color=#06e00c>Town</color>",

        "<color=#a9a9a9>Neutral</color> <color=#ff004e>Apocalypse</color>",
        "<color=#a9a9a9>Neutral</color> <color=#4a86e8>Benign</color>",
        "<color=#a9a9a9>Neutral</color> <color=#4a86e8>Chaos</color>",
        "<color=#a9a9a9>Neutral</color> <color=#4a86e8>Evil</color>",
        "<color=#a9a9a9>Neutral</color> <color=#4a86e8>Killing</color>",
        "<color=#a9a9a9>Neutral</color> <color=#4a86e8>Outlier</color>",
        "<color=#a9a9a9>Neutral</color> <color=#4a86e8>Pariah</color>",
        "<color=#4a86e8>Random</color> <color=#a9a9a9>Neutral</color>",
        
        "<color=#dd0000>Mafia</color> <color=#4a86e8>Deception</color>",
        "<color=#dd0000>Mafia</color> <color=#4a86e8>Killing</color>",
        "<color=#dd0000>Mafia</color> <color=#4a86e8>Support</color>",
        "<color=#4a86e8>Random</color> <color=#dd0000>Mafia</color>",
        "<color=#4a86e8>Common</color> <color=#dd0000>Mafia</color>",
        
        "<color=#ab42ef>Coven</color> <color=#4a86e8>Deception</color>",
        "<color=#ab42ef>Coven</color> <color=#4a86e8>Killing</color>",
        "<color=#ab42ef>Coven</color> <color=#4a86e8>Outlier</color>",
        "<color=#ab42ef>Coven</color> <color=#4a86e8>Power</color>",
        "<color=#ab42ef>Coven</color> <color=#4a86e8>Utility</color>",
        "<color=#4a86e8>Random</color> <color=#ab42ef>Coven</color>",
        "<color=#4a86e8>Common</color> <color=#ab42ef>Coven</color>",
        
        "<color=#ce36fa>Traitor</color> <color=#4a86e8>Deceptive</color>",
        "<color=#ce36fa>Traitor</color> <color=#4a86e8>Power</color>",
        "<color=#ce36fa>Traitor</color> <color=#4a86e8>Utility</color>",
        "<color=#4a86e8>Random</color> <color=#ce36fa>Traitor</color>",
        "<color=#4a86e8>Common</color> <color=#ce36fa>Traitor</color>",

        "Any",
        "Not <color=#dd0000>Mafia</color>"
    ];

    public override string GroupName => "Role";
    public override uint GroupPriority => 2;

    [ModdedToggleOption("Role List Enabled")]
    public bool RoleListEnabled { get; set; } = true;

    public ModdedEnumOption Slot1 { get; } =
        new("Slot 1", (int)RoleListOption.RandomTown, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot2 { get; } =
        new("Slot 2", (int)RoleListOption.RandomTown, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot3 { get; } =
        new("Slot 3", (int)RoleListOption.RandomTown, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot4 { get; } =
        new("Slot 4", (int)RoleListOption.RandomMafia, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot5 { get; } =
        new("Slot 5", (int)RoleListOption.TownInvestigative, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot6 { get; } =
        new("Slot 6", (int)RoleListOption.TownInvestigative, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot7 { get; } =
        new("Slot 7", (int)RoleListOption.TownInvestigative, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot8 { get; } =
        new("Slot 8", (int)RoleListOption.TownInvestigative, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot9 { get; } =
        new("Slot 9", (int)RoleListOption.RandomMafia, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot10 { get; } =
        new("Slot 10", (int)RoleListOption.TownInvestigative, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot11 { get; } =
        new("Slot 11", (int)RoleListOption.TownInvestigative, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot12 { get; } =
        new("Slot 12", (int)RoleListOption.TownInvestigative, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot13 { get; } =
        new("Slot 13", (int)RoleListOption.RandomTown, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot14 { get; } =
        new("Slot 14", (int)RoleListOption.RandomMafia, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled
        };

    public ModdedEnumOption Slot15 { get; } =
        new("Slot 15", (int)RoleListOption.TownInvestigative, typeof(RoleListOption), OptionStrings)
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
    TownInvestigative,
    TownKilling,
    TownProtective,
    TownOutlier,
    TownPower,
    TownSupport,
    TownUtility,
    RandomTown,
    CommonTown,

    NeutralApocalypse,
    NeutralBenign,
    NeutralChaos,
    NeutralEvil,
    NeutralKilling,
    NeutralOutlier,
    NeutralPariah,
    RandomNeutral,

    MafiaDeception,
    MafiaKilling,
    MafiaSupport,
    RandomMafia,
    CommonMafia,

    CovenDeception,
    CovenKilling,
    CovenOutlier,
    CovenPower,
    CovenUtility,
    RandomCoven,
    CommonCoven,

    TraitorDeceptive,
    TraitorPower,
    TraitorUtility,
    RandomTraitor,
    CommonTraitor,

    Any,
    NotMafia
}