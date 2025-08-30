using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Modifiers;
using ObjectWorkshop.Modifiers.Game.Impostor;
using ObjectWorkshop.Roles.Impostor;
using ObjectWorkshop.Utilities;

namespace ObjectWorkshop.Events.Modifiers;

public static class UnderdogEvents
{
    [RegisterEvent(1)]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;

        // Scavenger already handles it's own Kill timer
        if (!source.HasModifier<UnderdogModifier>() || source.IsRole<ScavengerRole>())
        {
            return;
        }

        source.SetKillTimer(source.GetKillCooldown());
    }

    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (!PlayerControl.LocalPlayer.HasModifier<UnderdogModifier>() ||
            PlayerControl.LocalPlayer.IsRole<ScavengerRole>())
        {
            return;
        }

        PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.LocalPlayer.GetKillCooldown());
    }
}