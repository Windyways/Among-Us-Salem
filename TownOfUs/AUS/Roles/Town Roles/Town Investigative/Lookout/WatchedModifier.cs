namespace AmongUsSalem.Modifiers;

public sealed class WatchedModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Watched";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
    public override void OnActivate()
    {
        if (!Caster.HasModifier<OverchargedModifier>())
        {
            var player = ModifierUtils.GetPlayersWithModifier<WatchedModifier>(x => x.Caster == Caster && x != this).FirstOrDefault();
            player?.RpcRemoveModifier<WatchedModifier>();
        }
    }

    public override void OnMeetingStart()
    {
        Player.GetModifiers<WatchedModifier>().Do(x => Player.RemoveModifier(x));
    }

    [MethodRpc((uint)AUSRpc.RpcPerformInteraction)]
    public static int RpcPerformInteraction(PlayerControl Caster, PlayerControl visitor, PlayerControl target)
    {
        if (Caster != visitor)
        {
            if (Caster.Data.Role is Lookout lookout)
            {
                var opt = OptionGroupSingleton<Lookout_Options>.Instance;
                if (lookout.VisitedInfo.Count >= (int)opt.MaxRecognize.GetFloatData() && opt.Mode == LookoutMode.TOS1)
                {
                    lookout.Player.Notify(Lookout.Info(NotificationType.Lookout_MoreThan3, visitor, target), NotifyMode.OnlyMeeting, sprite: AUSAssets.LookoutRoleCard.LoadAsset());
                    target.RpcRemoveModifier<WatchedModifier>();
                }
                else lookout.VisitedInfo.Add((visitor, target));
            }
            if (Caster.Data.Role is Agent agent) agent.LookoutVisitedInfo.Add((visitor, target));
            if (Caster.Data.Role is Wildling wildling) wildling.LookoutVisitedInfo.Add((visitor, target));
        }

        return 0; // Don't block visit.
    }
}