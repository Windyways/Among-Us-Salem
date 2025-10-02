using System.Collections;
using HarmonyLib;
using Reactor.Utilities;
using AmongUsSalem.Modules;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Patches;

[HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
public static class PlayerJoinPatch
{
    public static bool SentOnce { get; private set; }
    public static HudManager HUD => HudManager.Instance;

    public static void Postfix()
    {
        Coroutines.Start(CoSendJoinMsg());
    }

    private static IEnumerator CoSendJoinMsg()
    {
        while (!AmongUsClient.Instance)
        {
            yield return null;
        }

        Logger<AUSPlugin>.Info("Client Initialized?");

        while (!PlayerControl.LocalPlayer)
        {
            yield return null;
        }

        var player = PlayerControl.LocalPlayer;

        while (!player)
        {
            yield return null;
        }

        if (!player.AmOwner)
        {
            yield break;
        }

        Logger<AUSPlugin>.Info("Sending Message to Local Player...");
        TouRoleManagerPatches.ReplaceRoleManager = false;

        var time = 0f;
        if (GameHistory.EndGameSummary != string.Empty)
        {
            var factionText = string.Empty;
            var msg = string.Empty;
            if (GameHistory.WinningFaction != string.Empty)
            {
                factionText = $"<size=80%>Winning Team: {GameHistory.WinningFaction}</size>\n";
            }

            var title =
                $"<color=#8BFDFD>System (Toggleable In Options)</color>\n<size=62%>{factionText}{GameHistory.EndGameSummary}</size>";
            MiscUtils.AddFakeChat(PlayerControl.LocalPlayer.Data, title, msg);
        }

        if (time == 0)
        {
            yield break;
        }

        yield return new WaitForSeconds(time);
        Logger<AUSPlugin>.Info("Offset Wiki Button (if needed)");
        SentOnce = true;
    }
}