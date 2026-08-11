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
            var player = ModifierUtils.GetPlayersWithModifier<GuardedModifier>(x => x.Caster == Caster && x != this).FirstOrDefault();
            player?.RpcRemoveModifier<GuardedModifier>();
        }
    }

    public override void OnMeetingStart()
    {
        Player.GetModifiers<GuardedModifier>().Do(x => Player.RemoveModifier(x));
    }

    public int PerformInteraction(PlayerControl attacker, PlayerControl target)
    {
        bool bgKills = false;
        bool aKills = false;
        if (Caster.CanKill(attacker)) bgKills = true;
        if (attacker.CanKill(Caster)) aKills = true;

        Caster.AddModifier<Confirmed>();
        Bodyguard.RpcNotify(target, (int)NotificationType.Bodyguard_Protect);

        if (bgKills)
        {
            Caster.RpcAddModifier<InvisibleStatus>();
            Caster.RpcCustomMurder(attacker);
            VisitingMechanic.RpcAddDeathReason(attacker, (int)DeathReasonShow.KilledByABodyguard);
        }
        if (aKills)
        {
            attacker.RpcCustomMurder(Caster, teleportMurderer: attacker.HasDied());
            VisitingMechanic.RpcAddDeathReason(Caster, (int)DeathReasonShow.DiedWhileDefendingTheirTarget);
        }

        target.RpcRemoveModifier<GuardedModifier>();
        return 1; // Block visit.
    }
}