using AmongUs.Data;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using InnerNet;
using MiraAPI.Hud;
using MiraAPI.Networking;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement.MCI;

public class Debugger : MonoBehaviour
{
    [HideFromIl2Cpp]
    public DragWindow Window { get; }
    public static bool IsDebuggerActive => AmongUsClient.Instance.NetworkMode == NetworkModes.LocalGame || AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay;
    public bool WindowEnabled { get; set; } = true;
    public Debugger(IntPtr ptrs) : base(ptrs)
    {
        Window = new(new(20, 20, 0, 0), "AUS Debugger", () =>
        {
            GUILayout.Label($"Name: {DataManager.Player.customization.Name} - PRESS F3 TO HIDE WINDOW");

            var mouse = Input.mousePosition;
            GUILayout.Label($"Mouse Position\nx: {mouse.x:00.00} y: {mouse.y:00.00} z: {mouse.z:00.00}");

            if (PlayerControl.LocalPlayer)
            {
                GUILayout.Label($"Name: {PlayerControl.LocalPlayer.CurrentOutfit.PlayerName}");
                var position = PlayerControl.LocalPlayer.gameObject.transform.position;
                GUILayout.Label($"Your Position\nx: {position.x:00.00} y: {position.y:00.00} z: {position.z:00.00}");

                if (!PlayerControl.LocalPlayer.Data.IsDead && IsDebuggerActive)
                {
                    PlayerControl.LocalPlayer.Collider.enabled = GUILayout.Toggle(PlayerControl.LocalPlayer.Collider.enabled, "Enable Player Collider");
                }
            }

            if (!IsDebuggerActive || !PlayerControl.LocalPlayer)
            {
                GUILayout.Label("DEBUGGER ONLY WORKS ON LOCAL-HOSTED GAMES");
                return;
            }

            if (!(AmongUsClient.Instance?.GameState == InnerNetClient.GameStates.Joined || AmongUsClient.Instance?.GameState == InnerNetClient.GameStates.Started
            || GameManager.Instance?.GameHasStarted == true && AmongUsClient.Instance?.GameState != InnerNetClient.GameStates.Ended))
                return;

            if (GUILayout.Button($"Spawn Bot ({InstanceControlPatches.Clients.Count}/15)"))
            {
                Keyboard_Joystick.CreatePlayer();
            }

            if (GUILayout.Button("Remove Last Bot")) InstanceControlPatches.RemovePlayer((byte)InstanceControlPatches.Clients.Count);
            if (GUILayout.Button("Remove All Bots")) InstanceControlPatches.RemoveAllPlayers();
            if (GUILayout.Button("Next Player")) Keyboard_Joystick.Switch(true);
            if (GUILayout.Button("Previous Player")) Keyboard_Joystick.Switch(false);
            if (GUILayout.Button("End Game") && !RoleReferences.CountRoundToLeaderboard) MiscUtils.EndGame();

            if (GUILayout.Button("Complete Tasks"))
            {
                foreach (var task in PlayerControl.LocalPlayer.myTasks)
                {
                    PlayerControl.LocalPlayer.RpcCompleteTask(task.Id);
                }
            }

            if (GUILayout.Button("Complete Everyone's Tasks"))
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    foreach (var task in player.myTasks)
                    {
                        player.RpcCompleteTask(task.Id);
                    }
                }
            }

            if (GUILayout.Button("Redo Intro Sequence"))
            {
                DestroyableSingleton<HudManager>.Instance.StartCoroutine(DestroyableSingleton<HudManager>.Instance.CoFadeFullScreen(Color.clear, Color.black));
                DestroyableSingleton<HudManager>.Instance.StartCoroutine(DestroyableSingleton<HudManager>.Instance.CoShowIntro());
            }

            if (!MeetingHud.Instance && GUILayout.Button("Start Meeting")) DayNightMechanic.StartDayOne(PlayerControl.LocalPlayer);
            if (GUILayout.Button("End Meeting") && MeetingHud.Instance) MeetingHud.Instance.RpcClose();
            if (GUILayout.Button("Kill Self")) PlayerControl.LocalPlayer.RpcCustomMurder(PlayerControl.LocalPlayer, didSucceed: true);
            if (GUILayout.Button("Kill All"))
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    player.RpcCustomMurder(player, didSucceed: true);
                }
            }

            if (GUILayout.Button("Remove Cooldowns"))
            {
                PlayerControl.LocalPlayer.SetKillTimer(0f);
                foreach (var button in CustomButtonManager.Buttons.Where(x => x.Enabled(PlayerControl.LocalPlayer.Data.Role)))
                {
                    button.SetTimer(0f);
                }
            }

            if (GUILayout.Button("Gain Charge"))
            {
                foreach (var button in CustomButtonManager.Buttons.Where(x => x.Enabled(PlayerControl.LocalPlayer.Data.Role)))
                {
                    button.IncreaseUses();
                }
            }

            if (GUILayout.Button("Do Cycle Mode"))
            {
                CycleMode.Start();
            }

            isRandomClientSwapping = GUILayout.Toggle(isRandomClientSwapping, "Enable Random Swapping");
            SmartBotsEnabled = GUILayout.Toggle(SmartBotsEnabled, "Enable Smart Bots");
            ShowAllMessages = GUILayout.Toggle(ShowAllMessages, "Show All Messages");
            RoleReferences.CountRoundToLeaderboard = GUILayout.Toggle(RoleReferences.CountRoundToLeaderboard, "Round Counts To Leaderboard");
        });
    }

    public void OnGUI()
    {
        if (WindowEnabled) Window.OnGUI();
    }

    public void Toggle()
    {
        WindowEnabled = !WindowEnabled;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3) || Input.GetKeyDown(KeyCode.F1)) Toggle();
    }

    private void Start()
    {
        WindowEnabled = false;
    }

    public static bool ShowAllMessages;
    public static bool isRandomClientSwapping;
    public static bool SmartBotsEnabled;
}