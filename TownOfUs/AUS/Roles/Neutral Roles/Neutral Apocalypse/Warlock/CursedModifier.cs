namespace AmongUsSalem.Modifiers;

public sealed class CursedModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Cursed";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
    public override void OnActivate()
    {
        if (!Caster.HasModifier<OverchargedModifier>())
        {
            var player = ModifierUtils.GetPlayersWithModifier<CursedModifier>(x => x.Caster == Caster && x != this).FirstOrDefault();
            player?.RpcRemoveModifier<CursedModifier>();
        }
    }

    public override void OnMeetingStart()
    {
        if (Player.HasDied())
            Player.GetModifiers<CursedModifier>().Do(x => Player.RemoveModifier(x));
    }

    public int PerformInteraction(PlayerControl visitor, PlayerControl target)
    {
        Warlock.RpcNotify(Caster, visitor, target, (int)NotificationType.Warlock_TargetVisited);
        visitor.RpcRemoveModifier<WarlockFramedModifier>();
        target.RpcAddModifier<WarlockFramedModifier>(Caster);

        return 0; // Do not Block visit.
    }
}