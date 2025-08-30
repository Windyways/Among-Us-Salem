using HarmonyLib;
using ObjectWorkshop.Roles.Neutral;

namespace ObjectWorkshop.Patches.Roles;

[HarmonyPatch]
public static class JesterNoVentMovePatch
{
    [HarmonyPatch(typeof(Vent), nameof(Vent.SetButtons))]
    [HarmonyPrefix]
    public static bool JesterEnterVent()
    {
        if (PlayerControl.LocalPlayer == null)
        {
            return true;
        }

        if (PlayerControl.LocalPlayer.Data == null)
        {
            return true;
        }

        if (PlayerControl.LocalPlayer.Data.Role is JesterRole)
        {
            return false;
        }

        return true;
    }
}