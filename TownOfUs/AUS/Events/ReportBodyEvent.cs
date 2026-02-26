namespace AmongUsSalem.Events;

public static class AUS_ReportBodyEvent
{
    [RegisterEvent]
    public static void ReportBodyEvent(ReportBodyEvent @event)
    {
        AUS_AfterVoteEvent.Voters.Clear();

        var player = @event.Reporter;
        var target = MiscUtils.PlayerById(@event.Target.PlayerId);

        if (target.TryGetModifier<JinxedModifier>(out var jinxed) && !player.Is(Faction.Coven)) 
            player.RpcAddModifier<JinxReportKillModifier>(jinxed.Caster, target);

        if (target.TryGetModifier<FortifiedModifier>(out var fortified) && player != fortified.Caster)
            player.RpcAddModifier<FortifyReportKillModifier>(fortified.Caster, target);
    }
}