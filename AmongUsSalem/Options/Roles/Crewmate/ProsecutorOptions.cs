using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using AmongUsSalem.Roles.Crewmate;

namespace AmongUsSalem.Options.Roles.Crewmate;

public sealed class ProsecutorOptions : AbstractOptionGroup<ProsecutorRole>
{
    public override string GroupName => TouLocale.Get(TouNames.Prosecutor, "Prosecutor");

    [ModdedToggleOption("Prosecutor Dies When They Exile A Crewmate")]
    public bool ExileOnCrewmate { get; set; } = true;

    [ModdedNumberOption("Max Prosecutions", 1, 5)]
    public float MaxProsecutions { get; set; } = 2f;
}