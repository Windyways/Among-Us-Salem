using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Modifiers;
using ObjectWorkshop.Modifiers;

namespace ObjectWorkshop.Events.Misc;

public static class ScatterEvents
{
    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        //Logger<ObjectWorkshopPlugin>.Error($"ScatterEvents - RoundStartHandler");

        ModifierUtils.GetActiveModifiers<ScatterModifier>().Do(x => x.OnRoundStart());
    }
}