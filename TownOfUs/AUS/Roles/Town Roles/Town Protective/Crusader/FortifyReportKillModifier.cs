namespace AmongUsSalem.Modifiers;

public sealed class FortifyReportKillModifier(PlayerControl c, PlayerControl t) : BaseModifier
{
    public override string ModifierName => "Fortify Report Kill";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
    public PlayerControl Target => t;
    public override void OnActivate()
    {
        if (Debugger.IsDebuggerActive)
        {
            if (Caster.CanKill(Player))
            {
                Caster.RpcCustomMurder(Player, teleportMurderer: false);
                VisitingMechanic.RpcAddDeathReason(Player, (int)DeathReasonShow.KilledByACrusader);
                if (AmongUsClient.Instance.AmHost) DayNightMechanic.StartDayOne(PlayerControl.LocalPlayer);
            }

            Crusader.RpcNotify(Caster, (int)NotificationType.Crusader_AttackedVisitor, Target);

            Player.RpcRemoveModifier<FortifyReportKillModifier>();
        }
    }

    public override void OnMeetingStart()
    {
        if (Caster.CanKill(Player))
        {
            Caster.RpcCustomMurder(Player, teleportMurderer: false);
            VisitingMechanic.RpcAddDeathReason(Player, (int)DeathReasonShow.KilledByACrusader);
        }

        Crusader.RpcNotify(Caster, (int)NotificationType.Crusader_AttackedVisitor, Target);

        Player.RpcRemoveModifier<FortifyReportKillModifier>();
    }
}