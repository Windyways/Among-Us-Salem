using BepInEx.Unity.IL2CPP.Utils;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AmongUsSalem.LifeImprovement.MCI.SmartMCI;

public static class SmartCoroner
{
    public static void Start()
    {
        if (Debugger.IsDebuggerActive && Debugger.SmartBotsEnabled) Coroutines.Start(DelayStart());
    }

    public static IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(DayNightMechanic.PostMeetingIntroTime + 2.25f);
        foreach (var coroners in MiscUtils.GetPlayersWithRole<Coroner>())
        {
            var coroner = coroners.GetRole<Coroner>();

            var target = coroner.GetRandomTarget();
            if (target == null)
                yield break;

            coroner.NoExamine(target);
        }
    }

    public static void NoExamine(this Coroner coroner, PlayerControl target)
    {
        if (coroner.Player.HasDied())
            return;

        if (target.TryGetModifier<DeathHandlerModifier>(out var deathMod))
        {
            var killer = deathMod.KillerPlayer;
            if (killer.Data.Role is IAUSRole ausRole && !coroner.AutopsiedPlayers.Contains(killer.PlayerId))
            {
                coroner.AutopsiedPlayers.Add(killer.PlayerId);
                coroner.AutopsiedRoles.Add(ausRole.RoleName);
                coroner.recentlyAutopsied = killer;
                
                AUSPlugin.DebugLogMessage("Coroner Autopsy - " + ausRole.RoleName);
            }
        }
    }

    public static PlayerControl GetRandomTarget(this Coroner coroner)
    {
        var validTargets = PlayerControl.AllPlayerControls.ToArray().Where(x => x.HasDied() && !coroner.AutopsiedPlayers.Contains(x.PlayerId)).ToList();
        if (validTargets.Count > 0)
        {
            PlayerControl newTarget = validTargets[Random.RandomRangeInt(0, validTargets.Count)];
            validTargets.Remove(newTarget);
            return newTarget;
        }
        return null;
    }
}