using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using ObjectWorkshop.Modules;
using ObjectWorkshop.Roles.Neutral;
using ObjectWorkshop.Utilities;

namespace ObjectWorkshop.Events.Neutral;

public static class SoulCollectorEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;
        var target = @event.Target;

        if (source.IsRole<SoulCollectorRole>() && !MeetingHud.Instance)
            // leave behind standing body
            // Logger<ObjectWorkshopPlugin>.Message($"Leaving behind soulless player '{target.Data.PlayerName}'");
        {
            _ = new FakePlayer(target);
        }
    }
}