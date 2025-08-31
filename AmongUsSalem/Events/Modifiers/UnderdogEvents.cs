using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Modifiers;
using AmongUsSalem.Modifiers.Game.Impostor;
using AmongUsSalem.Roles.Impostor;
using AmongUsSalem.Utilities;

namespace AmongUsSalem.Events.Modifiers;

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