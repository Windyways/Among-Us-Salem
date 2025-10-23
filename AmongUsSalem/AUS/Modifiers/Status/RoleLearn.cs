using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Object = UnityEngine.Object;

namespace AmongUsSalem.LifeImprovement.Roles;

#region DeepfakeRole
#endregion
// This is used to make players see roles of their target.
// Example: Consigliere revealing a player, Coroner finding killer, etc.
public sealed class RoleLearn(PlayerControl visitor, bool showRevealNotif = false) : BaseModifier
{
    public override string ModifierName => "Role Learn";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Visitor = visitor;

    public override void OnActivate()
    {
        base.OnActivate();

        if (Visitor.AmOwner() && Visitor.Data.Role is ICustomAURole customRole && showRevealNotif)
        {
            MiscUtils.ShowNotification(MessageTexts.RevealRole(Player), Color.white, customRole.Configuration.Icon.LoadAsset());
            MiscUtils.AddFakeChat(Visitor.CachedPlayerData, MiscUtils.GetTitle(customRole.RoleColor, $"{customRole.RoleName} Info"), MessageTexts.RevealRole(Player));
        }
    }
}