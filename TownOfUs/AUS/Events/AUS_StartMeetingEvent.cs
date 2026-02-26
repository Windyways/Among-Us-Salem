namespace AmongUsSalem.Events;

public static class AUS_StartMeetingEvent
{
    [RegisterEvent(1)]
    public static void StartMeetingEvent(StartMeetingEvent @event)
    {
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player.HasDied())
            {
                var role = player.GetRoleWhenAlive();
                if (role is ICustomAURole cr) cr.Role_OnMeetingStart();
            }
            else if (player.Data.Role is ICustomAURole customRole)
            {
                customRole.Role_OnMeetingStart();
            }
        }
    }
}