using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using ObjectWorkshop.LifeImprovement.MCI.SmartMCI;

namespace ObjectWorkshop.LifeImprovement.Patches;

[HarmonyPatch(typeof(MeetingHud))]
public static class MeetingHud_Start
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(MeetingHud.Start))]
    public static void StartPostfix()
    {
        Keyboard_Joystick.RefreshSwapTargets();
        CalculatedVoting.infiltratorsAreSkipping = false;
    }
}