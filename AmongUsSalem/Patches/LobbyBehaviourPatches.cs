using HarmonyLib;
using ObjectWorkshop.Modules;
using ObjectWorkshop.Roles;

namespace ObjectWorkshop.Patches;

[HarmonyPatch]
public static class LobbyBehaviourPatches
{
    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
    [HarmonyPostfix]
    public static void LobbyStartPatch(LobbyBehaviour __instance)
    {
        foreach (var role in GameHistory.AllRoles)
        {
            if (!role || role is not IOWRole touRole)
            {
                continue;
            }

            touRole.LobbyStart();
        }

        GameHistory.ClearAll();
        ScreenFlash.Clear();
        MeetingMenu.ClearAll();
    }
}