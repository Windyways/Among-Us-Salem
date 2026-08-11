namespace AmongUsSalem.Roles;

public static class Godfather_Events
{
    [RegisterEvent]
    public static void EjectionEvent(EjectionEvent @event)
    {
        NetworkedPlayerInfo exiled = @event.ExileController.initData.networkedPlayer;
        if (exiled != null)
        {
            PlayerControl player = exiled.Object;
            if (player.IsRole<Godfather>()) MafiosoPromotionMechanic.GodfatherDied = true;
        }
    }

    [RegisterEvent]
    public static void AfterMurderEvent(AfterMurderEvent @event)
    {
        var role = @event.Target.GetRoleWhenAlive();
        var aliveMafiosos = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.IsRole<Mafioso>());
        if (role is Godfather && aliveMafiosos > 0) MafiosoPromotionMechanic.GodfatherDied = true;
    }
}