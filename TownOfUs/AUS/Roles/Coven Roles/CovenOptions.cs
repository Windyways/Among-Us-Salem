namespace AmongUsSalem.CovenRoles;

public sealed class CovenOptions : AbstractOptionGroup
{
    public override string GroupName => "Coven Settings";
    public override uint GroupPriority => 1;

    [ModdedToggleOption("Coven Roles Can Vent")]
    public bool CanVent { get; set; } = true;
}