using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Usables;
using ObjectWorkshop.Roles.Neutral;

namespace ObjectWorkshop.Events.Neutral;

public static class WerewolfEvents
{
    [RegisterEvent]
    public static void PlayerCanUseEventHandler(PlayerCanUseEvent @event)
    {
        if (!@event.IsVent)
        {
            return;
        }

        if (!PlayerControl.LocalPlayer)
        {
            return;
        }

        var vent = @event.Usable.TryCast<Vent>();

        if (vent == null)
        {
            return;
        }

        if (!PlayerControl.LocalPlayer.inVent && PlayerControl.LocalPlayer.Data.Role is WerewolfRole werewolf &&
            !werewolf.Rampaging)
        {
            @event.Cancel();
        }
    }
}