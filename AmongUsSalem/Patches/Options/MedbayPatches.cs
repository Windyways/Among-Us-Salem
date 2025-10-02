using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using AmongUsSalem.Options;

namespace AmongUsSalem.Patches.Options;

[HarmonyPatch(typeof(MedScanMinigame))]
public static class MedScanMinigameFixedUpdatePatch
{
    [HarmonyPatch(nameof(MedScanMinigame.FixedUpdate))]
    public static void Prefix(MedScanMinigame __instance)
    {
        if (OptionGroupSingleton<GeneralOptions>.Instance.ParallelMedbay)
        {
            // Allows multiple medbay scans at once
            __instance.medscan.CurrentUser = PlayerControl.LocalPlayer.PlayerId;
            __instance.medscan.UsersList.Clear();
        }
    }
}