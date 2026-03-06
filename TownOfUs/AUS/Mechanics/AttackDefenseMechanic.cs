using UnityEngine;

namespace AmongUsSalem.Mechanics;

public static class AttackDefenseMechanic
{
    [MethodRpc((uint)AUSRpc.RpcApplyDefense)]
    public static void RpcApplyDefense(PlayerControl player, Defense defense, bool perma = false, bool overrideValue = false, bool visualize = false)
    {
        if (player.Data.Role is ICustomAURole customRole) customRole.ApplyDefense(player, defense, perma, overrideValue, visualize);
    }

    [MethodRpc((uint)AUSRpc.RpcApplyAttack)]
    public static void RpcApplyAttack(PlayerControl player, Attack attack, bool perma = false, bool overrideValue = false)
    {
        if (player.Data.Role is ICustomAURole customRole) customRole.ApplyAttack(attack, perma, overrideValue);
    }

    public static bool CanKill(this PlayerControl player, PlayerControl target, Attack overrideAttack = Attack.None)
    {
        if (player.Data.Role is ICustomAURole role && target.Data.Role is ICustomAURole targetRole)
        {
            if (player.HasDied())
            {
                var deadRole = player.GetRoleWhenAlive();
                if (deadRole is ICustomAURole cr) role = cr;
            }
            if (target.HasDied())
            {
                var deadRole = target.GetRoleWhenAlive();
                if (!target.Is(Faction.Neutral) && !target.Is(Faction.Coven)) return CanKillLinkStat(player, target, overrideAttack);
                if (deadRole is ICustomAURole cr) targetRole = cr;
            }

            if (overrideAttack > Attack.None)
            {
                if (!player.Is(Faction.Town) && target.Is(Alignment.NeutralPariah))
                {
                    if ((int)overrideAttack > (int)targetRole.EtherealDefense) return true;
                    return false;
                }

                if ((int)overrideAttack > (int)targetRole.Defense) return true;

                return false;
            }

            if (!player.Is(Faction.Town) && target.Is(Alignment.NeutralPariah))
            {
                if ((int)role.Attack > (int)targetRole.EtherealDefense) return true;
                return false;
            }

            if ((int)role.Attack > (int)targetRole.Defense) return true;
        }
        return false;
    }

    private static bool CanKillLinkStat(PlayerControl player, PlayerControl target, Attack overrideAttack = Attack.None)
    {
        if (player.Data.Role is ICustomAURole role && target.TryGetModifier<LinkStatAD>(out var stat))
        {
            if (player.HasDied())
            {
                var deadRole = player.GetRoleWhenAlive();
                if (deadRole is ICustomAURole cr) role = cr;
            }

            if (overrideAttack > Attack.None)
            {
                if (!player.Is(Faction.Town) && target.Is(Alignment.NeutralPariah))
                {
                    if ((int)overrideAttack > (int)stat.etherealDefense) return true;
                    return false;
                }

                if ((int)overrideAttack > (int)stat.defense) return true;

                return false;
            }

            if (!player.Is(Faction.Town) && target.Is(Alignment.NeutralPariah))
            {
                if ((int)role.Attack > (int)stat.etherealDefense) return true;
                return false;
            }

            if ((int)role.Attack > (int)stat.defense) return true;
        }
        return false;
    }

    public static int Rampage(this PlayerControl player, PlayerControl t, DeathReasonShow deathReason)
    {
        int kills = 0;
        if (player.CanKill(t))
        {
            kills++;
            player.RpcCustomMurder(t);
            VisitingMechanic.RpcAddDeathReason(t, (int)deathReason);
        }
        else player.Notify(Feedback.TooMuchDefense(player, t), NotifyMode.InstantlyAndMeeting);

        float radius = OptionGroupSingleton<AUSOptions>.Instance.RampageRadius;
        foreach (var target in PlayerControl.AllPlayerControls)
        {
            if (Vector2.Distance(target.transform.position, t.transform.position) <= radius &&
                !target.HasDied() && target != player && t != target)
            {
                if (player.CanKill(target))
                {
                    kills++;
                    player.RpcCustomMurder(target, teleportMurderer: false);
                    VisitingMechanic.RpcAddDeathReason(target, (int)deathReason);
                }
                else player.Notify(Feedback.TooMuchDefense(player, target), NotifyMode.InstantlyAndMeeting);
            }
        }

        return kills;
    }
}