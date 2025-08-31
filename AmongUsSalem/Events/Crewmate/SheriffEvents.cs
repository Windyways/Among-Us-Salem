using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using AmongUsSalem.Buttons.Crewmate;
using AmongUsSalem.Modifiers.Game;
using AmongUsSalem.Modules;
using AmongUsSalem.Options.Roles.Crewmate;
using AmongUsSalem.Roles;
using AmongUsSalem.Roles.Crewmate;
using AmongUsSalem.Utilities;

namespace AmongUsSalem.Events.Neutral;

public static class SheriffEvents
{
    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            CustomButtonSingleton<SheriffShootButton>.Instance.FailedShot = false;
        }
        else if (PlayerControl.LocalPlayer.Data.Role is SheriffRole)
        {
            SheriffRole.OnRoundStart();
        }
    }

    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;

        if (source.Data.Role is not SheriffRole)
        {
            return;
        }

        if (source.TryGetModifier<AllianceGameModifier>(out var allyMod2) && !allyMod2.GetsPunished)
        {
            return;
        }

        var target = @event.Target;
        var options = OptionGroupSingleton<SheriffOptions>.Instance;

        if (GameHistory.PlayerStats.TryGetValue(source.PlayerId, out var stats))
        {
            if (target.IsImpostor() ||
                (target.IsCrewmate() && target.TryGetModifier<AllianceGameModifier>(out var allyMod) &&
                 !allyMod.GetsPunished) ||
                (target.Is(Alignment.NeutralEvil) && options.ShootNeutralEvil) ||
                (target.Is(Alignment.NeutralKilling) && options.ShootNeutralKiller))
            {
                stats.CorrectKills += 1;
            }
            else
            {
                stats.IncorrectKills += 1;
            }
        }
    }
}