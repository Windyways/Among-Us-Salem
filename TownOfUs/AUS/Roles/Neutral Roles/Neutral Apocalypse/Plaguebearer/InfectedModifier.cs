namespace AmongUsSalem.Modifiers;

public sealed class InfectedModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Infected";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
    public override void OnMeetingStart()
    {
        if (Player.HasDied())
            Player.RpcRemoveModifier<InfectedModifier>();
    }

    public int PerformInteraction(PlayerControl visitor, PlayerControl target)
    {
        if (visitor == Caster)
            return 0;

        if (visitor.HasModifier<InfectedModifier>())
        {
            Plaguebearer.RpcNotify(Caster, visitor, target, (int)NotificationType.Plaguebearer_SpreadPlague);
            if (!target.HasModifier<InfectedModifier>()) target.RpcAddModifier<InfectedModifier>(Caster);
        }

        if ((target.HasModifier<InfectedModifier>() || target.IsRole<Plaguebearer>()) && !visitor.HasModifier<InfectedModifier>())
        {
            visitor.RpcAddModifier<InfectedModifier>(Caster);
        }

        return 0; // Do not Block visit.
    }
}