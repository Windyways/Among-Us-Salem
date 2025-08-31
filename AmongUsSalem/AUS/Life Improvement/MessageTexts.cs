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

        if (player.IsRole<VigilanteRole>())
        {
            return target.GetDefaultAppearance().PlayerName + " was immune to your attack.";
        }
        return target.GetDefaultAppearance().PlayerName + "'s defense was too high to kill!";
    }

    public static string GetDeathReason(this PlayerControl player, DeathReasonShow reason)
    {
        if (reason == DeathReasonShow.ShotByAVeteran) return $"<color=#06e00c>Shot By A Veteran</color>";
        if (reason == DeathReasonShow.KilledByAMemberOfTheMafia) return $"<color=#dd0000>Killed By A Member Of The Mafia</color>";
        if (reason == DeathReasonShow.KilledByTheCoven) return $"<color=#06e00c>Killed By The Coven</color>";
        return "";
    }
}