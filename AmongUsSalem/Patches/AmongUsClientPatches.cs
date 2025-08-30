using HarmonyLib;
using Reactor.Utilities;

namespace ObjectWorkshop.Patches;

[HarmonyPatch]
public static class AmongUsClientPatches
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.Awake))]
    [HarmonyPostfix]
    public static void StartPatch(AmongUsClient __instance)
    {
        if (AmongUsClient.Instance != __instance)
        {
            Logger<ObjectWorkshopPlugin>.Error("AmongUsClient duplicate detected.");
        }
    }
}