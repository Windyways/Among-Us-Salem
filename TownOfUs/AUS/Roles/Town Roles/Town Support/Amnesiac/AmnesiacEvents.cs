namespace AmongUsSalem.Roles;

public static class Amnesiac_Events
{
    [RegisterEvent]
    public static void EjectionEvent(EjectionEvent @event)
    {
        NetworkedPlayerInfo exiled = @event.ExileController.initData.networkedPlayer;
        if (exiled != null)
        {
            PlayerControl player = exiled.Object;
            foreach (var amnesiac in MiscUtils.GetRoles<Amnesiac>()) amnesiac.OnTargetDeath(player);
        }
    }

    [RegisterEvent]
    public static void AfterMurderEvent(AfterMurderEvent @event)
    {
        foreach (var amnesiac in MiscUtils.GetRoles<Amnesiac>()) amnesiac.OnTargetDeath(@event.Target);
    }
}