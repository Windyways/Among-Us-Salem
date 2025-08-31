using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using Reactor.Utilities;
using AmongUsSalem.Modifiers.Game.Universal;
using AmongUsSalem.Utilities;

namespace AmongUsSalem.Events.Modifiers;

public static class SixthSenseEvents
{
    [RegisterEvent]
    public static void MiraButtonClickEventHandler(MiraButtonClickEvent @event)
    {
        // Logger<AUSPlugin>.Warning("SixthSense click event!");
        if (MeetingHud.Instance || ExileController.Instance)
        {
            return;
        }

        var button = @event.Button as CustomActionButton<PlayerControl>;
        var target = button?.Target;

        if (target == null || button == null || !button.CanClick())
        {
            return;
        }

        CheckForSixthSense(target);
    }

    [RegisterEvent]
    public static void KillButtonClickEventHandler(BeforeMurderEvent @event)
    {
        CheckForSixthSense(@event.Target);
        // I am groot (i am stupid)
    }

    [MethodRpc((uint)AUSRpc.TriggerSixthSense, SendImmediately = true, LocalHandling = RpcLocalHandling.None)]
    private static void CheckForSixthSense(PlayerControl target)
    {
        if (target.AmOwner && target.HasModifier<SixthSenseModifier>())
        {
            Coroutines.Start(MiscUtils.CoFlash(AUSColors.SixthSense));
        }
    }
}