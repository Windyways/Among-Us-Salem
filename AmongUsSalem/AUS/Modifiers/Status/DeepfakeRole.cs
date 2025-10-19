using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AmongUsSalem.LifeImprovement.Roles;

#region DeepfakeRole
#endregion
// This is used to make a player appear as a role they're not.
// For example, Vampire-Rec Mayor being promoted to Vampire should still show them as Mayor to everyone.
public sealed class DeepfakeRole(string name, Color col) : BaseModifier
{
    public override string ModifierName => "DeepfakeRole";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public string roleName = name;
    public Color roleColor = col;
}