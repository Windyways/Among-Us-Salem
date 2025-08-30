using UnityEngine;

namespace ObjectWorkshop.Modifiers.Game.Universal;

public sealed class SatelliteArrowModifier(DeadBody deadBody, Color color) : ArrowDeadBodyModifier(deadBody, color, 0)
{
    public override string ModifierName => "Satellite Arrow";
}