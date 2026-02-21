using HarmonyLib;
using TownOfUs.Modules;

namespace TownOfUs.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
public static class FirstDeadPatch
{
    public static List<string> PlayerNames { get; set; } = [];

    public static void Postfix(PlayerControl __instance, DeathReason reason)
    {
        if (PlayerNames.Count < 4)
        {
            PlayerNames.Add(__instance.name);
        }
        GameHistory.DeathHistory.Add((__instance.PlayerId, reason));
    }
}