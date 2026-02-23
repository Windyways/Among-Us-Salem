namespace AmongUsSalem.Modifiers;

public sealed class JinxReportKillModifier(PlayerControl c, PlayerControl t) : BaseModifier
{
    public override string ModifierName => "Jinx Report Kill";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
    public PlayerControl Target => t;
    public override void OnMeetingStart()
    {
        if (Caster.CanKill(Player))
        {
            Caster.RpcCustomMurder(Player);
            VisitingMechanic.RpcAddDeathReason(Player, (int)DeathReasonShow.KilledByAJinx);
        }

        Jinx.RpcNotify(Caster, (int)NotificationType.Jinx_JinxedVisitor, Player, Player);
        Caster?.GetTrueRole<Jinx>().RevealJinx(Player, Target);

        Player.RpcRemoveModifier<JinxedModifier>();
    }
}