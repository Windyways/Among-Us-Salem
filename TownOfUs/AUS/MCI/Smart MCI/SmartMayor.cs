using System.Collections;
using UnityEngine;

namespace AmongUsSalem.MCI.SmartMCI;

public static class SmartMayor
{
    // --- A MAYOR WOULD REVEAL BECAUSE ---
    // 1. A Ritualist died during the game.
    // 2. Is found with Incriminating Evidence.
    // 3. A player is considered 'Confirmed Evil'.
    // 4. It sees a player kill.
    // 5. There are less than or equal to 7 players left.
    // 6. 25% chance to just straight up reveal D2.

    public static void Start()
    {
        if (Debugger.IsDebuggerActive && Debugger.SmartBotsEnabled) Coroutines.Start(DelayStart());
    }

    public static IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(DayNightMechanic.PostMeetingIntroTime + 0.5f);
        if (SmartStarspawn.IsActive) yield break;

        if (DayNightMechanic.DayCount == 1)
            yield break;

        foreach (var mayors in MiscUtils.GetPlayersWithRole<Mayor>())
        {
            var mayor = mayors.GetRole<Mayor>();

            bool anyRitualistsDead = PlayerControl.AllPlayerControls.ToArray().Any(x => x.HasDied() && x.GetRoleWhenAlive() is Ritualist);
            bool anyConfirmedEvil = PlayerControl.AllPlayerControls.ToArray().Any(x => !x.HasDied() && x.HasModifier<ConfirmedEvil>());
            bool lessThanHalf = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied()) <= 7;
            bool instaReveal = CalculatedVoting.ChanceIs(25) && DayNightMechanic.DayCount == 2;
            if (anyRitualistsDead || mayors.HasModifier<IncriminatingEvidence>() || anyConfirmedEvil || mayors.HasModifier<SeenKill>() || lessThanHalf || instaReveal)
            {
                mayor.DoReveal();
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
