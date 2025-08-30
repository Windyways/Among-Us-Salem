using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.GameEnd;
using MiraAPI.Roles;
using Reactor.Utilities.Extensions;
using ObjectWorkshop.GameOver;
using ObjectWorkshop.Modules;
using ObjectWorkshop.Patches;
using ObjectWorkshop.Roles;

namespace ObjectWorkshop.Events;

public static class EndGameEvents
{
    public static int winType;

    [RegisterEvent(-100)]
    public static void OnGameEnd(GameEndEvent @event)
    {
        winType = 0;
        var reason = EndGameResult.CachedGameOverReason;
        var neutralWinner = CustomRoleUtils.GetActiveRolesOfTeam(ModdedRoleTeams.Custom).Any(x => x is IOWRole role && role.WinConditionMet());

        if (neutralWinner)
        {
            FactionReferences.UpdateFactionResult("Crewmate", false);
            FactionReferences.UpdateFactionResult("Neutral", true);
            FactionReferences.UpdateFactionResult("Infiltrator", false);
            return;
        }

        if (reason is GameOverReason.CrewmatesByVote or GameOverReason.CrewmatesByTask
            or GameOverReason.ImpostorDisconnect)
        {
            winType = 1;
            GameHistory.WinningFaction = $"<color=#{OWColors.Crewmate.ToHtmlStringRGBA()}>Crewmates</color>";
            FactionReferences.UpdateFactionResult("Crewmate", true);
            FactionReferences.UpdateFactionResult("Neutral", false);
            FactionReferences.UpdateFactionResult("Infiltrator", false);
        }
        else if (reason is GameOverReason.ImpostorsByKill or GameOverReason.ImpostorsBySabotage
                 or GameOverReason.ImpostorsByVote or GameOverReason.CrewmateDisconnect)
        {
            winType = 2;
            GameHistory.WinningFaction = $"<color=#{OWColors.Infiltrator.ToHtmlStringRGBA()}>Infiltrators</color>";
            FactionReferences.UpdateFactionResult("Crewmate", false);
            FactionReferences.UpdateFactionResult("Neutral", false);
            FactionReferences.UpdateFactionResult("Infiltrator", true);
        }

        if (reason == CustomGameOver.GameOverReason<DrawGameOver>())
        {
            winType = 0;
        }
    }

    [RegisterEvent]
    public static void GameEndEventHandler(GameEndEvent @event)
    {
        EndGamePatches.BuildEndGameSummary(@event.EndGameManager);
    }
}