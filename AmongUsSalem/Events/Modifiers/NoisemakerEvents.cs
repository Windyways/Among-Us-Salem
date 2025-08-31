using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Modifiers;
using AmongUsSalem.Modifiers.Game.Crewmate;
using AmongUsSalem.Roles.Neutral;
using AmongUsSalem.Utilities;

namespace AmongUsSalem.Events.Modifiers;

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