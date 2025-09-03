using UnityEngine;

namespace AmongUsSalem.LifeImprovement;

public static class AUSEvents
{
    [RegisterEvent]
    public static void ReportBodyEventHandler(ReportBodyEvent @event)
    {
        if (@event.Target == null)
        {
            return;
        }

        var target = MiscUtils.PlayerById(@event.Target.PlayerId);
        if (target.IsAmbushed() && @event.Reporter.AmOwner)
        {
            MiscUtils.SuccessfulVisit(@event.Reporter, target, false, true);
        }
    }
}