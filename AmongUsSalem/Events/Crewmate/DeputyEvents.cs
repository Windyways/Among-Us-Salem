using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Utilities;
using AmongUsSalem.Modifiers.Crewmate;
using AmongUsSalem.Modifiers.Game;
using AmongUsSalem.Modules;
using AmongUsSalem.Roles.Crewmate;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Events.Crewmate;

public static class DeputyEvents
{
    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        if (PlayerControl.LocalPlayer.Data.Role is DeputyRole)
        {
            DeputyRole.OnRoundStart();
        }
        foreach (var dep in CustomRoleUtils.GetActiveRolesOfType<DeputyRole>())
        {
            dep.Killer = null;
        }
    }

    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;
        var target = @event.Target;

        CheckForDeputyCamped(source, target);

        if (source.Data.Role is not DeputyRole)
        {
            return;
        }

        if (source.TryGetModifier<AllianceGameModifier>(out var allyMod) && !allyMod.GetsPunished)
        {
            return;
        }

        if (GameHistory.PlayerStats.TryGetValue(source.PlayerId, out var stats))
        {
            if (!target.IsCrewmate() ||
                (target.TryGetModifier<AllianceGameModifier>(out var allyMod2) && !allyMod2.GetsPunished))
            {
                stats.CorrectKills += 1;
            }
            else if (source != target)
            {
                stats.IncorrectKills += 1;
            }
        }
    }

    private static void CheckForDeputyCamped(PlayerControl source, PlayerControl target)
    {
        if (MeetingHud.Instance || ExileController.Instance)
        {
            return;
        }

        if (!target.HasModifier<DeputyCampedModifier>())
        {
            return;
        }

        var mod = target.GetModifier<DeputyCampedModifier>();

        if (mod == null)
        {
            return;
        }

        if (mod.Deputy.HasDied())
        {
            return;
        }

        if (mod.Deputy.Data.Role is not DeputyRole deputy)
        {
            return;
        }

        deputy.Killer = source;
        if (mod.Deputy.AmOwner)
        {
            var notif1 = Helpers.CreateAndShowNotification(
                $"<b>{AUSColors.Deputy.ToTextColor()}Your camped target, {target.Data.PlayerName}, has died! Avenge them in the meeting.</color></b>",
                Color.white, spr: TouRoleIcons.Deputy.LoadAsset());

            notif1.Text.SetOutlineThickness(0.35f);
            notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
            Coroutines.Start(MiscUtils.CoFlash(AUSColors.Deputy));
        }
    }
}