namespace AmongUsSalem.Roles;

public static class Mayor_Events
{
    [RegisterEvent()]
    public static void HandleVoteEvent(HandleVoteEvent @event)
    {
        if (@event.VoteData.Owner.Data.Role is not Mayor mayor || !mayor.Player.HasModifier<GlobalReveal>()/* || mayor.Player.IsSilenced()*/)
        {
            return;
        }

        @event.VoteData.SetRemainingVotes(0);

        for (var i = 0; i < mayor.Votes; i++)
        {
            @event.VoteData.VoteForPlayer(@event.TargetId);
        }

        @event.Cancel();
    }
}