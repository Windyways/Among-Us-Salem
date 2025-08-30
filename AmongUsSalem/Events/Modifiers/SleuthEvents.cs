using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Modifiers;
using ObjectWorkshop.Modifiers.Game.Universal;

namespace ObjectWorkshop.Events.Modifiers;

public static class SleuthEvents
{
    [RegisterEvent]
    public static void ReportBodyEventHandler(ReportBodyEvent @event)
    {
        var player = @event.Reporter;
        var target = @event.Target;

        if (player == null || target == null || !player.HasModifier<SleuthModifier>())
        {
            return;
        }

        var mod = player.GetModifier<SleuthModifier>();
        mod?.Reported.Add(target.PlayerId);

        // Logger<ObjectWorkshopPlugin>.Error($"SleuthEvents.ReportBodyEventHandler '{target.PlayerName}'");
    }
}