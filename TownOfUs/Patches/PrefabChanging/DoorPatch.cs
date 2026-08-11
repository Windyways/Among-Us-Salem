using HarmonyLib;
using MiraAPI.GameOptions;
using TownOfUs.Options;

namespace TownOfUs.Patches.PrefabSwitching;

[HarmonyPatch]
public static class AirshipDoors
{
    [HarmonyPatch(typeof(AirshipStatus), nameof(AirshipStatus.OnEnable))]
    [HarmonyPostfix]
    public static void Postfix(AirshipStatus __instance)
    {

    }
}