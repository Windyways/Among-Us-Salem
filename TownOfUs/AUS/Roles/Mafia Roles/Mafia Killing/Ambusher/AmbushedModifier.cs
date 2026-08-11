using System.Collections;
using UnityEngine;

namespace AmongUsSalem.Modifiers;

public sealed class AmbushedModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Ambushed";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
    public override void OnActivate()
    {
        if (!Caster.HasModifier<OverchargedModifier>())
        {
            var player = ModifierUtils.GetPlayersWithModifier<AmbushedModifier>(x => x.Caster == Caster && x != this).FirstOrDefault();
            player?.RpcRemoveModifier<AmbushedModifier>();
        }
    }

    public enum State { KillVisitor, KilledVisitor }
    public State currentState;

    public override void OnMeetingStart()
    {
        Player.GetModifiers<AmbushedModifier>().Do(x => Player.RemoveModifier(x));
    }

    public IEnumerator CoPerformInteraction(PlayerControl visitor)
    {
        yield return new WaitForSeconds(0.1f);
        PerformInteraction(visitor);
    }

    public int PerformInteraction(PlayerControl visitor)
    {
        if (currentState == State.KillVisitor)
        {
            if (Caster.CanKill(visitor))
            {
                Caster.RpcAddModifier<InvisibleStatus>();
                Caster.RpcCustomMurder(visitor, teleportMurderer: !MeetingHud.Instance);
                VisitingMechanic.RpcAddDeathReason(visitor, (int)DeathReasonShow.KilledByAnAmbusher);
            }

            Ambusher.RpcNotify(Caster, (int)NotificationType.Ambusher_Kill, Player, Player);
            currentState = State.KilledVisitor;
        }

        Caster?.GetTrueRole<Ambusher>().RevealAmbusher(visitor, Player);
        return 0; // Do not Block Visit.
    }
}