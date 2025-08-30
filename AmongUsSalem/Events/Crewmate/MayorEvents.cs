using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Meeting.Voting;
using ObjectWorkshop.Roles.Crewmate;

namespace ObjectWorkshop.Events.Crewmate;

public static class MayorEvents
{
    [RegisterEvent]
    public static void HandleVoteEvent(HandleVoteEvent @event)
    {
        if (@event.VoteData.Owner.Data.Role is not MayorRole mayor || !mayor.Revealed)
        {
            return;
        }

        @event.VoteData.SetRemainingVotes(0);

        for (var i = 0; i < 3; i++)
        {
            @event.VoteData.VoteForPlayer(@event.TargetId);
        }

        @event.Cancel();
    }
}