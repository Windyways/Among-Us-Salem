using Discord;
using HarmonyLib;

namespace AmongUsSalem.Patches.Misc;

[HarmonyPatch]
public static class DiscordStatus
{
    [HarmonyPatch(typeof(ActivityManager), nameof(ActivityManager.UpdateActivity))]
    [HarmonyPrefix]
    public static void Prefix([HarmonyArgument(0)] Activity activity)
    {
        activity.Details += $" - Among Us Salem III v{AUSPlugin.Version}" + (AUSPlugin.IsDevBuild ? " (DEV)" : string.Empty);
    }
}