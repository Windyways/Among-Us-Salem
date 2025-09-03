using HarmonyLib;
using AmongUsSalem.Modules;
using AmongUs.GameOptions;
using UnityEngine;

namespace AmongUsSalem.Patches;

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
public static class MeetingMenuUpdatePatch
{
    public static void Postfix()
    {
        MeetingMenu.Instances.Do(x => x.Update());
    }
}