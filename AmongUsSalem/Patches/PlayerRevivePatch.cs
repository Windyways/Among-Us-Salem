using HarmonyLib;
using MiraAPI.Events;
using AmongUsSalem.Events.TouEvents;

namespace AmongUsSalem.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Revive))]
public static class PlayerRevivePatch
{

    public static void Postfix(PlayerControl __instance)
    {
        var reviveEvent = new PlayerReviveEvent(__instance);
        MiraEventManager.InvokeEvent(reviveEvent);
    }
}