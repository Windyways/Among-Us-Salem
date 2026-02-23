using AmongUsSalem.Roles;
using Reactor.Utilities.Extensions;
using System.Collections;
using UnityEngine;

namespace AmongUsSalem.MCI.SmartMCI;

public static class SmartDeputy
{
    // --- A DEPUTY WOULD SHOOT SOMEONE IF ---
    // 1. A player is found with Incriminating Evidence.
    // 2. A player was caught killing.
    // 3. A player is considered 'Confirmed Evil'.
    // 4. A random player if there are 7 or less players alive.

    public static void Start()
    {
        if (Debugger.IsDebuggerActive && Debugger.SmartBotsEnabled) Coroutines.Start(DelayStart());
    }

    public static IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(DayNightMechanic.PostMeetingIntroTime + 1.5f);
        if (SmartProsecutor.IsActive) yield return new WaitForSeconds(8f);

        if (DayNightMechanic.DayCount == 1)
            yield break;

        foreach (var deputys in MiscUtils.GetPlayersWithRole<Deputy>())
        {
            var deputy = deputys.GetRole<Deputy>();
            var allPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && x != deputys).ToList();

            var incriminatingEvidence = IncriminatingEvidence.GetRandom();
            var seenKill = SeenKill.GetAll();
            var confirmedEvil = ConfirmedEvil.GetAll();

            if (confirmedEvil != null) deputy.DoShoot(confirmedEvil.Player);
            else if (incriminatingEvidence != null) deputy.DoShoot(incriminatingEvidence.Player);
            else if (seenKill != null) deputy.DoShoot(seenKill.killer);
            else if (allPlayers.Count <= 6) deputy.DoShoot(allPlayers.Random());
        }
    }


    public static void DoShoot(this Deputy deputy, PlayerControl target)
    {
        if (deputy.Charges <= 0 || deputy.Player.HasDied() || target.HasDied())
            return;

        deputy.Charges--;
        Deputy.RpcDeputy_HighNoon(deputy.Player, target);
        if (deputy.Player.AmOwner)
        {
            deputy.meetingMenu?.HideButtons();
        }
    }
}
