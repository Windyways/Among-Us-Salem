using UnityEngine;

namespace ObjectWorkshop.Modifiers.Crewmate;

public sealed class PlumberVenterModifier(PlayerControl owner, Color color) : ArrowTargetModifier(owner, color, 0)
{
    public override string ModifierName => "Plumber Venter Arrow";
}