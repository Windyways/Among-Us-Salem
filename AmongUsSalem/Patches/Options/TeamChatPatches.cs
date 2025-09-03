using HarmonyLib;
using InnerNet;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Utilities.Extensions;
using TMPro;
using AmongUsSalem.Modifiers.Crewmate;
using AmongUsSalem.Modules;
using AmongUsSalem.Options;
using AmongUsSalem.Roles.Crewmate;
using AmongUsSalem.Roles.Neutral;
using AmongUsSalem.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AmongUsSalem.Patches.Options;

public static class TeamChatPatches
{
    private static TextMeshPro? _teamText;
    public static bool TeamChatActive;

    public static void ToggleTeamChat()
    {
        // WIP
        TeamChatActive = !TeamChatActive;
        if (!TeamChatActive)
        {
            HudManagerPatches.TeamChatButton.transform.Find("Inactive").gameObject.SetActive(true);
        }
        else
        {
            HudManager.Instance.Chat.Toggle();
        }
    }

    [MethodRpc((uint)AUSRpc.SendJailorChat, SendImmediately = true)]
    public static void RpcSendJailorChat(PlayerControl player, string text)
    {
        if (PlayerControl.LocalPlayer.IsJailed())
        {
            MiscUtils.AddTeamChat(PlayerControl.LocalPlayer.Data,
                $"<color=#{AUSColors.Jailor.ToHtmlStringRGBA()}>Jailor</color>", text);
        }
        else if (PlayerControl.LocalPlayer.HasDied() && OptionGroupSingleton<GeneralOptions>.Instance.TheDeadKnow)
        {
            MiscUtils.AddTeamChat(player.Data,
                $"<color=#{AUSColors.Jailor.ToHtmlStringRGBA()}>{player.Data.PlayerName} (Jailor)</color>", text);
        }
    }

    [MethodRpc((uint)AUSRpc.SendJaileeChat, SendImmediately = true)]
    public static void RpcSendJaileeChat(PlayerControl player, string text)
    {
        if (PlayerControl.LocalPlayer.Data.Role is JailorRole || (PlayerControl.LocalPlayer.HasDied() &&
                                                                  OptionGroupSingleton<GeneralOptions>.Instance
                                                                      .TheDeadKnow))
        {
            MiscUtils.AddTeamChat(player.Data,
                $"<color=#{AUSColors.Jailor.ToHtmlStringRGBA()}>{player.Data.PlayerName} (Jailed)</color>", text);
        }
    }

