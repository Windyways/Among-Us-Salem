namespace AmongUsSalem.Events;

public static class AUS_AfterMurderEvent
{
    [RegisterEvent(1)]
    public static void AfterMurderEvent(AfterMurderEvent @event)
    {
        var victim = @event.Target;
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player.HasDied())
            {
                var role = player.GetRoleWhenAlive();
                if (role is ICustomAURole cr) cr.Role_AfterMurder(victim);
            }
            else if (player.Data.Role is ICustomAURole customRole)
            {
                customRole.Role_AfterMurder(victim);
            }
        }
    }
}