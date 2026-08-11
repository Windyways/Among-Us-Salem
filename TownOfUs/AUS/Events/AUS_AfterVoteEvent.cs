using MiraAPI.Events.Vanilla.Meeting.Voting;

namespace AmongUsSalem.Events;

public static class AUS_AfterVoteEvent
{
    public static List<(PlayerControl, byte)> Voters = new List<(PlayerControl, byte)>();

    [RegisterEvent]
    public static void AfterVoteEvent(AfterVoteEvent @event)
    {
        var target = @event.VoteArea.GetPlayer();
        if (target == null)
        {
            RoleFunctionOnSkip(@event.Player);
            return;
        }

        RoleFunctionOnVote(@event.Player, target);
    }
    
    public static void RoleFunctionOnVote(PlayerControl voter, PlayerControl target)
    {
        Voters.Add((voter, target.PlayerId));
    }

    public static void RoleFunctionOnSkip(PlayerControl voter)
    {
        Voters.Add((voter, MeetingHud.Instance.SkipVoteButton.TargetPlayerId));
    }
}