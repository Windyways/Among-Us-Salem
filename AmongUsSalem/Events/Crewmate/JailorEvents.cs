using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using ObjectWorkshop.Modifiers.Crewmate;
using ObjectWorkshop.Modifiers.Game;
using ObjectWorkshop.Modules;
using ObjectWorkshop.Options.Roles.Crewmate;
using ObjectWorkshop.Roles.Crewmate;
using ObjectWorkshop.Utilities;

namespace ObjectWorkshop.Events.Crewmate;

public static class JailorEvents
{
    [RegisterEvent]
    public static void EjectionEventHandler(EjectionEvent @event)
    {
        var sparedPlayers = ModifierUtils.GetPlayersWithModifier<JailSparedModifier>().ToList();
        sparedPlayers.Do(x => x.RemoveModifier<JailSparedModifier>());

        var players = ModifierUtils.GetPlayersWithModifier<JailedModifier>().ToList();
        if (!OptionGroupSingleton<JailorOptions>.Instance.JailInARow)
        {
            players.Do(x => x.AddModifier<JailSparedModifier>(x.GetModifier<JailedModifier>()!.JailorId));
        }

        players.Do(x => x.RemoveModifier<JailedModifier>());
    }

    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;
        var victim = @event.Target;

        if ((victim.Data.Role is JailorRole || victim.GetRoleWhenAlive() is JailorRole) && !MeetingHud.Instance)
        {
            ModifierUtils.GetPlayersWithModifier<JailedModifier>().Do(x => x.RemoveModifier<JailedModifier>());
        }

        if (source.Data.Role is not JailorRole jailor)
        {
            return;
        }

        jailor.Executes--;

        if (source.TryGetModifier<AllianceGameModifier>(out var allyMod) && !allyMod.GetsPunished)
        {
            return;
        }

        var target = @event.Target;

        if (GameHistory.PlayerStats.TryGetValue(source.PlayerId, out var stats))
        {
            if (!target.IsCrewmate() ||
                (target.TryGetModifier<AllianceGameModifier>(out var allyMod2) && !allyMod2.GetsPunished))
            {
                stats.CorrectKills += 1;
            }
            else if (source != target)
            {
                jailor.Executes = 0;
                stats.IncorrectKills += 1;
            }
        }
    }
}