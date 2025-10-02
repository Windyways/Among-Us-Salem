using System.Collections;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using TMPro;
using UnityEngine;
using static MeetingHud;
using Object = UnityEngine.Object;

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
        Statistics.HasMurder.Clear();

        Coroutines.Start(DelayResetDefense());
    }

    public static IEnumerator DelayResetDefense() // For attacks that occur when Day begins, (eg. Shroud, SK/ww/war jailor counterattack)
    {
        yield return new WaitForSeconds(1f);
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
        if (AmongUsClient.Instance.AmHost) StartDayOne(PlayerControl.LocalPlayer);
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

[HarmonyPatch]
public static class ShowDayNight
{
    public static GameObject NightTimeObj;
    public static GameObject TimerSpriteObj;
    public static SpriteRenderer TimerSprite;
    public static bool Enabled { get; set; }
    public static float NightTime { get; set; }

    private static void CreateNightTime(HudManager instance)
    {
        var pingTracker = Object.FindObjectOfType<PingTracker>(true);
        NightTimeObj = Object.Instantiate(pingTracker.gameObject, instance.transform);
        NightTimeObj.name = "NightTimeText";

        NightTimeObj.GetComponent<AspectPosition>().DistanceFromEdge = new Vector3(-0.6f, 5.5f);
        NightTimeObj.GetComponent<AspectPosition>().Alignment = AspectPosition.EdgeAlignments.Bottom;

        TimerSpriteObj = new GameObject("TimerSprite");
        TimerSpriteObj.transform.SetParent(NightTimeObj.transform);
        TimerSpriteObj.transform.localPosition = new Vector3(-1f, -0.4f, 1f);
        TimerSpriteObj.gameObject.layer = NightTimeObj.gameObject.layer;
        TimerSpriteObj.SetActive(true);

        //TimerSprite = TimerSpriteObj.AddComponent<SpriteRenderer>();
        //TimerSprite.sprite = TouAssets.TimerDrawSprite.LoadAsset();

        var ts = TimeSpan.FromSeconds(NightTime);

        var timerText = NightTimeObj.GetComponent<TextMeshPro>();
        timerText.text = $"<size=200%>Time:{ts.ToString(@"mm\:ss", AUSPlugin.Culture)}</size>";
        timerText.alignment = TextAlignmentOptions.TopLeft;
        timerText.verticalAlignment = VerticalAlignmentOptions.Top;

        NightTimeObj.SetActive(false);
    }

    public static void UpdateNightTime(HudManager instance)
    {
        if (NightTimeObj != null)
        {
            NightTimeObj.SetActive(false);
        }

        if (NightTimeObj == null)
        {
            CreateNightTime(instance);
        }

        if (NightTimeObj == null)
        {
            return;
        }

        var inMeeting = MeetingHud.Instance || ExileController.Instance;

        if (Enabled && NightTime > 0 && !inMeeting && OptionGroupSingleton<AUSOptions>.Instance.NightTimer)
        {
            NightTime -= Time.deltaTime;
            NightTime = Math.Max(NightTime, 0);

            if (AmongUsClient.Instance.AmHost && NightTime <= 0)
            {
                DayNightMechanic.StartDayOne(PlayerControl.LocalPlayer);
            }
        }

        var ts = TimeSpan.FromSeconds(NightTime);

        var timerText = NightTimeObj.GetComponent<TextMeshPro>();


        if (!MeetingHud.Instance)
        {
            var colour = new Color(0.57f, 0.12f, 0.34f, 1f);
            NightTimeObj.GetComponent<AspectPosition>().DistanceFromEdge = new Vector3(-0.6f, 5.5f);
            NightTimeObj.GetComponent<AspectPosition>().Alignment = AspectPosition.EdgeAlignments.Bottom;
            if (OptionGroupSingleton<AUSOptions>.Instance.NightTimer)
            {
                timerText.text =
                    DayNightMechanic.FullMoon()
                    ? $"<size=130%>{colour.ToTextColor()}Night {DayNightMechanic.NightCount} (Full Moon)\n{ts.ToString(@"mm\:ss", AUSPlugin.Culture)}</color></size>"
                    : $"<size=130%>{colour.ToTextColor()}Night {DayNightMechanic.NightCount} (Half Moon)\n{ts.ToString(@"mm\:ss", AUSPlugin.Culture)}</color></size>";
            }
            else
            {
                timerText.text =
                    DayNightMechanic.FullMoon()
                    ? $"<size=130%>{colour.ToTextColor()}Night {DayNightMechanic.NightCount} (Full Moon)</color></size>"
                    : $"<size=130%>{colour.ToTextColor()}Night {DayNightMechanic.NightCount} (Half Moon)</color></size>";
            }
            TimerSpriteObj.transform.localPosition = new Vector3(-1f, -0.4f, 1f);
        }
        else
        {
            var colour = Color.yellow;
            NightTimeObj.GetComponent<AspectPosition>().DistanceFromEdge = new Vector3(-0.25f, 0.9f);
            NightTimeObj.GetComponent<AspectPosition>().Alignment = AspectPosition.EdgeAlignments.Bottom;
            timerText.text = $"<size=130%>{colour.ToTextColor()}Day {DayNightMechanic.DayCount}</color></size>";
            TimerSpriteObj.transform.localPosition = new Vector3(-1f, -0.25f, 1f);
        }

        NightTimeObj.SetActive(!ExileController.Instance);
    }

    public static void BeginTimer()
    {
        Enabled = true;
        NightTime = OptionGroupSingleton<AUSOptions>.Instance.NightDuration + 1f;

        // TimerSprite.sprite = TouAssets.TimerDrawSprite.LoadAsset();
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPostfix]
    public static void HudManagerUpdatePatch(HudManager __instance)
    {
        if (PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || PlayerControl.LocalPlayer.Data.Role == null || !ShipStatus.Instance || TutorialManager.InstanceExists || AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started)
            return;

        UpdateNightTime(__instance);
    }
    
    [RegisterEvent]
    public static void GameStartEventHandler(RoundStartEvent @event)
    {
        if (TutorialManager.InstanceExists)
        {
            return; // Shouldn't run in Freeplay
        }

        // begin timer
        BeginTimer();
    }
}