using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using TownOfUs.Modifiers.Game;
using TownOfUs.Options;

namespace TownOfUs.Patches.Roles;

[HarmonyPatch(typeof(MeetingHud))]
public static class MeetingHudTimerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(MeetingHud.UpdateTimerText))]
    public static void TimerUpdatePostfix(MeetingHud __instance)
    {
        var newText = string.Empty;
        if (PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || PlayerControl.LocalPlayer.HasDied()) 
            return;

        switch (PlayerControl.LocalPlayer.Data.Role)
        {
            case Deputy deputy:
                newText = $"\nHigh Noons Left: {deputy.Charges}";
                break;
            case Prosecutor prosecutor:
                newText = $"\nProsecutes Left: {prosecutor.Charges}";
                break;
            case Jester jester:
                if (OptionGroupSingleton<Jester_Options>.Instance.EnableChatterbox)
                {
                    string text = $"\nChatterBox: {jester.MessagesSent} / {jester.ChatterBoxQuota}";
                    newText = text;
                    if (jester.MessagesSent >= jester.ChatterBoxQuota) newText = $"<color=#00ff00>{text}</color>";
                }
                break;
            case Starspawn starspawn:
                newText = $"\nDaybreaks Left: {starspawn.Charges}";
                break;
            case Admirer admirer:
                newText = $"\nBestows Left: {admirer.Charges}";
                break;
        }

        if (newText != string.Empty) __instance.TimerText.text += $"<color=#FFFFFF>{newText}</color>";
    }
}