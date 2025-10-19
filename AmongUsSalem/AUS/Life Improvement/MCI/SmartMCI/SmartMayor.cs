using BepInEx.Unity.IL2CPP.Utils;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AmongUsSalem.LifeImprovement.MCI.SmartMCI;

public static class SmartMayor
{
    public static void Start()
    {
        if (Debugger.IsDebuggerActive && Debugger.SmartBotsEnabled) Coroutines.Start(DelayStart());
    }

    public static IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(DayNightMechanic.PostMeetingIntroTime + 1f);
        var alivePlayers = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied());
        var evilPlayers = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && !x.Is(Faction.Town));
        var aliveConjurers = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.IsRole<Conjurer>());
        foreach (var mayors in MiscUtils.GetPlayersWithRole<Mayor>())
        {
            if (DayNightMechanic.DayCount >= 2)
            {
                var mayor = mayors.GetRole<Mayor>();
                if (CalculatedVoting.EvidenceAgainst.Contains(mayor.Player) || CalculatedVoting.KillerContagious.Contains(mayor.Player))
                {
                    mayor.DoReveal();
                }
                else if (alivePlayers <= 6) mayor.DoReveal();
                else if ((alivePlayers / 2) <= evilPlayers) mayor.DoReveal();
                else if (aliveConjurers == 0) mayor.DoReveal();
            }
        }
    }

    public static void DoReveal(this Mayor mayor)
    {
        if (mayor.Player.HasModifier<GlobalReveal>() || mayor.Player.HasDied())
            return;

        Mayor.RpcMayor_Reveal(mayor.Player);
        if (mayor.Player.AmOwner)
        {
            mayor.meetingMenu?.HideButtons();
        }
    }
}