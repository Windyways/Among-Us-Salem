using Discord;
using HarmonyLib;

namespace ObjectWorkshop.Patches.Misc;

[HarmonyPatch]
public static class DiscordStatus
{
    [HarmonyPatch(typeof(ActivityManager), nameof(ActivityManager.UpdateActivity))]
    [HarmonyPrefix]
    public static void Prefix([HarmonyArgument(0)] Activity activity)
    {
        activity.Details += $" - Object Workshop v{ObjectWorkshopPlugin.Version}" + (ObjectWorkshopPlugin.IsDevBuild ? " (DEV)" : string.Empty);
    }
}