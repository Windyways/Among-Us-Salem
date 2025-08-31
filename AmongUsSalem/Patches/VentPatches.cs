using HarmonyLib;
using MiraAPI.GameOptions;
using AmongUsSalem.Options;

namespace AmongUsSalem.Patches;

[HarmonyPatch]
public static class VentPatches
{
    public static bool InVision(PlayerControl player)
    {
        var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
        var vector = player.GetTruePosition() - truePosition;
        var magnitude = vector.magnitude;

        if (magnitude < PlayerControl.LocalPlayer.lightSource.viewDistance &&
            !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, magnitude,
                Constants.ShipAndObjectsMask))
        {
            return true;
        }

        return false;
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.EnterVent))]
    [HarmonyPrefix]
    public static bool EnterVentPatch(Vent __instance, PlayerControl pc)
    {
        if (!__instance.EnterVentAnim)
        {
            return true;
        }

        return true;
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.ExitVent))]
    [HarmonyPrefix]
    public static bool ExitVentPatch(Vent __instance, PlayerControl pc)
    {
        if (!__instance.ExitVentAnim)
        {
            return true;
        }

        return true;
    }
}