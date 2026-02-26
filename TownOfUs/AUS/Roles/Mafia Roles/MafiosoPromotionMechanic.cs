namespace AmongUsSalem.MafiaRoles;

public static class MafiosoPromotionMechanic
{
    public static bool GodfatherDied;

    [RegisterEvent]
    public static void RoundStartEvent(RoundStartEvent @event)
    {
        if (SmartProsecutor.IsActive)
            return;

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
        var totalGodfathers = PlayerControl.AllPlayerControls.ToArray().Count(x => x.IsTrueRole<Godfather>());
        var aliveGodfathers = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.IsRole<Godfather>());
        var aliveMafiosos = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.IsRole<Mafioso>());
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (aliveGodfathers == 0 && aliveMafiosos == 0 && player.Is(Faction.Mafia) && !player.HasDied())
            {
                if (player.AmOwner())
                {
                    player.RpcChangeRole(RoleId.Get<Mafioso>());
                    player.Notify(Mafioso.Info(), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.MafiosoRoleCard.LoadAsset());
                }

                break;
            }
            else if (aliveGodfathers == 0 && aliveMafiosos > 0 && player.IsRole<Mafioso>() && GodfatherDied && !player.HasDied() && totalGodfathers < 2)
            {
                GodfatherDied = false;
                if (player.AmOwner())
                {
                    player.RpcChangeRole(RoleId.Get<Godfather>());
                    player.Notify(Godfather.Info(), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.GodfatherRoleCard.LoadAsset());
                }

                break;
            }
        }
    }
}