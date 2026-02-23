namespace AmongUsSalem.Modifiers;

public sealed class GuardedModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Guarded";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public Bodyguard bodyguard => Caster?.Data?.Role as Bodyguard;
    public PlayerControl Caster => c;
    public override void OnActivate()
    {
        if (!Caster.HasModifier<OverchargedModifier>())
        {
            var player = ModifierUtils.GetPlayersWithModifier<GuardedModifier>(x => x.Caster == Caster).FirstOrDefault();
            player?.RpcRemoveModifier<GuardedModifier>();
        }
    }

    public override void OnMeetingStart()
    {
        Player.RpcRemoveModifier<GuardedModifier>();
    }


    public int PerformInteraction(PlayerControl attacker, PlayerControl target)
    {
        bool bgKills = false;
        bool aKills = false;
        if (Caster.CanKill(attacker)) bgKills = true;
        if (attacker.CanKill(Caster)) aKills = true;

        Bodyguard.RpcNotify(target, (int)NotificationType.Bodyguard_Protect);
        target.RpcRemoveModifier<GuardedModifier>();

        if (bgKills)
        {
            Caster.RpcAddModifier<InvisibleStatus>();
            Caster.RpcCustomMurder(attacker);
            VisitingMechanic.RpcAddDeathReason(attacker, (int)DeathReasonShow.KilledByABodyguard);
        }
        if (aKills)
        {
            attacker.RpcCustomMurder(Caster);
            VisitingMechanic.RpcAddDeathReason(Caster, (int)DeathReasonShow.DiedWhileDefendingTheirTarget);
        }

        return 1; // Block visit.
    }
}