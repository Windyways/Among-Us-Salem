namespace AmongUsSalem.Options;

public sealed class RoleGenOptions : AbstractOptionGroup
{
    public override string GroupName => "Role Generation Options";
    public override uint GroupPriority => 1;

    [ModdedToggleOption("Enable All Outliers")]
    public bool EnableAllOutliers { get; set; } = false;

    [ModdedToggleOption("Legacy Role Generation")]
    public bool LegacyRoleGen { get; set; } = false;
}