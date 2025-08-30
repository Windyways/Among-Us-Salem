using System;
using System.Collections;
using Il2CppSystem.Runtime.InteropServices;
using UnityEngine;

namespace ObjectWorkshop.LifeImprovement;

public static class MessageTexts
{
    public static string RevealRole(PlayerControl player, bool direct = true)
    {
        var role = player.GetOWRole();

        string text = player.GetDefaultAppearance().PlayerName + " ";
        string factionName = string.Empty;

        if (direct)
        {
            text += role.revealText;
        }
        else
        {
            if (player.IsFaction(Faction.Crewmate, true)) 
            {
                text += "attempts to rid the evil within others.";
                factionName = "Crewmate";
            }
            if (player.IsFaction(Faction.Neutral, true))
            {
                text += "has intentions of completing their goal.";
                factionName = "Neutral";
            }
            if (player.IsFaction(Faction.Infiltrator, true))
            {
                text += "wants to take over the ship.";
                factionName = "Infiltrator";
            }
        }

        var factionColor = MiscUtils.GetFactionColour(player);
        var roleColor = MiscUtils.GetRoleColour(role.RoleName);

        if (player.IsFaction(Faction.Crewmate, true)) roleColor = MiscUtils.GetRoleColour("Crewmate");
        if (player.IsFaction(Faction.Infiltrator, true)) roleColor = MiscUtils.GetRoleColour("Infiltrator");
        if (player.IsFaction(Faction.Neutral, true)) roleColor = MiscUtils.GetRoleColour("Neutral");

        if (!direct) return text + $" they must be a <color=#" + factionColor.ToHtmlStringRGBA() + $">{factionName}!</color>";
        return text + $" they must be the <color=#" + roleColor.ToHtmlStringRGBA() + $">{role.RoleName}!";
    }
}