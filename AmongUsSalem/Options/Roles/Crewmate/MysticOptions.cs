using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using AmongUsSalem.Roles.Crewmate;

namespace AmongUsSalem.Options.Roles.Crewmate;

public sealed class MysticOptions : AbstractOptionGroup<MysticRole>
{
    public override string GroupName => TouLocale.Get(TouNames.Mystic, "Mystic");

    [ModdedNumberOption("Dead Body Arrow Duration", 0f, 1f, 0.05f, MiraNumberSuffixes.Seconds, "0.00")]
    public float MysticArrowDuration { get; set; } = 0.1f;
}