using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AmongUsSalem.LifeImprovement.Roles;

#region DeepfakeRole
#endregion
// This is used to make players see roles of their target.
// Example: Consigliere revealing a player, Coroner finding killer, etc.
public sealed class RoleLearn(PlayerControl visitor) : BaseModifier
{
    public override string ModifierName => "Role Learn";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Visitor = visitor;
}