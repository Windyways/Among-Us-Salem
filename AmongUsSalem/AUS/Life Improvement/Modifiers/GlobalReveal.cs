using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AmongUsSalem.LifeImprovement.Roles;

#region GlobalReveal
#endregion
public sealed class GlobalReveal : BaseModifier
{
    public override string ModifierName => "Global Reveal";
    public override bool Unique => false;
    public override bool HideOnUi => true;
}