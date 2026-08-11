using AmongUs.Data;
using AmongUs.QuickChat;
using Assets.CoreScripts;
using Reactor.Networking.Rpc;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AmongUsSalem.ApocalypseRoles;

public class ApocalypseNightChat
{
    [RegisterEvent]
    public static void EjectionEvent(EjectionEvent @event)
    {
        UpdateApocalypseChat(true);
    }

    [RegisterEvent]
    public static void RoundStartEvent(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
            return;

        UpdateApocalypseChat();
    }

    [RegisterEvent]
    public static void StartMeetingEvent(StartMeetingEvent @event)
    {
        UpdateApocalypseChat(true);
    }

    [RegisterEvent]
    public static void AfterMurderEvent(AfterMurderEvent @event)
    {
        if (@event.Target.AmOwner) UpdateApocalypseChat(true);
    }

    public static ChatController ApocalypseChatButton;
    public static Transform Background;
    public static List<(PlayerControl, string, bool)> Messages = new List<(PlayerControl, string, bool)>();
    public static void UpdateApocalypseChat(bool disable = false)
    {
        if (ApocalypseChatButton && (disable || PlayerControl.LocalPlayer.HasDied()))
        {
            if (ApocalypseChatButton.IsOpenOrOpening) ControllerManager.Instance.CloseOverlayMenu(ApocalypseChatButton.name);
            Object.Destroy(ApocalypseChatButton.gameObject);
            Object.Destroy(Background.gameObject);
        }

        if (PlayerControl.LocalPlayer.HasDied() || disable) 
            return;

        if (!PlayerControl.LocalPlayer.Is(Faction.Apocalypse))
        {
            if (ApocalypseChatButton)
            {
                if (ApocalypseChatButton.IsOpenOrOpening) ControllerManager.Instance.CloseOverlayMenu(ApocalypseChatButton.name);
                Object.Destroy(ApocalypseChatButton.gameObject);
                Object.Destroy(Background.gameObject);
            }
            return;
        }

        if (AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay)
            return;

        if (!ApocalypseChatButton)
        {
            ApocalypseChatButton = Object.Instantiate(HudManager.Instance.Chat, HudManager.Instance.Chat.transform.parent);
            ApocalypseChatButton.name = "ApocalypseChat";
            foreach (var bubble in ApocalypseChatButton.chatBubblePool.activeChildren)
            {
                Object.Destroy(bubble.gameObject);
            }
            ApocalypseChatButton.chatBubblePool.activeChildren.Clear();
            ApocalypseChatButton.chatButton.transform.Find("Inactive").GetComponent<SpriteRenderer>().color = RoleColors.Apocalypse;
            ApocalypseChatButton.chatButton.transform.Find("Active").GetComponent<SpriteRenderer>().color = RoleColors.Apocalypse;
            ApocalypseChatButton.chatButton.transform.Find("Selected").GetComponent<SpriteRenderer>().color = RoleColors.Apocalypse;
            var container = ApocalypseChatButton.chatScreen.transform.Find("ChatScreenContainer");
            container.transform.FindChild("Background").GetComponent<SpriteRenderer>().color = RoleColors.Apocalypse;
        }
        if (!Background)
        {
            Background = Object.Instantiate(HudManager.Instance.SettingsButton.transform.GetChild(2), HudManager.Instance.SettingsButton.transform.GetChild(2).transform.parent);
            Background.name = "ApocalypseChatBackground";
            Background.transform.localPosition = new Vector3(0.717f, -0.631f, 1f);
        }
        ApocalypseChatButton.gameObject.SetActive(true);
        Background.gameObject.SetActive(true);
        ApocalypseChatButton.transform.localPosition = new Vector3(1.4386f, -0.7827f, 0f);
        ApocalypseChatButton.chatButton.transform.GetChild(3).gameObject.SetActive(false);
    }

    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Toggle))]
    public class ToggleChat
    {
        public static bool Prefix(ChatController __instance)
        {
            if (ApocalypseChatButton == null) return true;
            if (!ApocalypseChatButton.isActiveAndEnabled) return true;
            if (ApocalypseChatButton.IsOpenOrOpening && __instance != ApocalypseChatButton) return false;
            if (__instance == ApocalypseChatButton && !ApocalypseChatButton.IsOpenOrOpening) //Open chat
            {
                Coroutines.Start(WaitForSend(__instance));
            }
            return true;
        }

        public static IEnumerator WaitForSend(ChatController __instance)
        {
            yield return new WaitForSeconds(0.1f);
            while (Messages.Count > 0)
            {
                var message = Messages[0];
                ForceAddChat(__instance, message.Item1, message.Item2, message.Item3);
                Messages.Remove(message);
                yield return null;
            }
            yield break;
        }

        public static void ForceAddChat(ChatController __instance, PlayerControl srcPlayer, string chatText, bool censor)
        {
            NetworkedPlayerInfo data = PlayerControl.LocalPlayer.Data;
            NetworkedPlayerInfo data2 = srcPlayer.Data;
            if (data2 == null || data == null || (data2.IsDead && !data.IsDead))
            {
                return;
            }
            ChatBubble pooledBubble = __instance.GetPooledBubble();
            try
            {
                pooledBubble.transform.SetParent(__instance.scroller.Inner);
                pooledBubble.transform.localScale = Vector3.one;
                bool flag = srcPlayer == PlayerControl.LocalPlayer;
                if (flag)
                {
                    pooledBubble.SetRight();
                }
                else
                {
                    pooledBubble.SetLeft();
                }
                bool didVote = MeetingHud.Instance && MeetingHud.Instance.DidVote(srcPlayer.PlayerId);
                pooledBubble.SetCosmetics(data2);
                __instance.SetChatBubbleName(pooledBubble, data2, data2.IsDead, didVote, PlayerNameColor.Get(data2), null);
                if (censor && DataManager.Settings.Multiplayer.CensorChat)
                {
                    chatText = BlockedWords.CensorWords(chatText, false);
                }
                pooledBubble.SetText(chatText);
                pooledBubble.AlignChildren();
                __instance.AlignAllBubbles();
            }
            catch (Exception message)
            {
                ChatController.Logger.Error(message.ToString(), null);
                __instance.chatBubblePool.Reclaim(pooledBubble);
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSendChat))]
    public static class SendChat
    {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] ref string chatText)
        {
            if (ApocalypseChatButton == null) return true;
            if (!ApocalypseChatButton.isActiveAndEnabled) return true;
            if (!ApocalypseChatButton.IsOpenOrOpening) return true;
            if (!__instance.AmOwner) return true;
            chatText = Regex.Replace(chatText, "<.*?>", string.Empty);
            if (string.IsNullOrWhiteSpace(chatText))
            {
                return false;
            }
            if (DestroyableSingleton<HudManager>.Instance)
            {
                ApocalypseChatButton.AddChat(__instance, chatText, true);
            }
            if (chatText.IndexOf("who", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                DestroyableSingleton<UnityTelemetry>.Instance.SendWho();
            }

            RpcSendCustomChat(__instance, chatText, "ApocalypseChat");
            return false;
        }

        [MethodRpc((uint)AUSRpc.RpcSendCustomChat)]
        public static void RpcSendCustomChat(PlayerControl player, string chatText, string chatType)
        {
            if (chatType == "ApocalypseChat")
            {
                if (!PlayerControl.LocalPlayer.HasDied() && ApocalypseNightChat.ApocalypseChatButton != null &&
                    player != PlayerControl.LocalPlayer) ApocalypseNightChat.ApocalypseChatButton.AddChat(player, chatText, false);
                /*else if (PlayerControl.LocalPlayer.Data.IsDead && Utils.ShowDeadBodies)
                {
                    var text2 = "[Apocalypse Chat]\n" + chatText;
                    HudManager.Instance.Chat.AddChat(player, text2, false);
                }*/
            }
        }
    }

    [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
    public static class AddChat
    {
        public static bool Prefix(ChatController __instance, [HarmonyArgument(0)] PlayerControl srcPlayer, [HarmonyArgument(1)] string chatText, [HarmonyArgument(2)] bool censor)
        {
            if (ApocalypseChatButton == null) return true;
            if (!ApocalypseChatButton.isActiveAndEnabled) return true;
            if (__instance != ApocalypseChatButton) return true;
            if (ApocalypseChatButton.IsOpenOrOpening) return true;
            Messages.Add((srcPlayer, chatText, censor)); // Avoids weird SetFlipXWithoutPet error, really sketchy fix, but couldn't find any better way :sob:
            var flag = srcPlayer == PlayerControl.LocalPlayer;
            if (__instance.notificationRoutine == null)
            {
                __instance.notificationRoutine = __instance.StartCoroutine(__instance.BounceDot());
            }
            if (!flag)
            {
                SoundManager.Instance.PlaySound(__instance.messageSound, false, 1f, null).pitch = 0.5f + (float)srcPlayer.PlayerId / 15f;
                __instance.chatNotification.SetUp(srcPlayer, chatText);
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSendQuickChat))]
    public static class SendQuickChat
    {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] QuickChatPhraseBuilderResult data)
        {
            if (ApocalypseChatButton == null) return true;
            if (!ApocalypseChatButton.isActiveAndEnabled) return true;
            if (!ApocalypseChatButton.IsOpenOrOpening) return true;
            if (!__instance.AmOwner) return true;
            string text = data.ToChatText();
            if (string.IsNullOrWhiteSpace(text) || data == null || !data.IsValid())
            {
                return false;
            }
            if (DestroyableSingleton<HudManager>.Instance)
            {
                ApocalypseChatButton.AddChat(__instance, text, false);
            }
            if (data.ToChatText().IndexOf("who", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                DestroyableSingleton<UnityTelemetry>.Instance.SendWho();
            }

            ApocalypseNightChat.SendChat.RpcSendCustomChat(__instance, text, "ApocalypseChat");
            return false;
        }
    }

    [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetText))]
    public class ChatColor
    {
        public static void Postfix(ChatBubble __instance)
        {
            if (LobbyBehaviour.Instance) return;
            if ((ApocalypseChatButton != null && ApocalypseChatButton.isActiveAndEnabled && ApocalypseChatButton.chatBubblePool.activeChildren.Contains(__instance)) || (__instance.TextArea.text.Contains("[Apocalypse Chat]") && PlayerControl.LocalPlayer.Data.IsDead))
            {
                __instance.Background.color = RoleColors.Apocalypse;
                __instance.NameText.color = RoleColors.Apocalypse;
                var srcPlayer = MiscUtils.PlayerById(__instance.playerInfo.PlayerId);
                if (srcPlayer == PlayerControl.LocalPlayer) return;
                __instance.NameText.text = srcPlayer.Data.PlayerName;
            }
        }
    }

    /*[HarmonyPatch(typeof(PoolablePlayer), nameof(PoolablePlayer.UpdateFromPlayerOutfit))]
    public class ApocalypseOutfit
    {
        public static void Prefix(PoolablePlayer __instance, ref NetworkedPlayerInfo.PlayerOutfit outfit)
        {
            if (LobbyBehaviour.Instance) return;
            if (PlayerControl.LocalPlayer.IsJailed() && ApocalypseChatButton != null)
            {
                var apocalypse = PlayerControl.LocalPlayer.GetApocalypse();
                if (apocalypse == null) return;
                if (ApocalypseChatButton.chatBubblePool.activeChildren.ToArray().Where(x => x.Cast<ChatBubble>().Player == __instance).ToList().Count <= 0) return;
                var apocalypseOutfit = apocalypse.Player.Data.Outfits[PlayerOutfitType.Default];
                if (apocalypseOutfit != outfit) return;
                outfit = new NetworkedPlayerInfo.PlayerOutfit()
                {
                    ColorId = 14,
                    HatId = "",
                    SkinId = "",
                    VisorId = "",
                    PlayerName = "Apocalypse",
                    PetId = ""
                };
            }
        }
    }*/
}