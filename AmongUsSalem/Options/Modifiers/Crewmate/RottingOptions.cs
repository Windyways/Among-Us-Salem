using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using ObjectWorkshop.Modifiers.Game.Crewmate;
using UnityEngine;

namespace ObjectWorkshop.Options.Modifiers.Crewmate;

public sealed class RottingOptions : AbstractOptionGroup<RottingModifier>
{
    public override string GroupName => TouLocale.Get(TouNames.Rotting, "Rotting");
    public override uint GroupPriority => 36;
    public override Color GroupColor => OWColors.Rotting;

    [ModdedNumberOption("Time Before Body Rots Away", 0f, 25f, 1f, MiraNumberSuffixes.Seconds)]
    public float RotDelay { get; set; } = 5f;
}