using BepInEx.Unity.IL2CPP.Utils;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AmongUsSalem.LifeImprovement.MCI.SmartMCI;

public static class SmartConjurer
{
    public static void Start()
    {
        if (Debugger.IsDebuggerActive) Coroutines.Start(DelayStart());
    }

    public static IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(DayNightMechanic.PostMeetingIntroTime + 1.25f);
        var alivePlayers = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied());
        var aliveCoven = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.Is(Faction.Coven));
        foreach (var conjurers in MiscUtils.GetPlayersWithRole<Conjurer>())
        {
            if (DayNightMechanic.DayCount >= 2)
            {
                var conjurer = conjurers.GetRole<Conjurer>();

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Data.Role is Mayor mayor && mayor.IsRevealed)
                    {
                        conjurer.DoConjure(mayor.Player);
                        yield break;
                    }
                }

                if ((alivePlayers - 1) <= aliveCoven) conjurer.DoConjure(GetRandomTarget());
                else if (CalculatedVoting.EvidenceAgainst.Contains(conjurer.Player) || CalculatedVoting.KillerContagious.Contains(conjurer.Player))
                {
                    conjurer.DoConjure(GetRandomTarget());
                }
            }
        }
    }

    public static void DoConjure(this Conjurer conjurer, PlayerControl target)
    {
        if (conjurer.Charges == 0 || conjurer.Player.HasDied())
            return;

        if (conjurer.Player.CanKill(target, Attack.Powerful))
        {
            MiscUtils.RpcApplyDeathReason(conjurer.Player, target, DeathReasonShow.KilledByAConjurer, false, false);
            Conjurer.RpcConjurer_Conjure(conjurer.Player, target);
            conjurer.Charges--;
        }
    }

    public static PlayerControl GetRandomTarget()
    {
        var validTargets = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && !x.Is(Faction.Coven)).ToList();
        if (validTargets.Count > 0)
        {
            PlayerControl newTarget = validTargets[Random.RandomRangeInt(0, validTargets.Count)];
            validTargets.Remove(newTarget);

            for (var i = 0; i < CalculatedVoting.EvidenceAgainst.Count; i++)
            {
                if (!CalculatedVoting.EvidenceAgainst[i].Is(Faction.Coven)) return CalculatedVoting.EvidenceAgainst[i];
            }

            for (var i = 0; i < CalculatedVoting.KillerContagious.Count; i++)
            {
                if (!CalculatedVoting.KillerContagious[i].Is(Faction.Coven)) return CalculatedVoting.KillerContagious[i];
            }

            return newTarget;
        }
        return null;
    }
}