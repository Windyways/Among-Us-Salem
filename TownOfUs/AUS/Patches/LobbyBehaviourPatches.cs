using AmongUsSalem.MafiaRoles;

namespace TownOfUs.Patches;

[HarmonyPatch]
public static class LobbyBehaviourPatches
{
    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
    [HarmonyPostfix]
    public static void LobbyStartPatch(LobbyBehaviour __instance)
    {
        foreach (var role in GameHistory.AllRoles)
        {
            if (!role || role is not ICustomAURole touRole)
            {
                continue;
            }

            touRole.LobbyStart();
        }

        GameHistory.ClearAll();
        ScreenFlash.Clear();
        MeetingMenu.ClearAll();

        ShowRoleIcon.ClearAll();

        // --- ROLES ---
        MafiosoPromotionMechanic.GodfatherDied = false;
        TouRoleManagerPatches.MafiaCount = 0;

        // --- MECHANICS ---
        DayNightMechanic.DayCount = 0;
        DayNightMechanic.NightCount = 0;

        RolelistMechanic.CovenCount = 0;

        if (RoleReferences.PendingNotifications.Count != 0)
        {
            foreach (var msg in RoleReferences.PendingNotifications)
            {
                MiscUtils.AddFakeChat(PlayerControl.LocalPlayer.CachedPlayerData, "PLACEMENT CHANGES", msg);
            }

            RoleReferences.PendingNotifications.Clear(); // prevent repeats
        }

        Debugger.RandomizeModes();
    }
}