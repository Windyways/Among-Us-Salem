using HarmonyLib;
using AmongUsSalem.Modules;
using AmongUsSalem.Roles;

namespace AmongUsSalem.LifeImprovement.Patches;

[HarmonyPatch]
public static class ExileController_BeginForGameplay
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.BeginForGameplay))]
    [HarmonyPostfix]
    public static void ExileControllerPatch(ExileController __instance)
    {
		NetworkedPlayerInfo exiled = __instance.initData.networkedPlayer;
        if (exiled != null)
        {
            PlayerControl player = exiled.Object;
            CalculatedVoting.EvidenceAgainst.Remove(player);
            CalculatedVoting.KillerContagious.Remove(player);
            CalculatedVoting.QueueEvidenceAgainst.Remove(player);
            CalculatedVoting.QueueKillerContagious.Remove(player);

            if (player.Data.Role is ICustomAURole ausRole)
            {
                __instance.completeString = player.GetDefaultAppearance().PlayerName + "'s Role Was <b><color=#" + ausRole.RoleColor.ToHtmlStringRGBA() + $">{ausRole.RoleName}</color></b>!";
            }
        }
    }
}