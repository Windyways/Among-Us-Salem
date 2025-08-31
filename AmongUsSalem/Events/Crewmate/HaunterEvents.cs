using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Player;
using AmongUsSalem.Roles.Crewmate;

namespace AmongUsSalem.Events.Crewmate;

public static class HaunterEvents
{
    [RegisterEvent]
    public static void CompleteTaskEventHandler(CompleteTaskEvent @event)
    {
        if (@event.Player.Data.Role is not HaunterRole haunter)
        {
            return;
        }

        haunter.CheckTaskRequirements();
    }
}