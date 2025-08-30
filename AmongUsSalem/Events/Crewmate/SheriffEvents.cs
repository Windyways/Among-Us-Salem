using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using ObjectWorkshop.Buttons.Crewmate;
using ObjectWorkshop.Modifiers.Game;
using ObjectWorkshop.Modules;
using ObjectWorkshop.Options.Roles.Crewmate;
using ObjectWorkshop.Roles;
using ObjectWorkshop.Roles.Crewmate;
using ObjectWorkshop.Utilities;

namespace ObjectWorkshop.Events.Neutral;

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
                (target.Is(RoleAlignment.NeutralEvil) && options.ShootNeutralEvil) ||
                (target.Is(RoleAlignment.NeutralPredator) && options.ShootNeutralKiller))
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