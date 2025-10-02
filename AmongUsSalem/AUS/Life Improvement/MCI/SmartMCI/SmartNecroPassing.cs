using BepInEx.Unity.IL2CPP.Utils;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AmongUsSalem.LifeImprovement.MCI.SmartMCI;

public static class SmartNecroPassing
{
    public static void Start(NecroPassing necroPassing)
    {
        if (Debugger.IsDebuggerActive && Debugger.SmartBotsEnabled) Coroutines.Start(DelayStart(necroPassing));
    }

    public static IEnumerator DelayStart(NecroPassing necroPassing)
    {
        yield return new WaitForSeconds(DayNightMechanic.PostMeetingIntroTime + 3f);
        // I'll make and fix this some other time.
    }

    public static void VoteToPass(this PlayerControl player, PlayerControl target)
    {
        if (target.HasModifier<NecroPassing>())
        {
            var necroPassing = target.GetModifier<NecroPassing>();
            if (!necroPassing.VotesToGetBook.Contains(player.PlayerId))
            {
                NecroPassing.RpcNecroPassing_PassNecronomicon(player, target);
            }
        }
    }
}