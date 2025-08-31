using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Modifiers;
using AmongUsSalem.Modifiers.Game.Crewmate;

namespace AmongUsSalem.Events.Modifiers;

public static class TaskmasterEvents
{
    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            return; // Only run when round starts.
        }

        ModifierUtils.GetActiveModifiers<TaskmasterModifier>().Do(x => x.OnRoundStart());
    }
}