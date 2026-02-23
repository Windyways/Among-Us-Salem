namespace AmongUsSalem.Events;

public static class AUS_ReportBodyEvent
{
    [RegisterEvent]
    public static void ReportBodyEvent(ReportBodyEvent @event)
    {
        var player = @event.Reporter;
        var target = MiscUtils.PlayerById(@event.Target.PlayerId);

        if (target.TryGetModifier<JinxedModifier>(out var jinxed) && !player.Is(Faction.Coven)) 
            player.RpcAddModifier<JinxReportKillModifier>(jinxed.Caster, target);
    }
}