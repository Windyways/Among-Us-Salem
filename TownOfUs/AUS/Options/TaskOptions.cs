namespace AmongUsSalem.Options;

public sealed class TaskOptions : AbstractOptionGroup
{
    public override string GroupName => "Task Options";
    public override uint GroupPriority => 2;

    [ModdedToggleOption("Disable Download/Upload Task")]
    public bool DisableDownloadUpload { get; set; } = false;
    
    [ModdedToggleOption("Disable Divert Power Task")]
    public bool DisableDivertPower { get; set; } = false;
}