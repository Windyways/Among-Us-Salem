using Reactor.Utilities.Extensions;
using TownOfUs.Utilities.Appearances;
using UnityEngine;

namespace AmongUsSalem.Misc;

public static class Feedback
{
    public static string RevealRole(PlayerControl player)
    {
        if (player.Data.Role is ICustomAURole customRole)
        {
            string text = player.GetDefaultAppearance().PlayerName + " ";
            text += customRole.revealText;

            return text + $" they must be the <color=#" + customRole.RoleColor.ToHtmlStringRGBA() + $">{customRole.RoleName}!";
        }

        return "";
    }

    public static string UnknownObstacle(PlayerControl target)
    {
        return "There was an <b><color=#4a86e8>Unknown Obstacle</color></b> when visiting " + target.GetDefaultAppearance().PlayerName + "!";
    }

    public static string AttackedButDefense()
    {
        return "Someone attacked you, but your defense was too high!";
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

        if (player.AmOwner()) Coroutines.Start(MiscUtils.CoFlash(RoleColors.Mafia));

        /*if (player.IsRole<Vigilante>())
        {
            return target.GetDefaultAppearance().PlayerName + " was immune to your attack.";
        }*/

        target.Notify(AttackedButDefense(), NotifyMode.OnlyMeeting);
        return target.GetDefaultAppearance().PlayerName + "'s defense was too high to kill!";
    }

    public static void Notify(this PlayerControl player, string feedback, NotifyMode type, Color color = new(), Sprite? sprite = null, NetworkedPlayerInfo? basePlayer = null)
    {
        if (player.AmOwner())
        {
            if (type == NotifyMode.Instantly) MiscUtils.ShowNotification(feedback, color, sprite);
            if (type == NotifyMode.InstantlyAndMeeting)
            {
                MiscUtils.ShowNotification(feedback, color, sprite);
                MiscUtils.AddFakeChat(basePlayer == null ? player.CachedPlayerData : basePlayer, "FEEDBACK", feedback);
            }
            if (type == NotifyMode.OnlyMeeting) MiscUtils.AddFakeChat(basePlayer == null ? player.CachedPlayerData : basePlayer, "FEEDBACK", feedback);
        }
    }
}