using MiraAPI.GameEnd;
using MiraAPI.Modifiers.Types;
using Reactor.Utilities.Extensions;
using TownOfUs.Events;
using TownOfUs.GameOver;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Game;

namespace AmongUsSalem.Patches;

[HarmonyPatch]
public static class LogicGameFlowPatches
{
    public static bool EndGameEarlyCheck(RoleBehaviour role)
    {
        /*var deadAmneRoles = PlayerControl.AllPlayerControls.ToArray().Count(x => x.HasDied() &&
            x.Data.Role is Veteran or Mayor or Prosecutor);

        var deadVetsOnAlert = PlayerControl.AllPlayerControls.ToArray().Count(x => x.HasDied() &&
            x.Data.Role is Veteran veteran && veteran.isAlerted);

        var deadFortified = PlayerControl.AllPlayerControls.ToArray().Count(x => x.HasDied() &&
            x.IsFortified() || x.IsJinxed() || x.IsAmbushed());
        */
        var enemyRoles = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && x.Data.Role != role).ToList();
        if (enemyRoles.Count == 1)
        {
            var other = enemyRoles[0];
            /*if (role is Jackal) // Jackal end game early checks
            {
                if (
                    other.Data.Role is HexMaster or VoodooMaster or Arsonist or Mayor
                    || (other.Data.Role is Conjurer conjurer && conjurer.Charges > 0)
                    || (other.Data.Role is Prosecutor prosecutor && prosecutor.Charges > 0)
                    || (other.Data.Role is Veteran veteran && veteran.Charges > 0)
                    || (other.Data.Role is Amnesiac && deadAmneRoles > 0)
                    || other.IsGuarded() || deadVetsOnAlert > 0
                    ) return false;

                return true;
            }
            else if (role is Shroud) // Shroud end game early checks
            {
                if (
                    other.Data.Role is HexMaster or VoodooMaster or Arsonist or Mayor or Jackal
                    || (other.Data.Role is Conjurer conjurer && conjurer.Charges > 0)
                    || (other.Data.Role is Prosecutor prosecutor && prosecutor.Charges > 0)
                    || (other.Data.Role is Veteran veteran && veteran.Charges > 0)
                    || (other.Data.Role is Amnesiac && deadAmneRoles > 0)
                    || other.IsGuarded() || deadVetsOnAlert > 0
                    ) return false;

                return true;
            }
            else if (role is Arsonist) // Arsonist end game early checks
            {
                if (
                    other.Data.Role is HexMaster or VoodooMaster or Mayor or Jackal
                    || (other.Data.Role is Conjurer conjurer && conjurer.Charges > 0)
                    || (other.Data.Role is Prosecutor prosecutor && prosecutor.Charges > 0)
                    || (other.Data.Role is Veteran veteran && veteran.Charges > 0 && !other.IsDoused())
                    || (other.Data.Role is Amnesiac && deadAmneRoles > 0)
                    || deadVetsOnAlert > 0
                    ) return false;

                return true;
            }
            else if (role is Vampire) // Vampire end game early checks
            {
                if (
                    other.Data.Role is HexMaster or VoodooMaster or Mayor or Jackal or Shroud or Arsonist
                    || (other.Data.Role is Conjurer conjurer && conjurer.Charges > 0)
                    || (other.Data.Role is Prosecutor prosecutor && prosecutor.Charges > 0)
                    || (other.Data.Role is Veteran veteran && veteran.Charges > 0)
                    || (other.Data.Role is Amnesiac && deadAmneRoles > 0)
                    || other.IsGuarded() || deadVetsOnAlert > 0
                    ) return false;

                return true;
            }
            else */if (role.Player.Is(Faction.Coven)) // Coven end game early checks
            {
                if (
                    other.Data.Role is /*Mayor or Jackal or Shroud or Vampire or*/ ImpostorRole
                    /*|| (other.Data.Role is Prosecutor prosecutor && prosecutor.Charges > 0)
                    || (other.Data.Role is Veteran veteran && veteran.Charges > 0)
                    || (other.Data.Role is Amnesiac && deadAmneRoles > 0)
                    || (other.Data.Role is Deputy deputy && deputy.Charges > 0)
                    || other.IsGuarded() || other.IsFortified() || deadVetsOnAlert > 0 || deadFortified > 0 || other.IsAmbushed()*/
                    ) return false;

                return true;
            }
            else if (role is ImpostorRole) // Mafia end game early checks
            {
                if (
                    /*other.Data.Role is Mayor or Jackal or Shroud or Vampire*/
                    other.Is(Faction.Coven)
                    /*|| (other.Data.Role is Prosecutor prosecutor && prosecutor.Charges > 0)
                    || (other.Data.Role is Veteran veteran && veteran.Charges > 0)
                    || (other.Data.Role is Amnesiac && deadAmneRoles > 0)
                    || (other.Data.Role is Deputy deputy && deputy.Charges > 0)
                    || other.IsGuarded() || other.IsFortified() || deadVetsOnAlert > 0 || deadFortified > 0 || other.IsJinxed()*/
                    ) return false;

                return true;
            }
        }

        return false;
    }

    public static bool IsInStalemate()
    {
        //var alivePlayers = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied());
        //var shrouds = PlayerControl.AllPlayerControls.ToArray().Count(x => x.IsRole<Shroud>() && !x.HasDied());
        //var vampires = PlayerControl.AllPlayerControls.ToArray().Count(x => x.IsRole<Vampire>() && !x.HasDied());
        //if (alivePlayers == 2 && shrouds == 1 && vampires == 1) return true;
        return false;
    }

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

    public static bool CheckEndGameViaTimeLimit(LogicGameFlowNormal instance)
    {
        
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

        if (CheckEndGameViaTimeLimit(__instance))
        {
            return false;
        }

        if (IsInStalemate())
        {
            var randomPlayer = PlayerControl.AllPlayerControls.ToArray().Where(x =>
                !x.Data.Role.DidWin(CustomGameOver.GameOverReason<DrawGameOver>()) && !x.GetModifiers<GameModifier>()
                    .Any(x => x.DidWin(CustomGameOver.GameOverReason<DrawGameOver>()) == true)).Random();
            CustomGameOver.Trigger<DrawGameOver>([randomPlayer != null ? randomPlayer.Data : PlayerControl.LocalPlayer.Data]);
        }

        // If any coven win condition is met -> game over
        if (CustomRoleUtils.GetActiveRolesOfTeam(ModdedRoleTeams.Custom)
            .FirstOrDefault(x => x is ICustomAURole role && role.WinConditionMet() && role.Faction == Faction.Coven) is { } winner4)
        {
            Logger<AUSPlugin>.Message($"Game Over");
            CustomGameOver.Trigger<CovenGameOver>([winner4.Player.Data]);

            return false;
        }

        // If any neutral win condition is met -> game over
        // Using RoleAlignment as a quick and basic way to prioritise NeutralEvil wins over NeutralKiller wins
        if (CustomRoleUtils.GetActiveRolesOfTeam(ModdedRoleTeams.Custom)
                .OrderBy(x => (x as ICustomAURole)!.Alignment)
                .FirstOrDefault(x => x is ICustomAURole role && role.WinConditionMet()) is { } winner)
        {
            Logger<AUSPlugin>.Message($"Game Over");
            CustomGameOver.Trigger<NeutralGameOver>([winner.Player.Data]);

            return false;
        }

        // Prevents game end when all impostors are dead but there are neutral killers left alive
        if (MiscUtils.NKillersAliveCount > 0 ||
            (MiscUtils.ImpAliveCount > 0 && MiscUtils.CrewKillersAliveCount > 0))
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