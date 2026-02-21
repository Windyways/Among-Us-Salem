using HarmonyLib;
using TownOfUs.Modifiers;

namespace TownOfUs.Patches.Roles;

[HarmonyPatch]
public static class GlitchPatches
{
    [HarmonyPatch(typeof(ReportButton), nameof(ReportButton.DoClick))]
    [HarmonyPriority(Priority.First)]
    [HarmonyPrefix]
    public static bool DisabledReportButtonPatch(ActionButton __instance)
    {
        if (PlayerControl.LocalPlayer.GetModifiers<DisabledModifier>().Any(x => !x.CanReport))
        {
            return false;
        }

        return true;
    }

    [HarmonyPatch(typeof(VentButton), nameof(VentButton.DoClick))]
    [HarmonyPatch(typeof(UseButton), nameof(UseButton.DoClick))]
    [HarmonyPatch(typeof(SabotageButton), nameof(SabotageButton.DoClick))]
    [HarmonyPriority(Priority.First)]
    [HarmonyPrefix]
    public static bool GlitchHackedSabotageButtonPatch(ActionButton __instance)
    {
        return true;
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.ToggleMapVisible))]
    [HarmonyPrefix]
    public static bool GlitchHackedToggleMapVisiblePatch(HudManager __instance)
    {
        if (PlayerControl.LocalPlayer.GetModifiers<DisabledModifier>().Any(x => !x.CanUseAbilities))
        {
            return false;
        }

        return true;
    }
}