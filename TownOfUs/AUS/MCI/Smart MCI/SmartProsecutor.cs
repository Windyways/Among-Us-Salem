using Reactor.Utilities.Extensions;
using System.Collections;
using UnityEngine;

namespace AmongUsSalem.MCI.SmartMCI;

public static class SmartProsecutor
{
    // --- A Prosecutor WOULD Prosecute SOMEONE IF ---
    // 1. A player is found with Incriminating Evidence.
    // 2. A player was caught killing.
    // 3. A player is considered 'Confirmed Evil'.
    // 4. A random player if there are 7 or less players alive.

    public static bool IsActive;
    public static void Start()
    {
        IsActive = false;
        if (Debugger.IsDebuggerActive && Debugger.SmartBotsEnabled) Coroutines.Start(DelayStart());
    }

    public static IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(DayNightMechanic.PostMeetingIntroTime + 1f);
        if (DayNightMechanic.DayCount == 1)
            yield break;

        foreach (var prosecutor in MiscUtils.GetRoles<Prosecutor>())
        {
            var allPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && x != prosecutor.Player).ToList();

            var incriminatingEvidence = IncriminatingEvidence.GetRandom();
            var seenKill = SeenKill.GetAll();
            var confirmedEvil = ConfirmedEvil.GetAll();

            if (confirmedEvil != null) prosecutor.DoProsecute(confirmedEvil.Player);
            else if (incriminatingEvidence != null) prosecutor.DoProsecute(incriminatingEvidence.Player);
            else if (seenKill != null) prosecutor.DoProsecute(seenKill.killer);
            else if (allPlayers.Count <= 6) prosecutor.DoProsecute(allPlayers.Random());
        }
    }


    public static void DoProsecute(this Prosecutor prosecutor, PlayerControl target)
    {
        if (prosecutor.Charges <= 0 || prosecutor.Player.HasDied() || target.HasDied())
            return;

        IsActive = true;
        prosecutor.Charges--;
        Prosecutor.RpcProsecutor_Prosecute(prosecutor.Player, target);
        if (prosecutor.Player.AmOwner)
        {
            prosecutor.meetingMenu?.HideButtons();
        }
    }
}
