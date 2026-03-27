using System.Collections;
using UnityEngine;

namespace AmongUsSalem.Events;

public static class AUS_ReportBodyEvent
{
    [RegisterEvent]
    public static void ReportBodyEvent(ReportBodyEvent @event)
    {
        AUS_AfterVoteEvent.Voters.Clear();
        if (@event.Body == null) return;

        var user = @event.Reporter;
        if (@event.Target == null) return;

        var target = MiscUtils.PlayerById(@event.Target.PlayerId);
        if (target == null) return;

        Coroutines.Start(CoSuccessfulVisit(user, target));
    }

    public static IEnumerator CoSuccessfulVisit(PlayerControl user, PlayerControl target)
    {
        yield return new WaitForSeconds(0.1f);
        VisitingMechanic.IsSuccessfulVisit(null, user, target, false, true);
    }
}