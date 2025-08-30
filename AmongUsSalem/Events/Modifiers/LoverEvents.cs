using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Utilities;
using ObjectWorkshop.Modifiers;
using ObjectWorkshop.Modifiers.Game.Alliance;
using ObjectWorkshop.Options.Modifiers.Alliance;
using ObjectWorkshop.Utilities;

namespace ObjectWorkshop.Events.Modifiers;

public static class LoverEvents
{
    [RegisterEvent(400)]
    public static void PlayerDeathEventHandler(PlayerDeathEvent @event)
    {
        if (!PlayerControl.LocalPlayer.IsHost())
        {
            return;
        }
        if (@event.Player == null || !@event.Player.TryGetModifier<LoverModifier>(out var loveMod)
            || !OptionGroupSingleton<LoversOptions>.Instance.BothLoversDie || loveMod.OtherLover == null
            || loveMod.OtherLover.HasDied() || loveMod.OtherLover.HasModifier<InvulnerabilityModifier>())
        {
            return;
        }
        switch (@event.DeathReason)
        {
            case DeathReason.Exile:
                loveMod.OtherLover.RpcPlayerExile();
                DeathHandlerModifier.RpcUpdateDeathHandler(loveMod.OtherLover, "Heartbroken", DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse, lockInfo: DeathHandlerOverride.SetTrue);
                break;
            case DeathReason.Kill:
                loveMod.OtherLover.RpcCustomMurder(loveMod.OtherLover);
                DeathHandlerModifier.RpcUpdateDeathHandler(loveMod.OtherLover, "Heartbroken", DeathEventHandlers.CurrentRound,
                    (!MeetingHud.Instance && !ExileController.Instance) ? DeathHandlerOverride.SetTrue : DeathHandlerOverride.SetFalse, lockInfo: DeathHandlerOverride.SetTrue);
                break;
        }
    }

    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        ModifierUtils.GetActiveModifiers<LoverModifier>().Do(x => x.OnRoundStart());
    }
}