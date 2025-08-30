using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using ObjectWorkshop.Modifiers.Game.Universal;

namespace ObjectWorkshop.Events.Modifiers;

public static class ButtonBarryEvents
{
    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            return; // Never run when round starts.
        }

        ButtonBarryModifier.OnRoundStart();
    }
}