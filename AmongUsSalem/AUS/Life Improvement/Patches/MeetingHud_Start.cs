using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using AmongUsSalem.LifeImprovement.MCI.SmartMCI;

namespace AmongUsSalem.LifeImprovement.Patches;

[HarmonyPatch(typeof(MeetingHud))]
public static class MeetingHud_Start
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(MeetingHud.Start))]
    public static void StartPostfix()
    {
        Keyboard_Joystick.RefreshSwapTargets();

        CalculatedVoting.mafiasAreSkipping = false;
        CalculatedVoting.PairMafiaVotingTarget = null;

        CalculatedVoting.covensAreSkipping = false;
        CalculatedVoting.PairCovenVotingTarget = null;
    }
}