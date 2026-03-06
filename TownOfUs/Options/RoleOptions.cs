using MiraAPI.GameOptions.OptionTypes;

namespace TownOfUs.Options;

public sealed class RoleOptions : AbstractOptionGroup
{
    public static readonly string[] OptionStrings =
    [
        "<color=#06E00C>Town</color> <color=#4a86e8>Investigative</color>",
        "<color=#06E00C>Town</color> <color=#4a86e8>Executive</color>",
        "<color=#06E00C>Town</color> <color=#4a86e8>Government</color>",
        "<color=#06E00C>Town</color> <color=#4a86e8>Killing</color>",
        "<color=#06E00C>Town</color> <color=#4a86e8>Outlier</color>",
        "<color=#06E00C>Town</color> <color=#4a86e8>Protective</color>",
        "<color=#06E00C>Town</color> <color=#4a86e8>Support</color>",
        "<color=#4a86e8>Random</color> <color=#06E00C>Town</color>",
        "<color=#4a86e8>Common</color> <color=#06E00C>Town</color>",

        "<color=#4a86e8>Random</color> <color=#ff004e>Apocalypse</color>",
        "<color=#a9a9a9>Neutral</color> <color=#4a86e8>Benign</color>",
        "<color=#a9a9a9>Neutral</color> <color=#4a86e8>Chaos</color>",
        "<color=#a9a9a9>Neutral</color> <color=#4a86e8>Evil</color>",
        "<color=#a9a9a9>Neutral</color> <color=#4a86e8>Killing</color>",
        "<color=#a9a9a9>Neutral</color> <color=#4a86e8>Outlier</color>",
        "<color=#a9a9a9>Neutral</color> <color=#4a86e8>Pariah</color>",
        "<color=#4a86e8>Random</color> <color=#a9a9a9>Neutral</color>",

        "<color=#DD0000>Mafia</color> <color=#4a86e8>Deception</color>",
        "<color=#DD0000>Mafia</color> <color=#4a86e8>Killing</color>",
        "<color=#DD0000>Mafia</color> <color=#4a86e8>Support</color>",
        "<color=#4a86e8>Random</color> <color=#DD0000>Mafia</color>",
        "<color=#4a86e8>Common</color> <color=#DD0000>Mafia</color>",

        "<color=#B545FF>Coven</color> <color=#4a86e8>Deception</color>",
        "<color=#B545FF>Coven</color> <color=#4a86e8>Killing</color>",
        "<color=#B545FF>Coven</color> <color=#4a86e8>Outlier</color>",
        "<color=#B545FF>Coven</color> <color=#4a86e8>Power</color>",
        "<color=#B545FF>Coven</color> <color=#4a86e8>Utility</color>",
        "<color=#4a86e8>Random</color> <color=#B545FF>Coven</color>",
        "<color=#4a86e8>Common</color> <color=#ab42ef>Coven</color>",

        "Any",
        "<color=#06E00C>T</color><color=#4a86e8>E</color>-<color=#06E00C>T</color><color=#4a86e8>G</color>",
        "<color=#06E00C>T</color><color=#4a86e8>E</color>-<color=#06E00C>T</color><color=#4a86e8>G</color>-<color=#06E00C>T</color><color=#4a86e8>O</color>",
        "<color=#4a86e8>R</color><color=#ff004e>A</color>-<color=#4a86e8>R</color><color=#a9a9a9>N</color>",
        "Not <color=#DD0000>Mafia</color>"
    ];

    public override string GroupName => "Role";
    public override uint GroupPriority => 2;

    public ModdedEnumOption Slot1 { get; } =
        new("Slot 1", (int)RoleListOption.RandomTown, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => true
        };

    public ModdedEnumOption Slot2 { get; } =
        new("Slot 2", (int)RoleListOption.RandomTown, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => true
        };

    public ModdedEnumOption Slot3 { get; } =
        new("Slot 3", (int)RoleListOption.RandomTown, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => true
        };

    public ModdedEnumOption Slot4 { get; } =
        new("Slot 4", (int)RoleListOption.RandomMafia, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => true
        };

    public ModdedEnumOption Slot5 { get; } =
        new("Slot 5", (int)RoleListOption.RandomTown, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => true
        };

    public ModdedEnumOption Slot6 { get; } =
        new("Slot 6", (int)RoleListOption.RandomTown, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => true
        };

    public ModdedEnumOption Slot7 { get; } =
        new("Slot 7", (int)RoleListOption.RandomTown, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => true
        };

    public ModdedEnumOption Slot8 { get; } =
        new("Slot 8", (int)RoleListOption.RandomTown, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => true
        };

    public ModdedEnumOption Slot9 { get; } =
        new("Slot 9", (int)RoleListOption.RandomCoven, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => true
        };

    public ModdedEnumOption Slot10 { get; } =
        new("Slot 10", (int)RoleListOption.RandomMafia, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => true
        };

    public ModdedEnumOption Slot11 { get; } =
        new("Slot 11", (int)RoleListOption.RandomTown, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => true
        };

    public ModdedEnumOption Slot12 { get; } =
        new("Slot 12", (int)RoleListOption.RandomCoven, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => true
        };

    public ModdedEnumOption Slot13 { get; } =
        new("Slot 13", (int)RoleListOption.RandomTown, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => true
        };

    public ModdedEnumOption Slot14 { get; } =
        new("Slot 14", (int)RoleListOption.RandomTown, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => true
        };

    public ModdedEnumOption Slot15 { get; } =
        new("Slot 15", (int)RoleListOption.RandomTown, typeof(RoleListOption), OptionStrings)
        {
            Visible = () => true
        };
}

public enum RoleListOption
{
    TownInvestigative,
    TownExecutive,
    TownGovernment,
    TownKilling,
    TownOutlier,
    TownProtective,
    TownSupport,
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

    Any,
    TE_TG,
    TE_TG_TO,
    NA_RN,


    NotMafia,


    None
}