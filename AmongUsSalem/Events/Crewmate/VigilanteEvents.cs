using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using AmongUsSalem.Modules;
using AmongUsSalem.Roles.Crewmate;

namespace AmongUsSalem.Events.Neutral;

public static class VigilanteEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;

        if (source.Data.Role is not VigilanteRole vigi)
        {
            return;
        }

        vigi.MaxKills--;

        var target = @event.Target;

        if (GameHistory.PlayerStats.TryGetValue(source.PlayerId, out var stats))
        {
            if (source != target)
            {
                stats.CorrectAssassinKills++;
            }
            else
            {
                stats.CorrectAssassinKills--;
            }
        }
    }
}