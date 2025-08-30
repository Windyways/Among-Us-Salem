using System.Collections;
using HarmonyLib;
using Reactor.Utilities;
using ObjectWorkshop.Modules;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Patches;

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

        Logger<ObjectWorkshopPlugin>.Info("Client Initialized?");

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

        Logger<ObjectWorkshopPlugin>.Info("Sending Message to Local Player...");
        TouRoleManagerPatches.ReplaceRoleManager = false;

        var time = 0f;
        if (GameHistory.EndGameSummary != string.Empty && ObjectWorkshopPlugin.ShowSummaryMessage.Value)
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

        if (!SentOnce && ObjectWorkshopPlugin.ShowWelcomeMessage.Value)
        {
            var name = "<color=#8BFDFD>System</color>";
            var msg =
                $"Welcome to Object Workshop v{ObjectWorkshopPlugin.Version}!\nUse the wiki (the globe icon) to get more info on roles or modifiers, where you can use the searchbar. Otherwise use /help in the chat to get a list of commands.\nYou can also disable this message through your options menu.";
            MiscUtils.AddFakeChat(PlayerControl.LocalPlayer.Data, name, msg, true);
            time = 5f;
        }
        else if (!ObjectWorkshopPlugin.ShowWelcomeMessage.Value)
        {
            time = 2.48f;
        }

        if (time == 0)
        {
            yield break;
        }

        yield return new WaitForSeconds(time);
        Logger<ObjectWorkshopPlugin>.Info("Offset Wiki Button (if needed)");
        SentOnce = true;
    }
}