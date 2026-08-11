using UnityEngine;

namespace AmongUsSalem.Misc;

public static class Feedback
{
    public static string RevealRole(PlayerControl revealer, PlayerControl player)
    {
        if (player.Data.Role is ICustomAURole customRole)
        {
            if (player.HasDied()) customRole = player.GetICustomAURoleWhenAlive();

            string text = player.GetDefaultAppearance().PlayerName + " ";
            string roleName = customRole.RoleName;
            Color roleColor = customRole.RoleColor;
            string endText = $" They must be a <color=#" + roleColor.ToHtmlStringRGBA() + $"><b>{roleName}</b>!";

            if (revealer.IsRole<Consigliere>() && player.HasModifier<HexedModifier>())
            {
                roleName = "Hex Master";
                roleColor = RoleColors.Coven;
                text += "is versed in the ways of hexes.";
                endText = $" They must be a <color=#" + roleColor.ToHtmlStringRGBA() + $"><b>{roleName}</b>!";
            }
            else if (player.TryGetModifier<SelfReflectionModifier>(out var selfReflect))
            {
                roleName = selfReflect.GetRole().NiceName;
                roleColor = RoleColors.Town;
                text += selfReflect.GetICustom().revealText;
                endText = $" They must be a <color=#" + roleColor.ToHtmlStringRGBA() + $"><b>{roleName}</b>!";
            }
            else
            {
                text += customRole.revealText;
                if (player.IsRole<War>() || player.IsRole<Pestilence>() || player.IsRole<Death>()) 
                    endText = $" They must be <color=#" + roleColor.ToHtmlStringRGBA() + $"><b>{roleName}</b>, Horseman of the Apocalypse.";
            }

            return text + endText;
        }

        return "";
    }

    public static string UnknownObstacle(PlayerControl target)
    {
        return "There was an Unknown Obstacle when visiting " + target.GetDefaultAppearance().PlayerName + "!";
    }

    public static string LeaveTown(PlayerControl player)
    {
        return $"{player.Name()} has accomplished their goal as {player.Data.Role.NiceName} and left town.";
    }

    public static string AttackedButDefense()
    {
        return "Someone attacked you, but your defense was too high!";
    }

    public static string TooMuchDefense(PlayerControl player, PlayerControl target)
    {
        /* if (SurvivorFunction.Interfere(target)) SurvivorFunction.NotifySurvivor(player, target);
         else if (ClericFunction.Interfere(target)) ClericFunction.NotifyCleric(player, target);
         else if (OracleFunction.Interfere(target)) OracleFunction.NotifyOracle(player, target);
         else if (GuardianAngelFunction.Interfere(target)) GuardianAngelFunction.NotifyGuardianAngel(player, target);
         else if (target.IsTrapped()) // Some sort of trapper 'attacked' RPC here.
 */

        if (player.AmOwner()) Coroutines.Start(MiscUtils.CoFlash(RoleColors.Mafia));

        /*if (player.IsRole<Vigilante>())
        {
            return target.GetDefaultAppearance().PlayerName + " was immune to your attack.";
        }*/

        if (target.HasModifier<SelfProtectedModifier>()) Bodyguard.RpcNotify(target, (int)NotificationType.Bodyguard_SelfProtect);
        else if (target.HasModifier<VestedModifier>()) Survivor.RpcNotify(target, (int)NotificationType.Bodyguard_SelfProtect);
        else if (target.TryGetModifier<BarrieredModifier>(out var barriered))
        {
            PotionMaster.RpcNotify(target, (int)NotificationType.PotionMaster_AttackAndBarriered, barriered.Caster);
            PotionMaster.RpcNotify(barriered.Caster, (int)NotificationType.PotionMaster_TargetAttacked, target);
        }
        else RpcTMDNotify(target); //target.Notify(AttackedButDefense(), NotifyMode.OnlyMeeting);

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

    [MethodRpc((uint)AUSRpc.RpcNotifyLeaveTown)]
    public static void RpcLeaveTownNotify(PlayerControl player)
    {
        if (player.Data.Role is ICustomAURole customRole)
        {
            PlayerControl.LocalPlayer.Notify(LeaveTown(player), NotifyMode.InstantlyAndMeeting, sprite: customRole.Configuration.Icon.LoadAsset());
        }
    }

    [MethodRpc((uint)AUSRpc.RpcTMDNotify)]
    public static void RpcTMDNotify(PlayerControl player)
    {
        if (player.AmOwner())
        {
            player.Notify(AttackedButDefense(), NotifyMode.OnlyMeeting);
        }
    }
}