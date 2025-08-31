using HarmonyLib;
using AmongUsSalem.Modules;
using AmongUsSalem.Roles;

namespace AmongUsSalem.LifeImprovement.Patches;

[HarmonyPatch]
public static class ExileController_BeginForGameplay
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.BeginForGameplay))]
    [HarmonyPostfix]
    public static void ExileControllerPatch(ExileController __instance)
    {
        Statistics.Round++;
    }
}