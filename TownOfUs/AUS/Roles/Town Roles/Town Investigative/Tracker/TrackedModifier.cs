using static UnityEngine.GraphicsBuffer;

namespace AmongUsSalem.Modifiers;

public sealed class TrackedModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Tracked";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
    public bool isPermanent => Caster.IsRole<Werewolf>();
    public override void OnActivate()
    {
        if (!Caster.HasModifier<OverchargedModifier>() && !isPermanent)
        {
            var player = ModifierUtils.GetPlayersWithModifier<TrackedModifier>(x => x.Caster == Caster && x != this).FirstOrDefault();
            player?.RpcRemoveModifier<TrackedModifier>();
        }
    }

    public override void OnMeetingStart()
    {
        if (!isPermanent)
            Player.GetModifiers<TrackedModifier>().Do(x => Player.RemoveModifier(x));
    }

    [MethodRpc((uint)AUSRpc.RpcPerformInteractionTracked)]
    public static int RpcPerformInteraction(PlayerControl Caster, PlayerControl visitor, PlayerControl target)
    {
        if (visitor.Data.Role is Seer) return 0; // Ignore double target visits, this will be handled elsewhere.

        if (Caster.Data.Role is Tracker tracker) tracker.VisitedInfo.Add((visitor, target));
        if (Caster.Data.Role is Agent agent) agent.TrackerVisitedInfo.Add((visitor, target));
        if (Caster.Data.Role is Wildling wildling) wildling.TrackerVisitedInfo.Add((visitor, target));
        if (Caster.Data.Role is Werewolf werewolf) werewolf.VisitedInfo.Add((visitor, target));

        return 0; // Don't block visit.
    }

    [MethodRpc((uint)AUSRpc.RpcPerformDoubleInteraction)]
    public static void RpcPerformDoubleInteraction(PlayerControl Caster, PlayerControl visitor, PlayerControl target1, PlayerControl target2)
    {
        if (Caster.Data.Role is Tracker tracker) tracker.DoubleVisitedInfo.Add((visitor, (target1, target2)));
        if (Caster.Data.Role is Agent agent) agent.TrackerDoubleVisitedInfo.Add((visitor, (target1, target2)));
        if (Caster.Data.Role is Wildling wildling) wildling.TrackerDoubleVisitedInfo.Add((visitor, (target1, target2)));
        if (Caster.Data.Role is Werewolf werewolf) werewolf.DoubleVisitedInfo.Add((visitor, (target1, target2)));
    }
}