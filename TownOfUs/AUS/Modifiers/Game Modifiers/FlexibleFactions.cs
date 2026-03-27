using UnityEngine;

namespace AmongUsSalem.Modifiers;

public static class FlexibleFactions
{
    // Faction, Color, Team
    public static (Faction, Color, ModdedRoleTeams) GetNewFaction(int factionKey)
    {
        var faction = (Faction)factionKey;
        if (faction == Faction.Mafia) return (faction, RoleColors.Mafia, ModdedRoleTeams.Impostor);
        if (faction == Faction.Coven) return (faction, RoleColors.Coven, ModdedRoleTeams.Custom);
        if (faction == Faction.Apocalypse) return (faction, RoleColors.Apocalypse, ModdedRoleTeams.Custom);
        if (faction == Faction.SerialKiller) return (faction, RoleColors.SerialKiller(), ModdedRoleTeams.Custom);
        if (faction == Faction.Werewolf) return (faction, RoleColors.Werewolf, ModdedRoleTeams.Custom);
        return (faction, RoleColors.Town, ModdedRoleTeams.Crewmate);
    }

    public static bool GetWinConditionMet(RoleBehaviour role, Faction faction)
    {
        if (faction == Faction.Coven) return CovenGameOver.WinConditionMet(role);
        if (faction == Faction.Apocalypse) return ApocGameOver.WinConditionMet(role);
        return false;
    }

    public static bool GetDidWin(Faction faction, GameOverReason gameOverReason)
    {
        if (faction == Faction.Coven) return CovenGameOver.AnyCovenWon(gameOverReason);
        if (faction == Faction.Apocalypse) return ApocGameOver.AnyApocWon(gameOverReason);
        return false;
    }
}