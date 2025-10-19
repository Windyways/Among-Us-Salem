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
        
        CalculatedVoting.vampiresAreSkipping = false;
        CalculatedVoting.PairVampireVotingTarget = null;

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (CalculatedVoting.QueueEvidenceAgainst.TryGetValue(player, out var target) && !player.IsBlackmailed() && !player.IsSilenced() && !player.HasDied())
            {
                CalculatedVoting.EvidenceAgainst.Add(target);
            }
            if (CalculatedVoting.QueueKillerContagious.TryGetValue(player, out var target2) && !player.IsBlackmailed() && !player.IsSilenced() && !player.HasDied())
            {
                CalculatedVoting.KillerContagious.Add(target2);
            }
        }

        foreach (var queued in CalculatedVoting.QueueRecievedInformation)
        {
            CalculatedVoting.RecievedInformation.Add(queued);
        }

        CalculatedVoting.QueueEvidenceAgainst.Clear();
        CalculatedVoting.QueueKillerContagious.Clear();
        CalculatedVoting.QueueRecievedInformation.Clear();
        
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player.HasDied())
            {
                CalculatedVoting.EvidenceAgainst.Remove(player);
                CalculatedVoting.KillerContagious.Remove(player);
            }
        }
    }
}