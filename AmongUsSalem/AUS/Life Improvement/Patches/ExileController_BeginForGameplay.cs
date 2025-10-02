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

            if (player.Data.Role is IAUSRole ausRole)
            {
                if (GameOptionsManager.Instance.currentNormalGameOptions.ConfirmImpostor)
                {
                    /*bool flag3 = player.Is(ModifierEnum.HiddenRoles);
                    if (flag3)
                    {
                        __instance.completeString = player.GetDefaultOutfit().PlayerName + "'s Role Was <b><color=#a9a9a9>Hidden</color></b>!";
                    }
                    else
                    {
                        __instance.completeString = player.GetDefaultOutfit().PlayerName + "'s Role Was " + Colors.GetColorRole(role, true) + "!";
                    }*/

                    var roleColor = ausRole.Faction != Faction.Neutral ? MiscUtils.GetFactionColour(player) : MiscUtils.GetRoleColour(ausRole.RoleName);
                    __instance.completeString = player.GetDefaultAppearance().PlayerName + "'s Role Was <b><color=#" + roleColor.ToHtmlStringRGBA() + $">{ausRole.RoleName}</color></b>!";
                }
                else
                {
                    __instance.completeString = player.GetDefaultAppearance().PlayerName + " Was Lynched";
                }
            }
        }
    }
}