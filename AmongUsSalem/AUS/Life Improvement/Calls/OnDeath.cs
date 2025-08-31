using System;
using System.Collections;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
public static class OnDeath
{
    public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] DeathReason reason)
    {
        __instance.Data.IsDead = true;

        foreach (var role in GameHistory.AllRoles)
        {
            if (!role || role is not IAUSRole touRole)
            {
                continue;
            }

            if (role.Player == __instance)
            {
                touRole.OnDeath(reason);
            }
            
            touRole.OnTargetDeath(__instance, reason);
        }

        return true;
    }
}