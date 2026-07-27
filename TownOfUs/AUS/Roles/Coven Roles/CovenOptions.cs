namespace AmongUsSalem.CovenRoles;

public sealed class CovenOptions : AbstractOptionGroup
{
    public override string GroupName => "Coven Settings";
    public override uint GroupPriority => 1;

    [ModdedNumberOption("Max Coven Roles Per Game", 1f, 4f, 1f, MiraNumberSuffixes.None)]
    public float MaxCoven { get; set; } = 4;

    [ModdedNumberOption("Coven Attack Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedToggleOption("Coven Roles Can Vent")]
    public bool CanVent { get; set; } = true;

    /*[ModdedToggleOption("Coven Roles Can Sabotage")]
    public bool CanSabotage { get; set; } = true;*/

    [ModdedToggleOption("Enable Necro Passing")]
    public bool EnableNecroPassing { get; set; } = true;
}