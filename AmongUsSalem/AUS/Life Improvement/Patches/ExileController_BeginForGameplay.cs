using HarmonyLib;
using ObjectWorkshop.Modules;
using ObjectWorkshop.Roles;

namespace ObjectWorkshop.LifeImprovement.Patches;

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