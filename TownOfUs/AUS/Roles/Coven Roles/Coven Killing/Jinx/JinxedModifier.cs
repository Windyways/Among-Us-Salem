using static UnityEngine.GraphicsBuffer;

namespace AmongUsSalem.Modifiers;

public sealed class JinxedModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Jinxed";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
    public override void OnActivate()
    {
        if (!Caster.HasModifier<OverchargedModifier>())
        {
            var player = ModifierUtils.GetPlayersWithModifier<JinxedModifier>(x => x.Caster == Caster && x != this).FirstOrDefault();
            player?.RpcRemoveModifier<JinxedModifier>();
        }
    }

    public enum State { KillVisitor, KilledVisitor }
    public State currentState;

    public override void OnMeetingStart()
    {
        Player.GetModifiers<JinxedModifier>().Do(x => Player.RemoveModifier(x));
    }

    public int PerformInteraction(PlayerControl visitor)
    {
        if (currentState == State.KillVisitor)
        {
            if (Caster.CanKill(visitor))
            {
                Caster.RpcAddModifier<InvisibleStatus>();
                Caster.RpcCustomMurder(visitor);
                VisitingMechanic.RpcAddDeathReason(visitor, (int)DeathReasonShow.KilledByAJinx);
            }

            Jinx.RpcNotify(Caster, (int)NotificationType.Jinx_JinxedVisitor, Player, Player);
            currentState = State.KilledVisitor;
        }

        Caster?.GetTrueRole<Jinx>().RevealJinx(visitor, Player);
        return 0; // Do not Block Visit.
    }
}