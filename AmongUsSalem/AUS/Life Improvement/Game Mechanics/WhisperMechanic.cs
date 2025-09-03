using System.Collections.Generic;
using System.IO;
using System.Linq;
using AmongUsSalem.Patches;
using AmongUsSalem.Roles;
using HarmonyLib;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement;

[HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
public static class WhisperPatches
{
    public static void Prefix(ref string chatText, ref PlayerControl sourcePlayer)
    {
        if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started)
        {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                // if (player.IsBlackmailed() && player.AmOwner) chatText = ""; - test for Banshee maybe.
                if (chatText.Contains("/w " + player.Data.PlayerName))
                {
                    if (player.HasDied() || sourcePlayer.HasDied())
                    {
                        break;
                    }

                    if (player.Data.PlayerName == PlayerControl.LocalPlayer.Data.PlayerName)
                    {
                        chatText = chatText.Replace("/w " + player.Data.PlayerName, "<color=#9a71e6>From " + sourcePlayer.Data.PlayerName + ": ");
                    }
                    else if (sourcePlayer == PlayerControl.LocalPlayer)
                    {
                        chatText = chatText.Replace("/w " + player.Data.PlayerName, "<color=#9a71e6>To " + player.Data.PlayerName + ":");
                    }
                    else if (PlayerControl.LocalPlayer.IsRole<Blackmailer>() && OptionGroupSingleton<Blackmailer_Options>.Instance.SeeWhispers)
                    {
                        chatText = chatText.Replace("/w " + player.Data.PlayerName, "<color=#DD0000>From " + sourcePlayer.Data.PlayerName + ": ");
                    }
                    else
                    {
                        chatText = "<color=#9a71e6>" + sourcePlayer.Data.PlayerName + " is whispering to " + player.Data.PlayerName + "</color>";
                    }
                }
            }
        }
    }
}