using System.Collections;
using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.Modifiers;
using Reactor.Utilities;
using ObjectWorkshop.Events.TouEvents;
using ObjectWorkshop.Modifiers;
using ObjectWorkshop.Modules;
using ObjectWorkshop.Roles.Crewmate;
using ObjectWorkshop.Roles.Neutral;
using UnityEngine;

namespace ObjectWorkshop.Events;

public static class DeathEventHandlers
{
    public static bool IsDeathRecent { get; set; }
    public static IEnumerator CoWaitDeathHandler()
    {
        IsDeathRecent = true;
        yield return new WaitForSeconds(0.15f);
        IsDeathRecent = false;
    }
    public static int CurrentRound { get; set; } = 1;

    [RegisterEvent(-1)]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            CurrentRound = 1;
            Logger<ObjectWorkshopPlugin>.Warning("Game Has Started");
        }
        else
        {
            ++CurrentRound;
            ModifierUtils.GetActiveModifiers<DeathHandlerModifier>().Do(x => x.DiedThisRound = false);
            Logger<ObjectWorkshopPlugin>.Warning($"New Round Started: {CurrentRound}");
        }
    }

    [RegisterEvent(1000)]
    public static void PlayerDeathEventHandler(PlayerDeathEvent @event)
    {
        var victim = @event.Player;
        if (!victim.HasModifier<DeathHandlerModifier>())
        {
            var deathHandler = new DeathHandlerModifier();
            victim.AddModifier(deathHandler);
            var cod = "Disconnected";
            deathHandler.DiedThisRound = !MeetingHud.Instance && !ExileController.Instance;
            switch (@event.DeathReason)
            {
                case DeathReason.Exile:
                    cod = "Ejected";
                    deathHandler.DiedThisRound = false;
                    break;
                case DeathReason.Kill:
                    cod = "Killed";
                    break;
            }
            deathHandler.CauseOfDeath = cod;
            deathHandler.RoundOfDeath = CurrentRound;
            Coroutines.Start(CoWaitDeathHandler());
        }
    }
    
    [RegisterEvent(500)]
    public static void EjectionEventHandler(EjectionEvent @event)
    {
        var exiled = @event.ExileController?.initData?.networkedPlayer?.Object;
        if (exiled == null)
        {
            return;
        }
        if (!exiled.HasModifier<DeathHandlerModifier>())
        {
            DeathHandlerModifier.UpdateDeathHandler(exiled, "Ejected", CurrentRound, DeathHandlerOverride.SetFalse);
        }
    }

    [RegisterEvent(500)]
    public static void PlayerReviveEventHandler(PlayerReviveEvent reviveEvent)
    {
        reviveEvent.Player.RemoveModifier<DeathHandlerModifier>();
    }

    [RegisterEvent(500)]
    public static void AfterMurderEventHandler(AfterMurderEvent murderEvent)
    {
        var source = murderEvent.Source;
        var target = murderEvent.Target;
        
        if (target == source && target.TryGetModifier<DeathHandlerModifier>(out var deathHandler) && !deathHandler.LockInfo)
        {
            deathHandler.CauseOfDeath = "Suicide";
            deathHandler.DiedThisRound = !MeetingHud.Instance && !ExileController.Instance;
            deathHandler.RoundOfDeath = CurrentRound;
            deathHandler.LockInfo = true;
        }
        else if (target.TryGetModifier<DeathHandlerModifier>(out var deathHandler2) && !deathHandler2.LockInfo)
        {
            var cod = "Killed";
            switch (source.GetRoleWhenAlive())
            {
                case Pyre:
                    cod = "Incinerated";
                    break;
                case Duelist:
                    cod = "Dueled";
                    break;
                case Aimsman:
                    cod = "Shot";
                    break;
            }
            
            deathHandler2.CauseOfDeath = cod;
            deathHandler2.KilledBy = $"By {source.Data.PlayerName}";
            deathHandler2.DiedThisRound = !MeetingHud.Instance && !ExileController.Instance;
            deathHandler2.RoundOfDeath = CurrentRound;
        }
    }

    [RegisterEvent]
    public static void PlayerLeaveEventHandler(PlayerLeaveEvent @event)
    {
        if (!MeetingHud.Instance)
        {
            return;
        }

        var player = @event.ClientData.Character;

        if (!player)
        {
            return;
        }

        var pva = MeetingHud.Instance.playerStates.First(x => x.TargetPlayerId == player.PlayerId);

        if (!pva)
        {
            return;
        }

        pva.AmDead = true;
        pva.Overlay.gameObject.SetActive(true);
        pva.Overlay.color = Color.white;
        pva.XMark.gameObject.SetActive(false);
        pva.XMark.transform.localScale = Vector3.one;

        MeetingMenu.Instances.Do(x => x.HideSingle(player.PlayerId));
    }
}