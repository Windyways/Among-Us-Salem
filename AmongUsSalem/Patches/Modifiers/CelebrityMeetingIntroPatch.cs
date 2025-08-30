using HarmonyLib;
using MiraAPI.Modifiers;
using Reactor.Utilities.Extensions;
using TMPro;
using ObjectWorkshop.Modifiers.Game.Crewmate;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Patches.Modifiers;

[HarmonyPatch]
public static class CelebrityMeetingIntroPatch
{
    [HarmonyPatch(typeof(MeetingIntroAnimation), nameof(MeetingIntroAnimation.Init))]
    [HarmonyPostfix]
    public static void MeetingIntroAnimationPatch(MeetingIntroAnimation __instance)
    {
        //if (PlayerControl.AllPlayerControls.ToArray().Any(x => x.protectedByGuardianThisRound && !x.HasDied())) return;

        var celebrity = ModifierUtils.GetActiveModifiers<CelebrityModifier>(x => x.Player.HasDied() && !x.Announced)
            .FirstOrDefault();

        if (celebrity == null)
        {
            return;
        }

        __instance.ProtectedRecently.SetActive(true);
        var textObj = __instance.ProtectedRecently.transform.FindChild("ProtectedText_TMP");
        var textTMP = textObj.GetComponent<TextMeshPro>();

        celebrity.Announced = true;

        //var milliSeconds = (float)(DateTime.UtcNow - celebrity.DeathTime).TotalMilliseconds;
        celebrity.DeathMessage += $"{Math.Round(celebrity.DeathTimeMilliseconds / 1000)} seconds ago.";

        textTMP.text = celebrity.AnnounceMessage;

        var iconObj = __instance.ProtectedRecently.transform.FindChild("UI_ProtectionIcon");
        var iconSprite = iconObj.GetComponent<SpriteRenderer>();
        iconSprite.sprite = TouModifierIcons.Celebrity.LoadAsset();

        if (HudManager.Instance != null)
        {
            var title = $"<color=#{OWColors.Celebrity.ToHtmlStringRGBA()}>Celebrity Report</color>";
            MiscUtils.AddFakeChat(celebrity.Player.Data, title, celebrity.DeathMessage, false, true);
        }
    }
}