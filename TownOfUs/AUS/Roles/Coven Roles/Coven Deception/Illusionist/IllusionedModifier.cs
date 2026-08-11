namespace AmongUsSalem.Modifiers;

public sealed class IllusionedModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Illusioned";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
    public override void OnActivate()
    {
        if (!Caster.HasModifier<OverchargedModifier>())
        {
            var player = ModifierUtils.GetPlayersWithModifier<IllusionedModifier>(x => x.Caster == Caster && x != this).FirstOrDefault();
            player?.RpcRemoveModifier<IllusionedModifier>();
        }
    }
}

public static class IllusionedModifier_Events
{
    [RegisterEvent]
    public static void RoundStartEvent(RoundStartEvent @event)
    {
        foreach (var player in ModifierUtils.GetPlayersWithModifier<IllusionedModifier>())
            player.RpcRemoveModifier<IllusionedModifier>();
    }
}