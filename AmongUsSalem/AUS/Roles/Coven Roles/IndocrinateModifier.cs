using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AmongUsSalem.LifeImprovement.Roles;

#region IndocrinateModifier
#endregion
public sealed class IndocrinateModifier : BaseModifier, ICovenRole
{
    public override string ModifierName => "Indocrinated";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public NecronomiconPriority NecronomiconPriority => NecronomiconPriority.Indocrinated;
    public bool Necronomicon { get; set; }

    public override void OnActivate()
    {
        if (Player.Data.Role is IAUSRole ausRole)
        {
            ausRole.RoleColor = AUSColors.Coven;
            ausRole.RoleFaction = Faction.Coven;
        }
    }

    // To Do: figure out how to make Modifiers target players.
}