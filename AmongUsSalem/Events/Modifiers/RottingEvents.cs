using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Modifiers;
using Reactor.Utilities;
using ObjectWorkshop.Modifiers.Game.Crewmate;
using ObjectWorkshop.Roles.Neutral;
using ObjectWorkshop.Utilities;

namespace ObjectWorkshop.Events.Modifiers;

public static class RottingEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        if (@event.Target.HasModifier<RottingModifier>() && !@event.Source.IsRole<SoulCollectorRole>() &&
            !MeetingHud.Instance)
        {
            Coroutines.Start(RottingModifier.StartRotting(@event.Target));
        }
    }
}