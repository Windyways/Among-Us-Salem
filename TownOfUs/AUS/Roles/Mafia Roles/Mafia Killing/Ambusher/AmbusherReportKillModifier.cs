namespace AmongUsSalem.Modifiers;

public sealed class AmbusherReportKillModifier(PlayerControl c, PlayerControl t) : BaseModifier
{
    public override string ModifierName => "Ambusher Report Kill";
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
                VisitingMechanic.RpcAddDeathReason(Player, (int)DeathReasonShow.KilledByAnAmbusher);
                //DayNightMechanic.StartDayOne(PlayerControl.LocalPlayer);
            }

            Ambusher.RpcNotify(Caster, (int)NotificationType.Ambusher_Kill, Player, Player);
            Caster?.GetTrueRole<Ambusher>().RevealAmbusher(Player, Target);

            Player.RpcRemoveModifier<AmbusherReportKillModifier>();
        }
    }

    public override void OnMeetingStart()
    {
        if (Caster.CanKill(Player))
        {
            Caster.RpcCustomMurder(Player, teleportMurderer: false);
            VisitingMechanic.RpcAddDeathReason(Player, (int)DeathReasonShow.KilledByAnAmbusher);
        }

        if (Caster.AmOwner()) Ambusher.RpcNotify(Caster, (int)NotificationType.Ambusher_Kill, Player, Player);
        Caster?.GetTrueRole<Ambusher>().RevealAmbusher(Player, Target);

        Player.RpcRemoveModifier<AmbusherReportKillModifier>();
    }
}