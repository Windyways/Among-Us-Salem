namespace AmongUsSalem.Modifiers;

public sealed class WarlockFramedModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Warlock Framed";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
}

public static class WarlockFramedModifier_Events
{
    [RegisterEvent]
    public static void RoundStartEvent(RoundStartEvent @event)
    {
        foreach (var player in ModifierUtils.GetPlayersWithModifier<WarlockFramedModifier>())
            player.RpcRemoveModifier<WarlockFramedModifier>();
    }
}