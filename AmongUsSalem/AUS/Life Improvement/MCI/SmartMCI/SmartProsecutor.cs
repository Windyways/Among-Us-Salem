using BepInEx.Unity.IL2CPP.Utils;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AmongUsSalem.LifeImprovement.MCI.SmartMCI;

public static class SmartProsecutor
{
    public static void Start()
    {
        if (Debugger.IsDebuggerActive && Debugger.SmartBotsEnabled) Coroutines.Start(DelayStart());
    }

    public static IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(DayNightMechanic.PostMeetingIntroTime + 1.4f); // Priority over deputy.
        var townPlayers = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.Is(Faction.Town));
        foreach (var prosecutors in MiscUtils.GetPlayersWithRole<Prosecutor>())
        {
            if (DayNightMechanic.DayCount >= 2)
            {
                var prosecutor = prosecutors.GetRole<Prosecutor>();
                if (prosecutor.Player.HasModifier<VampireRecruit>()) prosecutor.DoProsecute(GetRandomTarget(prosecutor.Player));
                else if (prosecutor.Player.HasModifier<JackalRecruit>()) prosecutor.DoProsecute(GetRandomTarget(prosecutor.Player));
                else
                {
                    if (townPlayers == 1) prosecutor.DoProsecute(GetRandomTarget(prosecutor.Player));
                    else if (CalculatedVoting.EvidenceAgainst.Contains(prosecutor.Player))
                    {
                        prosecutor.DoProsecute(GetRandomTarget(prosecutor.Player));
                    }
                    else if (CalculatedVoting.EvidenceAgainst.Count > 0) prosecutor.DoProsecute(CalculatedVoting.EvidenceAgainst.Random());
                    else if (CalculatedVoting.KillerContagious.Count > 0) prosecutor.DoProsecute(CalculatedVoting.KillerContagious.Random());
                }
            }
        }
    }

    public static void DoProsecute(this Prosecutor prosecutor, PlayerControl target)
    {
        if (prosecutor.Charges == 0 || prosecutor.Player.HasDied())
            return;

        Prosecutor.RpcProsecutor_Prosecute(prosecutor.Player, target);
        prosecutor.Charges--;
    }

    public static PlayerControl GetRandomTarget(PlayerControl player)
    {
        var validTargets = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && !x.IsInnocent() && x != player).ToList();
        if (player.HasModifier<VampireRecruit>()) validTargets = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && !x.HasModifier<VampireRecruit>() && !x.IsRole<Vampire>()).ToList();
        if (player.HasModifier<JackalRecruit>()) validTargets = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && !x.HasModifier<JackalRecruit>()).ToList();

        if (validTargets.Count > 0)
        {
            PlayerControl newTarget = validTargets[Random.RandomRangeInt(0, validTargets.Count)];
            validTargets.Remove(newTarget);

            for (var i = 0; i < CalculatedVoting.EvidenceAgainst.Count; i++)
            {
                return CalculatedVoting.EvidenceAgainst[i];
            }

            for (var i = 0; i < CalculatedVoting.KillerContagious.Count; i++)
            {
                return CalculatedVoting.KillerContagious[i];
            }

            return newTarget;
        }
        return null;
    }
}