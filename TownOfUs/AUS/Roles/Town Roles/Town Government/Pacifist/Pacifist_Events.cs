namespace AmongUsSalem.Roles;

public static class Pacifist_Events
{
    [RegisterEvent()]
    public static void HandleVoteEvent(HandleVoteEvent @event)
    {
        if (!@event.VoteData.Owner.HasModifier<ProtestModifier>())
        {
            return;
        }

        @event.VoteData.SetRemainingVotes(0);
        @event.Cancel();
    }
}