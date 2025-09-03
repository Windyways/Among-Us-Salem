using System.Globalization;
using HarmonyLib;
using MiraAPI.GameOptions;
using Reactor.Utilities.Extensions;
using AmongUsSalem.Modules;
using AmongUsSalem.Options;
using AmongUsSalem.Patches.Options;
using AmongUsSalem.Roles.Crewmate;
using AmongUsSalem.Roles.Neutral;
using AmongUsSalem.Utilities;

namespace AmongUsSalem.Patches.Misc;

[HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
public static class ChatPatches
{
    // ReSharper disable once InconsistentNaming
    public static bool Prefix(ChatController __instance)
    {
        var text = __instance.freeChatField.Text.ToLower(CultureInfo.InvariantCulture);
        var textRegular = __instance.freeChatField.Text.WithoutRichText();

        if (textRegular.Length < 1 || textRegular.Length > 100)
        {
            return true;
        }

        if (text.Replace(" ", string.Empty).StartsWith("/", StringComparison.OrdinalIgnoreCase)
            && text.Replace(" ", string.Empty).Contains("summary", StringComparison.OrdinalIgnoreCase))
        {
            var title = "<color=#8BFDFD>System</color>";
            var msg = "No game summary to show!";
            if (GameHistory.EndGameSummary != string.Empty)
            {
                var factionText = string.Empty;
                if (GameHistory.WinningFaction != string.Empty)
                {
                    factionText = $"<size=80%>Winning Team: {GameHistory.WinningFaction}</size>\n";
                }

                title = $"<color=#8BFDFD>System</color>\n<size=62%>{factionText}{GameHistory.EndGameSummary}</size>";
                msg = string.Empty;
            }

            MiscUtils.AddFakeChat(PlayerControl.LocalPlayer.Data, title, msg);

            __instance.freeChatField.Clear();
            __instance.quickChatMenu.Clear();
            __instance.quickChatField.Clear();
            __instance.UpdateChatMode();
            return false;
        }

        if (text.Replace(" ", string.Empty).StartsWith("/help", StringComparison.OrdinalIgnoreCase))
        {
            var title = "<color=#8BFDFD>System</color>";

            var msg = "<size=75%>Chat Commands:\n" +
                      "/help - Shows this message\n" +
                      "/nerfme - Cuts your vision in half\n" +
                      "/summary - Shows the previous end game summary\n</size>";

            MiscUtils.AddFakeChat(PlayerControl.LocalPlayer.Data, title, msg);

            __instance.freeChatField.Clear();
            __instance.quickChatMenu.Clear();
            __instance.quickChatField.Clear();
            __instance.UpdateChatMode();
            return false;
        }

        if (TeamChatPatches.TeamChatActive && !PlayerControl.LocalPlayer.HasDied() &&
            (PlayerControl.LocalPlayer.Data.Role is JailorRole || PlayerControl.LocalPlayer.IsJailed() ||
             PlayerControl.LocalPlayer.Data.Role is VampireRole || PlayerControl.LocalPlayer.IsImpostor()))
        {
            if (PlayerControl.LocalPlayer.Data.Role is JailorRole)
            {
                TeamChatPatches.RpcSendJailorChat(PlayerControl.LocalPlayer, textRegular);
                MiscUtils.AddTeamChat(PlayerControl.LocalPlayer.Data,
                    $"<color=#{AUSColors.Jailor.ToHtmlStringRGBA()}>{PlayerControl.LocalPlayer.Data.PlayerName} (Jailor)</color>",
                    textRegular, onLeft: false);

                __instance.freeChatField.Clear();
                __instance.quickChatMenu.Clear();
                __instance.quickChatField.Clear();
                __instance.UpdateChatMode();

                return false;
            }

            if (PlayerControl.LocalPlayer.IsJailed())
            {
                TeamChatPatches.RpcSendJaileeChat(PlayerControl.LocalPlayer, textRegular);
                MiscUtils.AddTeamChat(PlayerControl.LocalPlayer.Data,
                    $"<color=#{AUSColors.Jailor.ToHtmlStringRGBA()}>{PlayerControl.LocalPlayer.Data.PlayerName} (Jailed)</color>",
                    textRegular, onLeft: false);

                __instance.freeChatField.Clear();
                __instance.quickChatMenu.Clear();
                __instance.quickChatField.Clear();
                __instance.UpdateChatMode();

                return false;
            }

            if (PlayerControl.LocalPlayer.Is(Faction.Mafia))
            {
                TeamChatPatches.RpcSendImpTeamChat(PlayerControl.LocalPlayer, textRegular);
                MiscUtils.AddTeamChat(PlayerControl.LocalPlayer.Data,
                    $"<color=#{AUSColors.Mafia.ToHtmlStringRGBA()}>{PlayerControl.LocalPlayer.Data.PlayerName} (Mafia Chat)</color>",
                    textRegular, onLeft: false);

                __instance.freeChatField.Clear();
                __instance.quickChatMenu.Clear();
                __instance.quickChatField.Clear();
                __instance.UpdateChatMode();

                return false;
            }

            return true;
        }

        return true;
    }
}