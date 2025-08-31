using AmongUs.Data.Player;
using HarmonyLib;

namespace AmongUsSalem.Patches.Misc;

[HarmonyPatch(typeof(PlayerBanData), nameof(PlayerBanData.IsBanned), MethodType.Getter)]
public static class BanPatch
{
    [HarmonyPrefix]
    public static bool Prefix(ref bool __result)
    {
        __result = false;
        return false;
    }
}