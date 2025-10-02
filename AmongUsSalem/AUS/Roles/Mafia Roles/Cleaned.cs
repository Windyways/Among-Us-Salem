using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AmongUsSalem.LifeImprovement.Roles;

#region Cleaned
#endregion
public sealed class Cleaned : BaseModifier
{
    public override string ModifierName => "Cleaned";
    public override bool Unique => false;
    public override bool HideOnUi => true;
}