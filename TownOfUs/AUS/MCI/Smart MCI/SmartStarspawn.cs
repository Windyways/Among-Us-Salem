using Reactor.Utilities.Extensions;
using System.Collections;
using UnityEngine;

namespace AmongUsSalem.MCI.SmartMCI;

public static class SmartStarspawn
{
    // --- A STARSPAWN WOULD DAYBREAK IF ---
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
        yield return new WaitForSeconds(DayNightMechanic.PostMeetingIntroTime);
        if (DayNightMechanic.DayCount == 1 || IsActive)
            yield break;

        foreach (var starspawn in MiscUtils.GetRoles<Starspawn>())
        {
            var allPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && x != starspawn.Player).ToList();

            var incriminatingEvidence = IncriminatingEvidence.GetRandom();
            var seenKill = SeenKill.GetAll();
            var confirmedEvil = ConfirmedEvil.GetAll();

            if (confirmedEvil != null) starspawn.DoDaybreak();
            else if (incriminatingEvidence != null) starspawn.DoDaybreak();
            else if (seenKill != null) starspawn.DoDaybreak();
            else if (allPlayers.Count <= 6) starspawn.DoDaybreak();
        }
    }


    public static void DoDaybreak(this Starspawn starspawn)
    {
        if (starspawn.Charges <= 0 || starspawn.Player.HasDied())
            return;

        starspawn.Charges--;
        Starspawn.RpcDaybreak(starspawn.Player);
        IsActive = true;

        if (starspawn.Player.AmOwner)
        {
            starspawn.meetingMenu?.HideButtons();
        }
    }
}
