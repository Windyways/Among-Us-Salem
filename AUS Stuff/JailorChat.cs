using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AmongUs.Data;
using AmongUs.QuickChat;
using Assets.CoreScripts;
using HarmonyLib;
using Reactor.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUsEdited.Patches.CrewmateRoles.JailorMod
{
    public class JailorChat
    {
        public static ChatController JailorChatButton;
        public static Transform Background;
        public static List<(PlayerControl, string, bool)> Messages = new List<(PlayerControl, string, bool)>();
        public static void UpdateJailorChat()
        {
            if (JailorChatButton && PlayerControl.LocalPlayer.Data.IsDead)
            {
                if (JailorChatButton.IsOpenOrOpening) ControllerManager.Instance.CloseOverlayMenu(JailorChatButton.name);
                Object.Destroy(JailorChatButton.gameObject);
                Object.Destroy(Background.gameObject);
            }
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Jailor) && !PlayerControl.LocalPlayer.IsJailed())
            {
                if (JailorChatButton)
                {
                    if (JailorChatButton.IsOpenOrOpening) ControllerManager.Instance.CloseOverlayMenu(JailorChatButton.name);
                    Object.Destroy(JailorChatButton.gameObject);
                    Object.Destroy(Background.gameObject);
                }
                return;
            }
            if (CustomGameOptions.GameMode == GameMode.Chaos) return;
            if (AmongUsClient.Instance.NetworkMode == NetworkModes.FreePlay) return;
            List<PlayerControl> JailorTeam = PlayerControl.AllPlayerControls.ToArray().Where(x => (x.Is(RoleEnum.Jailor) || x.IsJailed()) && !x.Data.IsDead && !x.Data.Disconnected).ToList();
            if (JailorTeam.Count < 2)
            {
                if (JailorChatButton)
                {
                    if (JailorChatButton.IsOpenOrOpening) ControllerManager.Instance.CloseOverlayMenu(JailorChatButton.name);
                    Object.Destroy(JailorChatButton.gameObject);
                    Object.Destroy(Background.gameObject);
                }
                return;
            }
            if (!JailorChatButton)
            {
                JailorChatButton = Object.Instantiate(HudManager.Instance.Chat, HudManager.Instance.Chat.transform.parent);
                JailorChatButton.name = "JailorChat";
                foreach (var bubble in JailorChatButton.chatBubblePool.activeChildren)
                {
                    Object.Destroy(bubble.gameObject);
                }
                JailorChatButton.chatBubblePool.activeChildren.Clear();
                JailorChatButton.chatButton.transform.Find("Inactive").GetComponent<SpriteRenderer>().color = Colors.Jailor;
                JailorChatButton.chatButton.transform.Find("Active").GetComponent<SpriteRenderer>().color = Colors.Jailor;
                JailorChatButton.chatButton.transform.Find("Selected").GetComponent<SpriteRenderer>().color = Colors.Jailor;
                var container = JailorChatButton.chatScreen.transform.Find("ChatScreenContainer");
                container.transform.FindChild("Background").GetComponent<SpriteRenderer>().color = Colors.Jailor;
            }
            if (!Background)
            {
                Background = Object.Instantiate(HudManager.Instance.SettingsButton.transform.GetChild(2), HudManager.Instance.SettingsButton.transform.GetChild(2).transform.parent);
                Background.name = "JailorChatBackground";
                Background.transform.localPosition = new Vector3(0.717f, -0.631f, 1f);
            }
            JailorChatButton.gameObject.SetActive(true);
            Background.gameObject.SetActive(true);
            JailorChatButton.transform.localPosition = new Vector3(1.4386f, -0.7827f, 0f);
            JailorChatButton.chatButton.transform.GetChild(3).gameObject.SetActive(false);
        }

        [HarmonyPatch(typeof(ChatController), nameof(ChatController.Toggle))]
        public class ToggleChat
        {
            public static bool Prefix(ChatController __instance)
            {
                if (JailorChatButton == null) return true;
                if (!JailorChatButton.isActiveAndEnabled) return true;
                if (JailorChatButton.IsOpenOrOpening && __instance != JailorChatButton) return false;
                if (__instance == JailorChatButton && !JailorChatButton.IsOpenOrOpening) //Open chat
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
				if (JailorChatButton == null) return true;
                if (!JailorChatButton.isActiveAndEnabled) return true;
                if (!JailorChatButton.IsOpenOrOpening) return true;
                if (!__instance.AmOwner) return true;
                chatText = Regex.Replace(chatText, "<.*?>", string.Empty);
                if (string.IsNullOrWhiteSpace(chatText))
                {
                    return false;
                }
                if (DestroyableSingleton<HudManager>.Instance)
                {
                    JailorChatButton.AddChat(__instance, chatText, true);
                }
                if (chatText.IndexOf("who", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    DestroyableSingleton<UnityTelemetry>.Instance.SendWho();
                }
                Utils.Rpc(CustomRPC.SendCustomChat, __instance.PlayerId, chatText, "JailorChat");
                return false;
			}
		}

        [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
		public static class AddChat
		{
			public static bool Prefix(ChatController __instance, [HarmonyArgument(0)] PlayerControl srcPlayer, [HarmonyArgument(1)] string chatText, [HarmonyArgument(2)] bool censor)
            {
                if (JailorChatButton == null) return true;
                if (!JailorChatButton.isActiveAndEnabled) return true;
                if (__instance != JailorChatButton) return true;
                if (JailorChatButton.IsOpenOrOpening) return true;
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
                if (JailorChatButton == null) return true;
                if (!JailorChatButton.isActiveAndEnabled) return true;
                if (!JailorChatButton.IsOpenOrOpening) return true;
                if (!__instance.AmOwner) return true;
                string text = data.ToChatText();
                if (string.IsNullOrWhiteSpace(text) || data == null || !data.IsValid())
                {
                    return false;
                }
                if (DestroyableSingleton<HudManager>.Instance)
                {
                    JailorChatButton.AddChat(__instance, text, false);
                }
                if (data.ToChatText().IndexOf("who", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    DestroyableSingleton<UnityTelemetry>.Instance.SendWho();
                }
                Utils.Rpc(CustomRPC.SendCustomChat, __instance.PlayerId, text, "JailorChat");
                return false;
            }
		}

        [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetText))]
        public class ChatColor
        {
            public static void Postfix(ChatBubble __instance)
            {
                if (LobbyBehaviour.Instance) return;
                if ((JailorChatButton != null && JailorChatButton.isActiveAndEnabled && JailorChatButton.chatBubblePool.activeChildren.Contains(__instance)) || (__instance.TextArea.text.Contains("[Jailor Chat]") && PlayerControl.LocalPlayer.Data.IsDead))
                {
                    __instance.Background.color = Colors.Jailor;
                    __instance.NameText.color = Colors.Jailor;
                    var srcPlayer = Utils.PlayerByData(__instance.playerInfo);
                    if (srcPlayer == PlayerControl.LocalPlayer) return;
                    if (srcPlayer.Is(RoleEnum.Jailor))
                    {
                        __instance.NameText.text = "Jailor";
                    }
                    else __instance.NameText.text = "Jailed";
                }
            }
        }

        [HarmonyPatch(typeof(PoolablePlayer), nameof(PoolablePlayer.UpdateFromPlayerOutfit))]
        public class JailorOutfit
        {
            public static void Prefix(PoolablePlayer __instance, ref NetworkedPlayerInfo.PlayerOutfit outfit)
            {
                if (LobbyBehaviour.Instance) return;
                if (PlayerControl.LocalPlayer.IsJailed() && JailorChatButton != null)
                {
                    var jailor = PlayerControl.LocalPlayer.GetJailor();
                    if (jailor == null) return;
                    if (JailorChatButton.chatBubblePool.activeChildren.ToArray().Where(x => x.Cast<ChatBubble>().Player == __instance).ToList().Count <= 0) return;
                    var jailorOutfit = jailor.Player.Data.Outfits[PlayerOutfitType.Default];
                    if (jailorOutfit != outfit) return;
                    outfit = new NetworkedPlayerInfo.PlayerOutfit()
                    {
                        ColorId = 14,
                        HatId = "",
                        SkinId = "",
                        VisorId = "",
                        PlayerName = "Jailor",
                        PetId = ""
                    };
                }
            }
        }
    }
}