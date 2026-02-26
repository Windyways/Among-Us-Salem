namespace AmongUsSalem.Events;

public static class AUS_BeforeMurderEvent
{
    [RegisterEvent(1)]
    public static void BeforeMurderEvent(BeforeMurderEvent @event)
    {
        var target = @event.Target;
        if (target is ICustomAURole customRole)
        {
            target.RpcAddModifier<LinkStatAD>((int)customRole.Attack, (int)customRole.Defense);
        }
    }
}