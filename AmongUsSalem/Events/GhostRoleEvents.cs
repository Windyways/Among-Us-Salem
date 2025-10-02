using System.Collections;
using AmongUs.GameOptions;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Usables;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using Reactor.Utilities;
using AmongUsSalem.Modifiers.Game;
using AmongUsSalem.Roles;
using AmongUsSalem.Roles.Neutral;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Events;

public static class GhostRoleEvents
{
    [RegisterEvent]
    public static void PlayerCanUseEventHandler(PlayerCanUseEvent @event)
    {
        if (!PlayerControl.LocalPlayer || !PlayerControl.LocalPlayer.Data ||
            !PlayerControl.LocalPlayer.Data.Role)
        {
            return;
        }

        if (!@event.Usable.TryCast<Console>() ||
            PlayerControl.LocalPlayer.Data.Role is not IGhostRole { GhostActive: false })
        {
            return;
        }

        @event.Cancel();
    }

    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            return;
        }

        Coroutines.Start(SpawnCoroutine());
    }

    private static IEnumerator SpawnCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        foreach (var ghost in CustomRoleUtils.GetActiveRoles().OfType<IGhostRole>())
        {
            if (ghost.Caught)
            {
                continue;
            }

            ghost.Spawn();
        }
    }
}