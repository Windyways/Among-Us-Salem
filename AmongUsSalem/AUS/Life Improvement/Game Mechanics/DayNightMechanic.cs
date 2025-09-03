using System.Collections;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using static MeetingHud;

namespace AmongUsSalem.LifeImprovement.GameMechanics;

public static class DayNightMechanic
{
    public static float PostMeetingIntroTime = 7f;
    public static int DayCount;
    public static int NightCount;
    public static float NightTimer;

    public static bool FullMoon()
    {
        return NightCount == 2 || NightCount >= 4;
    }

    public static bool HalfMoon()
    {
        return !FullMoon();
    }

    public static void OnLobbyStart()
    {
        DayCount = 0;
        NightCount = 0;
    }

    #region RPCs
    #endregion
    [MethodRpc((uint)AUSRpc.StartDayOne, SendImmediately = true)]
    public static void StartDayOne(PlayerControl player)
    {
        if (AmongUsClient.Instance.AmHost)
        {
            MeetingRoomManager.Instance.AssignSelf(player, null);

            if (GameManager.Instance.CheckTaskCompletion())
            {
                return;
            }

            HudManager.Instance.OpenMeetingRoom(player);
            player.RpcStartMeeting(null);
        }
    }

    #region Events
    #endregion
    [RegisterEvent(-1)]
    public static void StartMeetingEventHandler(StartMeetingEvent @event)
    {
        DayCount++;

        foreach (var role in GameHistory.AllRoles)
        {
            if (!role || role is not IAUSRole ausRole)
            {
                continue;
            }

            ausRole.Attack = ausRole.ogAttack;
            ausRole.Defense = ausRole.ogDefense;
            ausRole.EtherealDefense = ausRole.ogEtherealDefense;
        }
        
        Statistics.HasMurder.Clear();
    }

    [RegisterEvent(-1)]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            return; // Only run when round starts.
        }

        NightCount++;
    }

    [RegisterEvent]
    public static void GameStartHandler(RoundStartEvent @event)
    {
        if (!@event.TriggeredByIntro)
        {
            return; // Only run when game starts.
        }

        //if (TutorialManager.InstanceExists) PathfindingExperiment.BuildRoomGraphOnce();
        //else 
        StartDayOne(PlayerControl.LocalPlayer);
    }
}

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
public static class VotingComplete
{
    public static void Postfix(MeetingHud __instance, [HarmonyArgument(0)] Il2CppStructArray<VoterState> states, [HarmonyArgument(1)] NetworkedPlayerInfo exiled, [HarmonyArgument(2)] bool tie)
    {
        if (__instance.exiledPlayer != null)
        {
            // var votedPlayer = MiscUtils.PlayerById(__instance.exiledPlayer.PlayerId);
            if (DayNightMechanic.DayCount == 1)
            {
                __instance.exiledPlayer = null;
            }
        }
    }
}