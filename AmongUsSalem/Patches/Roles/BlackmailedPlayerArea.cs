using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using AmongUsSalem.Modifiers.Impostor;
using AmongUsSalem.Options.Roles.Impostor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AmongUsSalem.Patches.Roles;

[HarmonyPatch(typeof(PlayerVoteArea))]
public static class BlackmailedPlayerArea
{
    public static bool shookAlready = true;
    public static SpriteRenderer BmOverlay;

    [HarmonyPostfix]
    [HarmonyPatch(nameof(PlayerVoteArea.SetCosmetics))]
    public static void SetCosmeticsPostfix(PlayerVoteArea __instance, NetworkedPlayerInfo playerInfo)
    {
        var bmMod = playerInfo?.Object?.GetModifier<BlackmailedModifier>();

        if (bmMod == null)
        {
            return;
        }

        var amOwner = playerInfo?.Object?.AmOwner;
        var bmOwns = bmMod.BlackMailerId == PlayerControl.LocalPlayer.PlayerId;
        var targetSeeOnly = OptionGroupSingleton<BlackmailerOptions>.Instance.OnlyTargetSeesBlackmail;
        var maxAliveNeeded = OptionGroupSingleton<BlackmailerOptions>.Instance.MaxAliveForVoting;

        if (amOwner == true || bmOwns || !targetSeeOnly)
        {
            shookAlready = false;
            var bmIcon = Object.Instantiate(__instance.XMark, __instance.XMark.transform.parent);
            bmIcon.transform.localPosition = new Vector3(-0.804f, -0.212f, -2);
            bmIcon.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            bmIcon.sprite = TouAssets.BlackmailLetterSprite.LoadAsset();
            bmIcon.gameObject.SetActive(true);

            BmOverlay = Object.Instantiate(__instance.XMark, __instance.XMark.transform.parent);
            BmOverlay.transform.localPosition = new Vector3(0, 0, -2);
            BmOverlay.transform.localScale = new Vector3(0.769f, 1, 1);
            BmOverlay.sprite = TouAssets.BlackmailOverlaySprite.LoadAsset();
            BmOverlay.gameObject.SetActive(true);
            __instance.ColorBlindName.gameObject.SetActive(false);
        }

        if (Helpers.GetAlivePlayers().Count > maxAliveNeeded)
        {
            __instance.SetVote(252);
            if (targetSeeOnly)
            {
                __instance.Flag.enabled = false;
            }

            if (amOwner == true)
            {
                MeetingHud.Instance.Confirm(252);
            }
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    public static class BmHudUpdate
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (__instance.state != MeetingHud.VoteStates.Animating && !shookAlready)
            {
                shookAlready = true;
                __instance.StartCoroutine(Effects.SwayX(BmOverlay.transform));
            }
        }
    }
}