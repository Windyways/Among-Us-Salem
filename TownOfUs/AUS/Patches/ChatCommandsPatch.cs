using System.Globalization;
using HarmonyLib;
using Reactor.Utilities.Extensions;
using TownOfUs.Modules;

namespace AmongUsSalem.Patches;

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
                      "/roledeck - Shows the current role list.\n" +
                      "/summary - Shows the previous end game summary\n</size>";

            MiscUtils.AddFakeChat(PlayerControl.LocalPlayer.Data, title, msg);

            __instance.freeChatField.Clear();
            __instance.quickChatMenu.Clear();
            __instance.quickChatField.Clear();
            __instance.UpdateChatMode();
            return false;
        }

        if (text.Replace(" ", string.Empty).StartsWith("/roledeck", StringComparison.OrdinalIgnoreCase))
        {
            var list = RolelistMechanic.GetBuckets();
            var title = "<color=#8BFDFD>CURRENT ROLEDECK</color>";
            var msg =
                (GameData.Instance.PlayerCount >= 1 ? $"{list[0].ToSpacedString()}\n" : "") +
                (GameData.Instance.PlayerCount >= 2 ? $"{list[1].ToSpacedString()}\n" : "") +
                (GameData.Instance.PlayerCount >= 3 ? $"{list[2].ToSpacedString()}\n" : "") +
                (GameData.Instance.PlayerCount >= 4 ? $"{list[3].ToSpacedString()}\n" : "") +
                (GameData.Instance.PlayerCount >= 5 ? $"{list[4].ToSpacedString()}\n" : "") +
                (GameData.Instance.PlayerCount >= 6 ? $"{list[5].ToSpacedString()}\n" : "") +
                (GameData.Instance.PlayerCount >= 7 ? $"{list[6].ToSpacedString()}\n" : "") +
                (GameData.Instance.PlayerCount >= 8 ? $"{list[7].ToSpacedString()}\n" : "") +
                (GameData.Instance.PlayerCount >= 9 ? $"{list[8].ToSpacedString()}\n" : "") +
                (GameData.Instance.PlayerCount >= 10 ? $"{list[9].ToSpacedString()}\n" : "") +
                (GameData.Instance.PlayerCount >= 11 ? $"{list[10].ToSpacedString()}\n" : "") +
                (GameData.Instance.PlayerCount >= 12 ? $"{list[11].ToSpacedString()}\n" : "") +
                (GameData.Instance.PlayerCount >= 13 ? $"{list[12].ToSpacedString()}\n" : "") +
                (GameData.Instance.PlayerCount >= 14 ? $"{list[13].ToSpacedString()}\n" : "") +
                (GameData.Instance.PlayerCount >= 15 ? $"{list[14].ToSpacedString()}\n" : "");

            MiscUtils.AddFakeChat(PlayerControl.LocalPlayer.Data, title, "<size=65%>" + msg + "</size>");

            __instance.freeChatField.Clear();
            __instance.quickChatMenu.Clear();
            __instance.quickChatField.Clear();
            __instance.UpdateChatMode();
            return false;
        }

        if (AUSPlugin.InGame())
        {
            var player = PlayerControl.LocalPlayer;
            var host = GameData.Instance.GetHost();
            if (player.Data.Role is Jester jester) jester.OnMessageSend(__instance);

            if (player.TryGetModifier<JuryModifier>(out var jury))
            {
                string name = jury.isJudge ? "JUDGE" : "JURY";
                string newText = jury.isJudge ? "<color=#ffff00>" + text + "</color>" : text;
                // MiscUtils.AddFakeChat(host, name, newText);

                jury.AnonymousChatSendPatch(name, newText);
                __instance.freeChatField.Clear();
                __instance.quickChatMenu.Clear();
                __instance.quickChatField.Clear();
                __instance.UpdateChatMode();
                return false;
            }
        }
        return true;
    }
}