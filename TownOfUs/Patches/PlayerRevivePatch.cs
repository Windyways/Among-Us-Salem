using HarmonyLib;
using MiraAPI.Events;
using TownOfUs.Events.TouEvents;

namespace TownOfUs.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Revive))]
public static class PlayerRevivePatch
{

    public static void Postfix(PlayerControl __instance)
    {
        var reviveEvent = new PlayerReviveEvent(__instance);
        MiraEventManager.InvokeEvent(reviveEvent);
    }
}