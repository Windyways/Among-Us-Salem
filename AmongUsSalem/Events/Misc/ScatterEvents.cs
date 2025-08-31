using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Modifiers;
using AmongUsSalem.Modifiers;

namespace AmongUsSalem.Events.Misc;

public static class ScatterEvents
{
    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        //Logger<AUSPlugin>.Error($"ScatterEvents - RoundStartHandler");

        ModifierUtils.GetActiveModifiers<ScatterModifier>().Do(x => x.OnRoundStart());
    }
}