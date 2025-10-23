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

        if (@event.Reporter.AmOwner)
        {
            MiscUtils.PostSuccessfulVisit(@event.Reporter, target, false, true);
            @event.UnCancel();
        }
    }
}