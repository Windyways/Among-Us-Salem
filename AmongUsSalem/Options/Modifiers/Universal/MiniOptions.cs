using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using ObjectWorkshop.Modifiers.Game.Universal;
using UnityEngine;

namespace ObjectWorkshop.Options.Modifiers.Universal;

public sealed class MiniOptions : AbstractOptionGroup<MiniModifier>
{
    public override string GroupName => TouLocale.Get(TouNames.Mini, "Mini");
    public override uint GroupPriority => 26;
    public override Color GroupColor => OWColors.Mini;

    [ModdedNumberOption("Mini Speed", 1.05f, 2.5f, 0.05f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float MiniSpeed { get; set; } = 1.35f;
}