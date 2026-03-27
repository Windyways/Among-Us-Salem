using System.Collections;
using UnityEngine;

namespace AmongUsSalem.Modifiers;

public sealed class FortifiedModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Fortified";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
    public override void OnActivate()
    {
        if (!Caster.HasModifier<OverchargedModifier>())
        {
            var player = ModifierUtils.GetPlayersWithModifier<FortifiedModifier>(x => x.Caster == Caster && x != this).FirstOrDefault();
            player?.RpcRemoveModifier<FortifiedModifier>();
        }
    }

    public override void OnMeetingStart()
    {
        Player.GetModifiers<FortifiedModifier>().Do(x => Player.RemoveModifier(x));
    }

    public IEnumerator CoPerformInteraction(PlayerControl attacker, PlayerControl target, bool isAttacking)
    {
        yield return new WaitForSeconds(0.1f);
        PerformInteraction(attacker, target, isAttacking);
    }

    public int PerformInteraction(PlayerControl attacker, PlayerControl target, bool isAttacking)
    {
        if (Caster.CanKill(attacker))
        {
            Caster.RpcAddModifier<InvisibleStatus>();
            Caster.RpcCustomMurder(attacker);
            VisitingMechanic.RpcAddDeathReason(attacker, (int)DeathReasonShow.KilledByACrusader);
        }

        if (isAttacking) Crusader.RpcNotify(target, (int)NotificationType.Crusader_AttackAndFortified, target);
        Crusader.RpcNotify(Caster, (int)NotificationType.Crusader_AttackedVisitor, target);

        target.RpcRemoveModifier<FortifiedModifier>();

        return 0; // Don't Block visit.
    }
}