using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using ObjectWorkshop.Modifiers.Impostor;
using ObjectWorkshop.Roles.Impostor;

namespace ObjectWorkshop.Events.Impostor;

public static class HypnotistEvents
{
    [RegisterEvent]
    private static void StartMeetingEventHandler(StartMeetingEvent @event)
    {
        foreach (var mod in ModifierUtils.GetActiveModifiers<HypnotisedModifier>())
        {
            mod.UnHysteria();
        }
    }

    [RegisterEvent]
    public static void EjectionEventHandler(EjectionEvent @event)
    {
        foreach (var hypnotist in CustomRoleUtils.GetActiveRolesOfType<HypnotistRole>())
        {
            if (!hypnotist.HysteriaActive)
            {
                continue;
            }

            var mods = ModifierUtils.GetActiveModifiers<HypnotisedModifier>(x => x.Hypnotist == hypnotist.Player);
            mods.Do(x => x.Hysteria());
        }
    }

    [RegisterEvent]
    public static void HypnotistDeathHandler(PlayerDeathEvent @event)
    {
        if (@event.Player.Data.Role is HypnotistRole hypno)
        {
            ModifierUtils.GetPlayersWithModifier<HypnotisedModifier>(x => x.Hypnotist == hypno)
                         .Do(x => x.RemoveModifier<HypnotisedModifier>());
        }
    }

    /* whenever the event gets changed to having the character not being null
    [RegisterEvent]
    public static void HypnotistDisconnectHandler(PlayerLeaveEvent @event)
    {
        if (@event.ClientData.Character.Data.Role is HypnotistRole hypno)
        {
            ModifierUtils.GetPlayersWithModifier<HypnotisedModifier>(x => x.Hypnotist == hypno)
                         .Do(x => x.RemoveModifier<HypnotisedModifier>());
        }
    }*/
}