    [MethodRpc((uint)AUSRpc.SendImpTeamChat, SendImmediately = true)]
    public static void RpcSendImpTeamChat(PlayerControl player, string text)
    {
        if ((PlayerControl.LocalPlayer.Is(Faction.Mafia) && player != PlayerControl.LocalPlayer) ||
            (PlayerControl.LocalPlayer.HasDied() && OptionGroupSingleton<GeneralOptions>.Instance.TheDeadKnow))
        {
            MiscUtils.AddTeamChat(player.Data,
                $"<color=#{AUSColors.Mafia.ToHtmlStringRGBA()}>{player.Data.PlayerName} (Mafia Chat)</color>",
                text);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetName))]
    public static void SetNamePostfix(ChatBubble __instance, [HarmonyArgument(0)] string playerName, [HarmonyArgument(3)] Color color)
    {
        var player = PlayerControl.AllPlayerControls.ToArray()
            .FirstOrDefault(x => x.Data.PlayerName == playerName);
        if (player == null) return;
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
        if (color == Color.white &&
                 (player.AmOwner || player.Data.Role is MayorRole mayor && mayor.Revealed ||
                  PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow) && PlayerControl.AllPlayerControls
                     .ToArray()
                     .FirstOrDefault(x => x.Data.PlayerName == playerName) && MeetingHud.Instance)
        {
            __instance.NameText.color = (player.GetRoleWhenAlive() is ICustomRole custom) ? custom.RoleColor : player.GetRoleWhenAlive().TeamColor;
        }
    }

    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Toggle))]
    public static class TogglePatch
    {
        public static void Postfix(ChatController __instance)
        {
            if (PlayerControl.LocalPlayer == null ||
                PlayerControl.LocalPlayer.Data == null ||
                PlayerControl.LocalPlayer.Data.Role == null ||
                !ShipStatus.Instance ||
                (AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started &&
                 !TutorialManager.InstanceExists))
            {
                return;
            }
            try
            {
                if (__instance.IsOpenOrOpening)
                {
                    if (_teamText == null)
                    {
                        _teamText = Object.Instantiate(__instance.sendRateMessageText,
                            __instance.sendRateMessageText.transform.parent);
                        _teamText.text = string.Empty;
                        _teamText.color = AUSColors.Mafia;
                    }

                    var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
                    _teamText.text = string.Empty;
                    if (PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow &&
                        Helpers.GetAlivePlayers().Any(x => x.Data.Role is JailorRole))
                    {
                        _teamText.text = "Jailor, Impostor, and Vampire Chat can be seen here.";
                        _teamText.color = Color.white;
                    }

                    var ChatScreenContainer = GameObject.Find("ChatScreenContainer");
                    // var FreeChat = GameObject.Find("FreeChatInputField");
                    var Background = ChatScreenContainer.transform.FindChild("Background");
                    // var bubbleItems = GameObject.Find("Items");
                    // var typeBg = FreeChat.transform.FindChild("Background");
                    // var typeText = FreeChat.transform.FindChild("Text");

                    if (TeamChatActive)
                    {
                        if (PlayerControl.LocalPlayer.TryGetModifier<JailedModifier>(out var jailMod) && !jailMod.HasOpenedQuickChat)
                        {
                            if (!__instance.quickChatMenu.IsOpen) __instance.OpenQuickChat();
                            __instance.quickChatMenu.Close();
                            jailMod.HasOpenedQuickChat = true;
                        }
                        var ogChat = HudManager.Instance.Chat.chatButton;
                        ogChat.transform.Find("Inactive").gameObject.SetActive(true);
                        ogChat.transform.Find("Active").gameObject.SetActive(false);
                        ogChat.transform.Find("Selected").gameObject.SetActive(false);

                        Background.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.1f, 0.1f, 0.8f);
                        //typeBg.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.1f, 0.1f, 0.6f);
                        //typeText.GetComponent<TextMeshPro>().color = Color.white;
                        if (MeetingHud.Instance)
                        {
                            ChatScreenContainer.transform.localPosition =
                                HudManager.Instance.Chat.chatButton.transform.localPosition -
                                new Vector3(3.5133f + 4.33f * (Camera.main.orthographicSize / 3f), 4.576f);
                        }
                        else
                        {
                            ChatScreenContainer.transform.localPosition =
                                HudManager.Instance.Chat.chatButton.transform.localPosition -
                                new Vector3(3.5133f + 3.49f * (Camera.main.orthographicSize / 3f), 4.576f);
                        }

                        if ((PlayerControl.LocalPlayer.IsJailed() ||
                             PlayerControl.LocalPlayer.Data.Role is JailorRole) && _teamText != null)
                        {
                            _teamText.text = "Jailor Chat is Open. Only the Jailor and Jailee can see this.";
                            _teamText.color = AUSColors.Jailor;
                        }
                        else if (PlayerControl.LocalPlayer.IsImpostor() &&
                                 genOpt is { ImpostorChat.Value: true } &&
                                 !PlayerControl.LocalPlayer.Data.IsDead && _teamText != null)
                        {
                            _teamText.text = "Impostor Chat is Open. Only Impostors can see this.";
                            _teamText.color = AUSColors.Mafia;
                        }
                        else if (_teamText != null)
                        {
                            _teamText.text = "Jailor, Impostor, and Vampire Chat can be seen here.";
                            _teamText.color = Color.white;
                        }
                        /* foreach (var bubble in bubbleItems.GetAllChilds())
                            {
                                bubble.gameObject.SetActive(true);
                                var bg = bubble.transform.Find("Background").gameObject;
                                if (bg != null)
                                {
                                    var sprite = bg.GetComponent<SpriteRenderer>();
                                    var color = sprite.color.SetAlpha(1f);
                                    if (color == Color.white || color == Color.black) bubble.gameObject.SetActive(false);
                                }
                            }
                        __instance.AlignAllBubbles(); */
                    }
                    else
                    {
                        Background.GetComponent<SpriteRenderer>().color = Color.white;
                        ChatScreenContainer.transform.localPosition =
                            HudManager.Instance.Chat.chatButton.transform.localPosition - new Vector3(3.5133f + 3.49f * (Camera.main.orthographicSize / 3f), 4.576f);
                    }
                }
                else if (TeamChatActive)
                {
                    ToggleTeamChat();
                }
            }
            catch
            {
                // Nothing Happens Here
            }
        }
    }

    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Close))]
    public static class ClosePatch
    {
        public static void Postfix(ChatController __instance)
        {
            if (TeamChatActive)
            {
                ToggleTeamChat();
            }
        }
    }
}