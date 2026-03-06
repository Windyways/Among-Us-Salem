namespace AmongUsSalem.Modifiers;

public sealed class FramedModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Framed";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
    public override void OnMeetingStart()
    {
        if (!OptionGroupSingleton<Framer_Options>.Instance.RemovedOnVisit) 
            Player.RpcRemoveModifier<FramedModifier>();
    }
}

public static class FramedModifier_Events
{
    [RegisterEvent]
    public static void RoundStartEvent(RoundStartEvent @event)
    {
        foreach (var player in ModifierUtils.GetPlayersWithModifier<FramedModifier>())
            if (!OptionGroupSingleton<Framer_Options>.Instance.RemovedOnVisit)
                player.RpcRemoveModifier<FramedModifier>();
    }
}