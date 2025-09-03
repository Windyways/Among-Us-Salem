using BepInEx.Unity.IL2CPP.Utils;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AmongUsSalem.LifeImprovement.MCI.SmartMCI;

public static class SmartDeputy
{
    public static void Start()
    {
        if (Debugger.IsDebuggerActive) Coroutines.Start(DelayStart());
    }

    public static IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(DayNightMechanic.PostMeetingIntroTime + 1.5f);
        var alivePlayers = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied());
        var evilPlayers = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && !x.Is(Faction.Town));
        foreach (var deputys in MiscUtils.GetPlayersWithRole<Deputy>())
        {
            if (DayNightMechanic.DayCount >= 2)
            {
                var deputy = deputys.GetRole<Deputy>();

                var num = Random.Range(0, 100);
                if ((alivePlayers / 2) <= evilPlayers && num <= 25) deputy.DoShoot(GetRandomTarget(deputy.Player));
                else if (CalculatedVoting.EvidenceAgainst.Contains(deputy.Player))
                {
                    deputy.DoShoot(GetRandomTarget(deputy.Player));
                }
                else if (CalculatedVoting.EvidenceAgainst.Count > 0) deputy.DoShoot(CalculatedVoting.EvidenceAgainst.Random());
                else if (CalculatedVoting.KillerContagious.Count > 0) deputy.DoShoot(CalculatedVoting.KillerContagious.Random());
            }
        }
    }

    public static void DoShoot(this Deputy deputy, PlayerControl target)
    {
        if (deputy.Charges == 0 || deputy.Player.HasDied())
            return;

        if (deputy.Player.CanKill(target))
        {
            Deputy.RpcDeputy_Shoot(deputy.Player, target);
            deputy.Charges--;
        }
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