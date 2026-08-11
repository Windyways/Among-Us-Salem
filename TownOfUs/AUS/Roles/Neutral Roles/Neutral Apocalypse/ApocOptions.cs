namespace AmongUsSalem.CovenRoles;

public sealed class ApocOptions : AbstractOptionGroup
{
    public override string GroupName => "Apocalypse Settings";
    public override uint GroupPriority => 2;

    /*[ModdedToggleOption("Apocalypse Roles Can Sabotage")]
    public bool CanSabotage { get; set; } = false;*/

    [ModdedToggleOption("Enable Four Horsemen")]
    public bool EnableFourHorsemen { get; set; } = true;
}