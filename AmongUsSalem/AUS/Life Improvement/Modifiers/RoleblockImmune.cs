using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AmongUsSalem.LifeImprovement.Roles;

#region RBimmune
#endregion
public sealed class RBimmune() : BaseModifier
{
    public override string ModifierName => "Roleblock Immunity";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public bool permanent;

    public override void OnMeetingStart()
    {
        if (!permanent) Player.RpcRemoveModifier<RBimmune>();
    }
}