namespace AmongUsSalem.Events;

public static class AUS_RoundStartEvent
{
    [RegisterEvent(1)]
    public static void RoundStartEvent(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
            return;

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player.HasDied())
            {
                var role = player.GetRoleWhenAlive();
                if (role is ICustomAURole cr) cr.Role_OnRoundStart();
            }
            else if (player.Data.Role is ICustomAURole customRole)
            {
                customRole.Role_OnRoundStart();
            }
        }
    }
}