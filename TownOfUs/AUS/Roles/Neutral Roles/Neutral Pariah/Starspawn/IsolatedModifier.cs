namespace AmongUsSalem.Modifiers;

public sealed class IsolatedModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Isolated";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public enum State { Isolated, PreviouslyIsolated }

    public State state = State.Isolated;
    public PlayerControl Caster => c;
    public override void OnMeetingStart()
    {
        if (state == State.PreviouslyIsolated) Player.RpcRemoveModifier<IsolatedModifier>();
        state = State.PreviouslyIsolated;
    }

    public int PerformInteraction(CustomActionButton button, PlayerControl visitor, PlayerControl target)
    {
        if (Caster.AmOwner()) Starspawn.RpcNotify(Caster, (int)NotificationType.Starspawn_Isolate, visitor);

        if (visitor.Is(Faction.Town))
        {
            visitor.Notify(Feedback.UnknownObstacle(target), NotifyMode.InstantlyAndMeeting);

            if (visitor.AmOwner() && button != null)
            {
                if (visitor.Data.Role is Seer seer) seer.fullCooldown = true;
                button.ResetCooldownAndOrEffect();
            }
            return 100; // Block visit and everything else.
        }

        if (visitor.AmOwner()) Starspawn.RpcNotify(visitor, (int)NotificationType.Starspawn_IsolateButImmune, visitor);
        return 0; // Do not Block evil visits.
    }
}