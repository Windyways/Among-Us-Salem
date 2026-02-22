namespace AmongUsSalem.MafiaRoles;

public static class MafiosoPromotionMechanic
{
    [RegisterEvent]
    public static void RoundStartEvent(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            return; // Only run when round starts.
        }

        PromoteMafia();
    }

    [RegisterEvent]
    public static void StartMeetingEvent(StartMeetingEvent @event)
    {
        PromoteMafia();
    }

    public static void PromoteMafia()
    {
        var aliveMafiosos = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.IsRole<Mafioso>());
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (aliveMafiosos == 0 && player.Is(Faction.Mafia))
            {
                if (player.AmOwner())
                {
                    player.RpcChangeRole(RoleId.Get<Mafioso>());
                    player.Notify(Mafioso.Info(), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.MafiosoRoleCard.LoadAsset());
                }

                break;
            }
        }
    }
}