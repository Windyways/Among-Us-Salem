using HarmonyLib;
using MiraAPI.GameOptions;
using AmongUsSalem.Options;

namespace AmongUsSalem.Patches.PrefabSwitching;

[HarmonyPatch]
public static class AirshipDoors
{
    [HarmonyPatch(typeof(AirshipStatus), nameof(AirshipStatus.OnEnable))]
    [HarmonyPostfix]
    public static void Postfix(AirshipStatus __instance)
    {
        if (!OptionGroupSingleton<AirshipOptions>.Instance.AirshipPolusDoors)
        {
            return;
        }

        var polusdoor = PrefabLoader.Polus.GetComponentInChildren<DoorConsole>().MinigamePrefab;
        foreach (var door in __instance.GetComponentsInChildren<DoorConsole>())
        {
            door.MinigamePrefab = polusdoor;
        }
    }
}