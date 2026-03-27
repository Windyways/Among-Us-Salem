using Reactor.Utilities.Extensions;

namespace AmongUsSalem.Patches;

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
            if (player.Data.Role is ICustomAURole ausRole)
            {
                __instance.completeString = player.Name() + "'s Role Was <b><color=#" + ausRole.RoleColor.ToHtmlStringRGBA() + $">{ausRole.RoleName}</color></b>!";
            }
        }

        __instance.ImpostorText.text = "";
    }
}