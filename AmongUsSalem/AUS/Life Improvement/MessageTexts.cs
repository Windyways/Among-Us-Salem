using System;
using System.Collections;
using Il2CppSystem.Runtime.InteropServices;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement;

public static class MessageTexts
{
    public static string RevealRole(PlayerControl player)
    {
        var role = player.GetOWRole();

        string text = player.GetDefaultAppearance().PlayerName + " ";
        text += role.revealText;

        var roleColor = MiscUtils.GetRoleColour(role.RoleName);
        return text + $" they must be the <color=#" + roleColor.ToHtmlStringRGBA() + $">{role.RoleName}!";
    }

    public static string UnknownObstacle(PlayerControl target)
    {
        return "There was an <b><color=#4a86e8>Unknown Obstacle</color></b> when visiting " + target.GetDefaultAppearance().PlayerName + "!";
    }

    public static string TooMuchDefense(PlayerControl player, PlayerControl target)
    {
        /* if (SurvivorFunction.Interfere(target)) SurvivorFunction.NotifySurvivor(player, target);
         else if (ClericFunction.Interfere(target)) ClericFunction.NotifyCleric(player, target);
         else if (BodyguardFunction.Interfere(BodyguardFunction.InterfereTypes.NotifyBodyguard, player, target, true, true, false)) BodyguardFunction.NotifyBodyguard(player, target);
         else if (OracleFunction.Interfere(target)) OracleFunction.NotifyOracle(player, target);
         else if (GuardianAngelFunction.Interfere(target)) GuardianAngelFunction.NotifyGuardianAngel(player, target);
         else if (target.IsTrapped()) // Some sort of trapper 'attacked' RPC here.
 */

        if (player.AmOwner()) Coroutines.Start(MiscUtils.CoFlash(AUSColors.Mafia));

        /*if (player.IsRole<Vigilante>())
        {
            return target.GetDefaultAppearance().PlayerName + " was immune to your attack.";
        }*/
        return target.GetDefaultAppearance().PlayerName + "'s defense was too high to kill!";
    }

    public static string ToSpacedString(this Enum value)
    {
        var name = value.ToString();

        // Insert space before capital letters that follow a lowercase
        name = System.Text.RegularExpressions.Regex.Replace(name, "([a-z])([A-Z])", "$1 $2");

        // Insert space when a capital is followed by another capital + lowercase (e.g., "AShroud")
        name = System.Text.RegularExpressions.Regex.Replace(name, "([A-Z])([A-Z][a-z])", "$1 $2");

        return name;
    }
}