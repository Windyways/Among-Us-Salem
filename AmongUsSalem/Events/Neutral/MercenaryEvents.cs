using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using AmongUsSalem.Modifiers.Neutral;
using AmongUsSalem.Roles.Neutral;
using AmongUsSalem.Utilities;

namespace AmongUsSalem.Events.Neutral;

public static class MercenaryEvents
{
    [RegisterEvent]
    public static void MiraButtonClickEventHandler(MiraButtonClickEvent @event)
    {
        var button = @event.Button as CustomActionButton<PlayerControl>;
        var source = PlayerControl.LocalPlayer;
        var target = button?.Target;

        if (target == null || button == null || !button.CanClick())
        {
            return;
        }

        // only check if this interaction was via a custom button
        CheckForMercenaryGuard(source, target);
    }

    [RegisterEvent]
    public static void BeforeMurderEventHandler(BeforeMurderEvent @event)
    {
        var source = @event.Source;
        var target = @event.Target;

        // only check if this interaction was via the standard kill button
        if (source.Data.Role is ICustomRole { Configuration.UseVanillaKillButton: true } ||
            (source.Data.Role is not ICustomRole && source.IsImpostor()))
        {
            CheckForMercenaryGuard(source, target);
        }
    }

    private static void CheckForMercenaryGuard(PlayerControl source, PlayerControl target)
    {
        if (MeetingHud.Instance || ExileController.Instance)
        {
            return;
        }

        if (!target.HasModifier<MercenaryGuardModifier>())
        {
            return;
        }

        var mercenary = target.GetModifier<MercenaryGuardModifier>()?.Mercenary;

        if (mercenary && source.AmOwner)
        {
            MercenaryRole.RpcGuarded(mercenary!);
        }
    }
}