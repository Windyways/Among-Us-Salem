using HarmonyLib;
using ObjectWorkshop.Modules;

namespace ObjectWorkshop.Patches;

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
public static class MeetingMenuUpdatePatch
{
    public static void Postfix()
    {
        MeetingMenu.Instances.Do(x => x.Update());
    }
}