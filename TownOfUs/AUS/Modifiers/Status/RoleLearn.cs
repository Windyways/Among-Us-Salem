namespace AmongUsSalem.Modifiers;

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
            Visitor.Notify(Feedback.RevealRole(Player), NotifyMode.InstantlyAndMeeting, sprite: customRole.Configuration.Icon.LoadAsset());
        }
    }
}