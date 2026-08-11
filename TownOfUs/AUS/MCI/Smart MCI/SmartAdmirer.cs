using System.Collections;
using UnityEngine;

namespace AmongUsSalem.MCI.SmartMCI;

public static class SmartAdmirer
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
        yield return new WaitForSeconds(DayNightMechanic.PostMeetingIntroTime + 2f);
        if (SmartStarspawn.IsActive) yield break;

        if (DayNightMechanic.DayCount == 1)
            yield break;

        foreach (var admirer in MiscUtils.GetRoles<Admirer>())
        {
            var allPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && x != admirer.Player).ToList();

            var confirmed = ModifierUtils.GetPlayersWithModifier<Confirmed>(x => !x.Player.HasModifier<BestowedModifier>() && !x.Player.HasModifier<GlobalReveal>()).Random();
            var ti = ModifierUtils.GetPlayersWithModifier<Confirmed>(x => !x.Player.HasModifier<BestowedModifier>())
                .OrderBy(x => x.IsRole<Seer>() || x.IsRole<Sheriff>()).Random();
            var sc = ModifierUtils.GetPlayersWithModifier<SoftCleared>(x => !x.Player.HasModifier<BestowedModifier>()).Random();

            if (!admirer.Obsession.HasModifier<BestowedModifier>() && admirer.foundObsession) admirer.DoBestow(admirer.Obsession);
            else if (confirmed != null) admirer.DoBestow(confirmed);
            else if (ti != null) admirer.DoBestow(ti);
            else if (sc != null) admirer.DoBestow(sc);
            else if (allPlayers.Count <= 6) admirer.DoBestow(allPlayers.Random());
        }
    }

    public static void DoBestow(this Admirer admirer, PlayerControl target)
    {
        if (admirer.Charges <= 0 || admirer.Player.HasDied() || target.HasDied())
            return;

        admirer.Charges--;
        target.RpcAddModifier<BestowedModifier>(admirer.Player);

        if (admirer.Player.AmOwner)
        {
            admirer.meetingMenu?.HideButtons();
        }
    }
}
