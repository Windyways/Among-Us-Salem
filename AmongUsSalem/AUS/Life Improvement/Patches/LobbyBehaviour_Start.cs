using HarmonyLib;
using AmongUsSalem.Modules;
using AmongUsSalem.Roles;

namespace AmongUsSalem.LifeImprovement.Patches;

[HarmonyPatch]
public static class LobbyBehaviour_Start
{
    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
    [HarmonyPostfix]
    public static void LobbyStartPatch(LobbyBehaviour __instance)
    {
        CalculatedVoting.EvidenceAgainst.Clear();
        CalculatedVoting.KillerContagious.Clear();
        CalculatedVoting.RecievedInformation.Clear();
        CalculatedVoting.QueueEvidenceAgainst.Clear();
        CalculatedVoting.QueueKillerContagious.Clear();
        CalculatedVoting.QueueRecievedInformation.Clear();

        // Mechanics
        DayNightMechanic.OnLobbyStart();
        DayNightMechanic.NightTimer = 0;

        MafiosoPromotionMechanic.GodfatherDiedWithMafiosoAlive = false;

        Statistics.IsTrespassing.Clear();
        Statistics.HasMurder.Clear();
    }
}