using HarmonyLib;
using MiraAPI.GameEnd;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using AmongUsSalem.Events;
using AmongUsSalem.GameOver;
using AmongUsSalem.Modifiers;
using AmongUsSalem.Modifiers.Crewmate;
using AmongUsSalem.Modifiers.Game;
using AmongUsSalem.Modifiers.Game.Alliance;
using AmongUsSalem.Options;
using AmongUsSalem.Options.Roles.Impostor;
using AmongUsSalem.Roles;
using AmongUsSalem.Utilities;

namespace AmongUsSalem.Patches;

[HarmonyPatch]
public static class LogicGameFlowPatches
{
    public static bool CheckEndGameViaTasks(LogicGameFlowNormal instance)
    {
        GameData.Instance.RecomputeTaskCounts();

        if (GameData.Instance.TotalTasks > 0 && GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks)
        {
            instance.Manager.RpcEndGame(GameOverReason.CrewmatesByTask, false);

            return true;
        }

        return false;
    }

    [HarmonyPatch(typeof(GameData), nameof(GameData.RecomputeTaskCounts))]
    [HarmonyPrefix]
    private static bool RecomputeTasksPatch(GameData __instance)
    {
        if (__instance == null)
        {
            return false;
        }

        __instance.TotalTasks = 0;
        __instance.CompletedTasks = 0;
        for (var i = 0; i < __instance.AllPlayers.Count; i++)
        {
            var playerInfo = __instance.AllPlayers.ToArray()[i];
            if (!playerInfo.Disconnected && playerInfo.Tasks != null && playerInfo.Object &&
                (GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks || !playerInfo.IsDead) &&
                !playerInfo._object.IsImpostor() &&
                !(
                    (playerInfo._object.TryGetModifier<AllianceGameModifier>(out var allyMod) && !allyMod.DoesTasks)
                    || !playerInfo._object.Data.Role.TasksCountTowardProgress
                ))
            {
                for (var j = 0; j < playerInfo.Tasks.Count; j++)
                {
                    __instance.TotalTasks++;
                    if (playerInfo.Tasks.ToArray()[j].Complete)
                    {
                        __instance.CompletedTasks++;
                    }
                }
            }
        }

        if (__instance.TotalTasks == 0)
        {
            __instance.TotalTasks =
                1; // This results in avoiding unfair task wins by essentially defaulting to 0/1 which can never lead to a win
        }

        return false;
    }

    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
    [HarmonyPrefix]
    public static bool CheckEndCriteriaPatch(LogicGameFlowNormal __instance)
    {
        if (TutorialManager.InstanceExists)
        {
            return true;
        }

        if (!AmongUsClient.Instance.AmHost)
        {
            return false;
        }

        if (!GameData.Instance)
        {
            return false;
        }
        
        // Prevents game end on exile screen
        if (ExileController.Instance)
        {
            return false;
        }

        if (DeathHandlerModifier.IsCoroutineRunning || DeathEventHandlers.IsDeathRecent)
        {
            return false;
        }

        if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.LifeSupp))
        {
            var lifeSuppSystemType = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
            if (lifeSuppSystemType is { Countdown: < 0f })
            {
                __instance.EndGameForSabotage();
                lifeSuppSystemType.Countdown = 10000f;

                return false;
            }
        }

        foreach (var systemType2 in ShipStatus.Instance.Systems.Values)
        {
            var sabo = systemType2.TryCast<ICriticalSabotage>();
            if (sabo == null)
            {
                continue;
            }

            var criticalSabotage = sabo;
            if (criticalSabotage != null && criticalSabotage.Countdown < 0f)
            {
                __instance.EndGameForSabotage();
                criticalSabotage.ClearSabotage();
            }
        }

        if (CheckEndGameViaTasks(__instance))
        {
            return false;
        }

        // End game if there are 3 players alive and 2 are lovers.
        var activeLovers = ModifierUtils.GetActiveModifiers<LoverModifier>().ToArray();
        if (!ExileController.Instance && LoverModifier.WinConditionMet(activeLovers))
        {
            CustomGameOver.Trigger<LoverGameOver>(activeLovers.Select(x => x.Player.Data).ToArray());
            return false;
        }

        // If any coven win condition is met -> game over
        // Using Alignment as a quick and basic way to prioritise NeutralEvil wins over NeutralKiller wins
        if (CustomRoleUtils.GetActiveRolesOfTeam(ModdedRoleTeams.Custom)
                .OrderBy(x => (x as IAUSRole)!.RoleFaction == Faction.Coven)
                .FirstOrDefault(x => x is IAUSRole role && role.WinConditionMet()) is { } winner2)
        {
            Logger<AUSPlugin>.Message($"Game Over");
            CustomGameOver.Trigger<CovenGameOver>([winner2.Player.Data]);

            return false;
        }

        // If any neutral win condition is met -> game over
        // Using Alignment as a quick and basic way to prioritise NeutralEvil wins over NeutralKiller wins
        if (CustomRoleUtils.GetActiveRolesOfTeam(ModdedRoleTeams.Custom)
                .OrderBy(x => (x as IAUSRole)!.Alignment)
                .FirstOrDefault(x => x is IAUSRole role && role.WinConditionMet()) is { } winner)
        {
            Logger<AUSPlugin>.Message($"Game Over");
            CustomGameOver.Trigger<NeutralGameOver>([winner.Player.Data]);

            return false;
        }

        // Prevents game end when all impostors are dead but there are neutral killers left alive
        if (MiscUtils.intKillersAliveCount > 0 ||
            (MiscUtils.ImpAliveCount > 0 && MiscUtils.CrewKillersAliveCount() > 0))
        {
            return false;
        }

        // Causes the game to draw in extreme scenarios
        if (Helpers.GetAlivePlayers().Count <= 0)
        {
            var randomPlayer = PlayerControl.AllPlayerControls.ToArray().Where(x =>
                !x.Data.Role.DidWin(CustomGameOver.GameOverReason<DrawGameOver>()) && !x.GetModifiers<GameModifier>()
                    .Any(x => x.DidWin(CustomGameOver.GameOverReason<DrawGameOver>()) == true)).Random();
            CustomGameOver.Trigger<DrawGameOver>([
                randomPlayer != null ? randomPlayer.Data : PlayerControl.LocalPlayer.Data
            ]);
        }

        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
    public static void IsGameOverDueToDeathPatch(LogicGameFlowNormal __instance, ref bool __result)
    {
        __result = false;
    }
}