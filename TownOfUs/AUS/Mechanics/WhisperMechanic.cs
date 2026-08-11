namespace AmongUsSalem.Mechanics;

[HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
public static class WhisperPatches
{
    public static void Prefix(ref string chatText, ref PlayerControl sourcePlayer)
    {
        if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started)
        {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                string command = string.Empty;
                if (chatText.Contains("/w " + player.Data.PlayerName)) command = "/w ";
                if (chatText.Contains("/whisper " + player.Data.PlayerName)) command = "/whisper ";

                // if (player.IsBlackmailed() && player.AmOwner) chatText = ""; - test for Banshee maybe.
                if (chatText.Contains(command + player.Data.PlayerName))
                {
                    if (player.HasDied() || sourcePlayer.HasDied())
                        break;

                    if (player.Data.PlayerName == PlayerControl.LocalPlayer.Data.PlayerName)
                    {
                        chatText = chatText.Replace(command + player.Data.PlayerName, "<color=#9a71e6>From " + sourcePlayer.Data.PlayerName + ": ");
                    }
                    else if (sourcePlayer == PlayerControl.LocalPlayer)
                    {
                        chatText = chatText.Replace(command + player.Data.PlayerName, "<color=#9a71e6>To " + player.Data.PlayerName + ":");
                    }
                    /*else if (PlayerControl.LocalPlayer.IsRole<Blackmailer>() && OptionGroupSingleton<Blackmailer_Options>.Instance.SeeWhispers)
                    {
                        chatText = chatText.Replace(command + player.Data.PlayerName, "<color=#DD0000>From " + sourcePlayer.Data.PlayerName + ": ");
                    }*/
                    else if (PlayerControl.LocalPlayer.IsRole<Wildling>())
                    {
                        chatText = chatText.Replace(command + player.Data.PlayerName, "<color=#DD0000>From " + sourcePlayer.Data.PlayerName + ": ");
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