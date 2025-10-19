using System;
using System.Collections;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
public static class PlayerControl_Die
{
    public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] DeathReason reason)
    {
        __instance.Data.IsDead = true;

        foreach (var role in GameHistory.AllRoles)
        {
            if (!role || role is not ICustomAURole ausRole)
            {
                continue;
            }

            if (role.Player == __instance)
            {
                ausRole.OnDeath(reason);
            }
            
            ausRole.OnTargetDeath(__instance, reason);
        }
        
        foreach (var role in GameHistory.AllRoles)
        {
            if (!role || role is not ICovenRole coven)
            {
                continue;
            }

            if (role.Player == __instance)
            {
                coven.OnDeath();
            }
        }

        return true;
    }
}