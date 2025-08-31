using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using AmongUsSalem.Roles.Crewmate;

namespace AmongUsSalem.Options.Roles.Crewmate;

public sealed class SwapperOptions : AbstractOptionGroup<SwapperRole>
{
    public override string GroupName => TouLocale.Get(TouNames.Swapper, "Swapper");

    [ModdedToggleOption("Can Call Button")]
    public bool CanButton { get; set; } = true;
}