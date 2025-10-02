using AmongUsSalem.Modifiers;
using BepInEx.Unity.IL2CPP.Utils;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AmongUsSalem.LifeImprovement.MCI.SmartMCI;

public static class CycleMode
{
    public static int PlayersCycled;
    public static void Start()
    {
        PlayersCycled = 0;
        Coroutines.Start(Sequence());
    }

    public static IEnumerator Sequence()
    {
        if (MeetingHud.Instance)
            yield break;

        yield return new WaitForSeconds(0.25f);

        Keyboard_Joystick.Cycle(true);

        var newPlayer = MiscUtils.PlayerById((byte)Keyboard_Joystick.ControllingFigure);
        InstanceControlPatches.SwitchTo((byte)Keyboard_Joystick.ControllingFigure);

        yield return new WaitForSeconds(0.25f);

        var availableButtons = CustomButtonManager.Buttons
            .Where(x => x.Enabled(newPlayer.Data.Role))
            .ToList();

        var button = availableButtons.Count > 0 
            ? availableButtons[UnityEngine.Random.Range(0, availableButtons.Count)] 
            : null;

        if (button != null && !newPlayer.HasDied())
        {
            PlayerControl.LocalPlayer.SetKillTimer(0f);
            button.SetTimer(0f);

            // -------------------------
            // Find next living player (walk through dead ones)
            // -------------------------
            var controllingId = (byte)Keyboard_Joystick.ControllingFigure;
            var nextId = (byte)(controllingId + 1);
            PlayerControl? finalTarget = null;

            for (int i = 0; i < PlayerControl.AllPlayerControls.Count; i++)
            {
                var candidate = MiscUtils.PlayerById(nextId);
                if (candidate == null) break;

                if (candidate.HasDied() || candidate.IsHost())
                {
                    // Move to each dead player's position along the way
                    newPlayer.transform.position = candidate.transform.position;
                    nextId++;
                }
                else
                {
                    finalTarget = candidate;
                    break;
                }
            }

            // If we found a living target, use ability
            if (finalTarget != null)
            {
                button.ClickHandler();
            }
        }

        if (newPlayer.HasDied() && newPlayer.TryGetModifier<DeathHandlerModifier>(out var deathMod) && deathMod.DiedThisRound)
        {
            yield return new WaitForSeconds(2.5f);
        }
        else
        {
            yield return new WaitForSeconds(0.25f);
        }

        if (PlayersCycled < 14)
        {
            PlayersCycled++;
            Coroutines.Start(Sequence());
        }
    }

    /*public static int DeadPlayers;
    public static bool SomeoneDied()
    {
        var deadPlayers = PlayerControl.AllPlayerControls.ToArray().Count(x => x.HasDied());
        if (DeadPlayers < deadPlayers)
        {
            DeadPlayers = deadPlayers;
            return true;
        }
        return false;
    }*/
}