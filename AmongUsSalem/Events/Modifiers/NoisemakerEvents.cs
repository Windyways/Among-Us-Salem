using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Modifiers;
using ObjectWorkshop.Modifiers.Game.Crewmate;
using ObjectWorkshop.Roles.Neutral;
using ObjectWorkshop.Utilities;

namespace ObjectWorkshop.Events.Modifiers;

public static class NoisemakerEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        if (@event.Target.TryGetModifier<NoisemakerModifier>(out var noise) &&
            !@event.Source.IsRole<SoulCollectorRole>() && !MeetingHud.Instance)
        {
            noise.NotifyOfDeath(@event.Target);
        }
    }
}