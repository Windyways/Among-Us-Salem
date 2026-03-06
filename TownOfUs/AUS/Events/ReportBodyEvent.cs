namespace AmongUsSalem.Events;

public static class AUS_ReportBodyEvent
{
    [RegisterEvent]
    public static void ReportBodyEvent(ReportBodyEvent @event)
    {
        AUS_AfterVoteEvent.Voters.Clear();

        var player = @event.Reporter;
        if (@event.Target == null)
            return;

        var target = MiscUtils.PlayerById(@event.Target.PlayerId);
        if (target == null)
            return;

        VisitingMechanic.IsSuccessfulVisit(null, player, target, false, true);
    }
}