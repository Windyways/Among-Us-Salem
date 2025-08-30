using HarmonyLib;
using ObjectWorkshop.Modules;
using ObjectWorkshop.Roles;

namespace ObjectWorkshop.LifeImprovement.Patches;

[HarmonyPatch]
public static class LobbyBehaviour_Start
{
    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
    [HarmonyPostfix]
    public static void LobbyStartPatch(LobbyBehaviour __instance)
    {
        Statistics.Round = 1;
        OnGameStart.SequenceCheck = 0;
    }
}