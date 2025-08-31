namespace AmongUsSalem.LifeImprovement.GameMechanics;

public static class DayNightMechanic
{
    public static int DayCount;
    public static int NightCount;
    public static float NightTimer;

    public static bool FullMoon()
    {
        return NightCount == 2 || NightCount >= 4;
    }

    public static bool HalfMoon()
    {
        return !FullMoon();
    }

    public static void OnLobbyStart()
    {
        DayCount = 0;
        NightCount = 0;
    }

    #region Events
    #endregion
    
    [RegisterEvent]
    public static void StartMeetingEventHandler(StartMeetingEvent @event)
    {
        DayCount++;
        
        foreach (var role in GameHistory.AllRoles)
        {
            if (!role || role is not IAUSRole ausRole)
            {
                continue;
            }

            ausRole.Attack = ausRole.ogAttack;
            ausRole.Defense = ausRole.ogDefense;
            ausRole.EtherealDefense = ausRole.ogEtherealDefense;
        }
    }

    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            return; // Only run when round starts.
        }

        NightCount++;
    }
}