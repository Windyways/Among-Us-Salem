using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using AmongUsSalem.Modifiers.Impostor;
using AmongUsSalem.Options.Roles.Impostor;

namespace AmongUsSalem.Events.Impostor;

public static class BlackmailerEvents
{
    [RegisterEvent]
    public static void EjectionEvent(EjectionEvent @event)
    {
        var sparedPlayers = ModifierUtils.GetPlayersWithModifier<BlackmailSparedModifier>().ToList();
        sparedPlayers.Do(x => x.RemoveModifier<BlackmailSparedModifier>());

        var players = ModifierUtils.GetPlayersWithModifier<BlackmailedModifier>().ToList();
        if (!OptionGroupSingleton<BlackmailerOptions>.Instance.BlackmailInARow)
        {
            players.Do(x =>
                x.AddModifier<BlackmailSparedModifier>(x.GetModifier<BlackmailedModifier>()!.BlackMailerId));
        }

        players.Do(x => x.RemoveModifier<BlackmailedModifier>());
    }
}