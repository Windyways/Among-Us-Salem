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
        var attack = Attack.None;
        var defense = Defense.None;
        var etherealDefense = EtherealDefense.None;

        if (overrideAttack != Attack.None) attack = overrideAttack;
        else if (player.HasDied() && player.TryGetModifier<LinkStatAD>(out var stat)) attack = stat.attack;
        else if (player.Data.Role is ICustomAURole customRole) attack = customRole.Attack;

        if (target.HasDied() && target.TryGetModifier<LinkStatAD>(out var stat2))
        {
            defense = stat2.defense;
            etherealDefense = stat2.etherealDefense;
        }
        else if (target.Data.Role is ICustomAURole customRole2)
        {
            defense = customRole2.Defense;
            etherealDefense = customRole2.EtherealDefense;
        }


        if (!player.Is(Faction.Town) && target.Is(Alignment.NeutralPariah))
        {
            if ((int)attack > (int)etherealDefense) return true;
            return false;
        }

        if ((int)attack > (int)defense) return true;
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

    public static int Rampage(this PlayerControl player, PlayerControl t, DeathReasonShow deathReason, bool lunge = true)
    {
        int kills = 0;
        if (player.CanKill(t))
        {
            kills++;
            player.RpcCustomMurder(t, teleportMurderer: lunge);
            VisitingMechanic.RpcAddDeathReason(t, (int)deathReason);
        }
        else player.Notify(Feedback.TooMuchDefense(player, t), NotifyMode.InstantlyAndMeeting);

        if (MeetingHud.Instance)
            return kills; // Rampages don't work during meetings.

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