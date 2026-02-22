using System.Collections;
using TownOfUs.Modifiers;
using UnityEngine;

namespace AmongUsSalem.Mechanics;

public static class VisitingMechanic
{
    public static bool IsTargetingValid(this CustomActionButton button, PlayerControl user, PlayerControl? target, 
        bool isAttacking, bool isVisiting)
    {
        if (target == null || button == null || user == null) return false;
        if (button.Timer > 0) return false;
        return IsSuccessfulVisit(user, target, isAttacking, isVisiting);
    }

    public static bool IsSuccessfulVisit(PlayerControl user, PlayerControl target, bool isAttacking, bool isVisiting)
    {
        Coroutines.Start(PostSuccessfulVisit(user, target, isAttacking, isVisiting));
        bool blockVisit = false;

        // --- PROTECTION INTERACTIONS ---
        if (isAttacking && isVisiting && target.TryGetModifier<GuardedModifier>(out var guarded) && !user.HasModifier<IllusionedModifier>()) 
            return guarded.bodyguard.PerformInteraction(user, target);

        if (blockVisit) return false;
        return true;
    }

    public static IEnumerator PostSuccessfulVisit(PlayerControl user, PlayerControl target, bool isAttacking, bool isVisiting)
    {
        yield return new WaitForSeconds(0.5f);
        if (isVisiting && target.HasModifier<FramedModifier>() && user.Is(Alignment.TownInvestigative)) 
            target.RpcRemoveModifier<FramedModifier>();
    }

    public static bool CanKill(this PlayerControl player, PlayerControl target, Attack overrideAttack = Attack.None)
    {
        if (player.Data.Role is ICustomAURole role && target.Data.Role is ICustomAURole targetRole)
        {
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

            if (player.Is(Faction.Town) && target.Is(Alignment.NeutralPariah))
            {
                if ((int)role.Attack > (int)targetRole.EtherealDefense) return true;
                return false;
            }

            if ((int)role.Attack > (int)targetRole.Defense) return true;
        }
        return false;
    }

    [MethodRpc((uint)AUSRpc.RpcAddDeathReason)]
    public static void RpcAddDeathReason(PlayerControl player, int deathReasonShow)
    {
        DeathHandlerModifier.UpdateDeathHandler(player, (DeathReasonShow)deathReasonShow, DeathHandlerOverride.SetFalse);
    }
}