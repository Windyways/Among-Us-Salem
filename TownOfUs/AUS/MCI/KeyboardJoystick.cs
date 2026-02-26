using UnityEngine;

namespace AmongUsSalem.MCI;

[HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
public static class Keyboard_Joystick
{
    public static int ControllingFigure;
    public static void Postfix()
    {
        if (!Debugger.IsDebuggerActive) return;

        if (Input.GetKeyDown(KeyCode.F5))
        {
            CreatePlayer();
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            Switch(true);
        }

        if (Input.GetKeyDown(KeyCode.F10))
        {
            Switch(false);
        }
        else if (Input.GetKeyDown(KeyCode.F6)) AUSPlugin.Persistence = !AUSPlugin.Persistence;

        if (Input.GetKeyDown(KeyCode.F11)) InstanceControlPatches.RemoveAllPlayers();
    }

    internal static void CreatePlayer()
    {
        ControllingFigure = PlayerControl.LocalPlayer.PlayerId;

        if (PlayerControl.AllPlayerControls.Count == 15 && !Input.GetKeyDown(KeyCode.LeftShift)) return; 
        //press f6 and f5 to bypass limit

        InstanceControlPatches.CleanUpLoad();
        InstanceControlPatches.CreatePlayerInstance();
    }

    internal static void Switch(bool increment)
    {
        if (!LobbyBehaviour.Instance)
        {
            if (Debugger.isRandomClientSwapping && !MeetingHud.Instance)
            {
                if (AvailableSwapTargets.Count == 0)
                {
                    RefreshSwapTargets();
                }
                InstanceControlPatches.SwitchTo(GetSwapTarget());
            }
            else
            {
                Cycle(increment);
                InstanceControlPatches.SwitchTo((byte)ControllingFigure);
            }
            
            PlayerControl.LocalPlayer.SetKillTimer(0f);
            foreach (var button in CustomButtonManager.Buttons.Where(x => x.Enabled(PlayerControl.LocalPlayer.Data.Role)))
            {
                button.SetTimer(0f);
            }
        }
    }

    public static List<PlayerControl> AvailableSwapTargets = new List<PlayerControl>();
    public static void RefreshSwapTargets()
    {
        AvailableSwapTargets.Clear();
        foreach (var players in PlayerControl.AllPlayerControls)
        {
            if (!AvailableSwapTargets.Contains(players))
            {
                AvailableSwapTargets.Add(players);
            }
        }
    }

    public static byte GetSwapTarget()
    {
        AvailableSwapTargets.Shuffle();
        var player = AvailableSwapTargets[0];
        AvailableSwapTargets.Remove(player);
        return player.PlayerId;
    }

    public static void Cycle(bool increment)
    {
        if (increment)
            ControllingFigure++;
        else
            ControllingFigure--;

        if (ControllingFigure < 0)
            ControllingFigure = InstanceControlPatches.Clients.Count - 1;
        else if (ControllingFigure > InstanceControlPatches.Clients.Count)
            ControllingFigure = 0;
    }
}
