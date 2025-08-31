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


        if (text.Replace(" ", string.Empty).StartsWith("/setname", StringComparison.OrdinalIgnoreCase))
        {
            var title = "<color=#8BFDFD>System</color>";
            if (text.StartsWith("/setname ", StringComparison.OrdinalIgnoreCase))
            {
                textRegular = textRegular[9..];
            }
            else if (text.StartsWith("/setname", StringComparison.OrdinalIgnoreCase))
            {
                textRegular = textRegular[8..];
            }
            else if (text.StartsWith("/ setname ", StringComparison.OrdinalIgnoreCase))
            {
                textRegular = textRegular[10..];
            }
            else if (text.StartsWith("/ setname", StringComparison.OrdinalIgnoreCase))
            {
                textRegular = textRegular[9..];
            }

            var msg = "You cannot change your name outside of the lobby!";
            if (LobbyBehaviour.Instance)
            {
                if (textRegular.Length < 1 || textRegular.Length > 12)
                {
                    msg =
                        "The player name must be at least 1 character long, and cannot be more than 12 characters long!";
                }
                else if (PlayerControl.AllPlayerControls.ToArray().Any(x => x.Data.PlayerName.ToLower(CultureInfo.InvariantCulture).Trim() == textRegular.ToLower(CultureInfo.InvariantCulture).Trim() && x.Data.PlayerId != PlayerControl.LocalPlayer.PlayerId))
                {
                    msg = $"Another player has a name too similar to {textRegular}! Please try a different name.";
                }
                else
                {
                    PlayerControl.LocalPlayer.CmdCheckName(textRegular);
                    msg = $"Changed player name for the next match to: {textRegular}";
                }
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

            List<string> randomNames =
            [
                "Atony", "Alchlc", "angxlwtf", "Digi", "Donners", "K3ndo", "DragonBreath", "Pietro",
                "twix", "xerm", "XtraCube", "Zeo", "Slushie", "chloe", "moon", "decii", "Northie", "GD", "Chilled",
                "Himi", "Riki", "Leafly", "miniduikboot"
            ];

            var msg = "<size=75%>Chat Commands:\n" +
                      "/help - Shows this message\n" +
                      "/nerfme - Cuts your vision in half\n" +
                      $"/setname - Change your name to whatever text follows the command (like /setname {randomNames.Random()}) for the next match.\n" +
                      "/summary - Shows the previous end game summary\n</size>";

            MiscUtils.AddFakeChat(PlayerControl.LocalPlayer.Data, title, msg);

            __instance.freeChatField.Clear();
            __instance.quickChatMenu.Clear();
            __instance.quickChatField.Clear();
            __instance.UpdateChatMode();
            return false;
        }

        if (text.Replace(" ", string.Empty).StartsWith("/jail", StringComparison.OrdinalIgnoreCase))
        {
            var title = "<color=#8BFDFD>System</color>";

            MiscUtils.AddFakeChat(PlayerControl.LocalPlayer.Data, title,
                "The mod no longer supports /jail chat. Use the red in-game chat button instead.");

            __instance.freeChatField.Clear();
            __instance.quickChatMenu.Clear();
            __instance.quickChatField.Clear();
            __instance.UpdateChatMode();
            return false;
        }

        if (text.Replace(" ", string.Empty).StartsWith("/lb", StringComparison.OrdinalIgnoreCase))
        {

        }
        else if (text.Replace(" ", string.Empty).StartsWith("/resetlb", StringComparison.OrdinalIgnoreCase))
        {
            
        }
        else if (text.Replace(" ", string.Empty).StartsWith("/", StringComparison.OrdinalIgnoreCase))
        {
            var title = "<color=#8BFDFD>System</color>";

            MiscUtils.AddFakeChat(PlayerControl.LocalPlayer.Data, title,
                "Invalid command. If you need information on chat commands, type /help. If you are trying to know what a role or modifier does, check out the in-game wiki by pressing the globe icon on the top right of your screen.");

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
            var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
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

            /*if (PlayerControl.LocalPlayer.Data.Role is VampireRole && genOpt.VampireChat)
            {
                TeamChatPatches.RpcSendVampTeamChat(PlayerControl.LocalPlayer, textRegular);
                MiscUtils.AddTeamChat(PlayerControl.LocalPlayer.Data,
                    $"<color=#{AUSColors.Vampire.ToHtmlStringRGBA()}>{PlayerControl.LocalPlayer.Data.PlayerName} (Vampire Chat)</color>",
                    textRegular, onLeft: false);

                __instance.freeChatField.Clear();
                __instance.quickChatMenu.Clear();
                __instance.quickChatField.Clear();
                __instance.UpdateChatMode();

                return false;
            }*/

            if (PlayerControl.LocalPlayer.IsImpostor() &&
                genOpt is { FFAImpostorMode: false, ImpostorChat.Value: true })
            {
                TeamChatPatches.RpcSendImpTeamChat(PlayerControl.LocalPlayer, textRegular);
                MiscUtils.AddTeamChat(PlayerControl.LocalPlayer.Data,
                    $"<color=#{AUSColors.Mafia.ToHtmlStringRGBA()}>{PlayerControl.LocalPlayer.Data.PlayerName} (Impostor Chat)</color>",
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