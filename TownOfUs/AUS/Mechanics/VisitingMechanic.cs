using System.Collections;
using TownOfUs.Modifiers;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace AmongUsSalem.Mechanics;

public static class VisitingMechanic
{
    public static bool IsTargetingValid(this CustomActionButton button, PlayerControl user, PlayerControl? target, 
        bool isAttacking, bool isVisiting, bool ignoreCooldown = false)
    {
        if (target == null || button == null || user == null) return false;
        if (button.Timer > 0 && !ignoreCooldown) return false;
        return IsSuccessfulVisit(button, user, target, isAttacking, isVisiting);
    }

    public static bool IsSuccessfulVisit(CustomActionButton button, PlayerControl user, PlayerControl target, bool isAttacking, bool isVisiting)
    {
        Coroutines.Start(PostSuccessfulVisit(user, target, isAttacking, isVisiting));
        int blockVisit = 0;

        // --- UNKNOWN OBSTACLE INTERACTIONS ---
        if (isVisiting && target.TryGetModifier<IsolatedModifier>(out var isolated) && isolated.state == IsolatedModifier.State.Isolated)
            blockVisit += isolated.PerformInteraction(button, user, target);

        if (blockVisit < 100)
        {
            // --- PROTECTION INTERACTIONS ---
            if (isAttacking && isVisiting && target.TryGetModifier<GuardedModifier>(out var guarded) && !user.HasModifier<IllusionedModifier>())
                blockVisit += guarded.PerformInteraction(user, target);

            // --- COUNTERATTACK INTERACTIONS ---
            if (isVisiting && target.TryGetModifier<FortifiedModifier>(out var fortified) && fortified.Caster != user) blockVisit += fortified.PerformInteraction(user, target, isAttacking);
            if (isVisiting && target.TryGetModifier<JinxedModifier>(out var jinxed) && !user.Is(Faction.Coven)) blockVisit += jinxed.PerformInteraction(user);
        }

        if (blockVisit > 0) return false;
        return true;
    }

    public static IEnumerator PostSuccessfulVisit(PlayerControl user, PlayerControl target, bool isAttacking, bool isVisiting)
    {
        yield return new WaitForSeconds(0.5f);
        if (isVisiting && target.HasModifier<FramedModifier>() && user.Is(Alignment.TownInvestigative)) 
            target.RpcRemoveModifier<FramedModifier>();
    }

    [MethodRpc((uint)AUSRpc.RpcAddDeathReason)]
    public static void RpcAddDeathReason(PlayerControl player, int deathReasonShow)
    {
        DeathHandlerModifier.UpdateDeathHandler(player, (DeathReasonShow)deathReasonShow, DeathHandlerOverride.SetFalse);
    }

    public static void LeaveTown(this PlayerControl player)
    {
        if (player.AmOwner())
        {
            player.RpcCustomMurder(player);
            RpcAddDeathReason(player, (int)DeathReasonShow.LeftTown);
        }
    }
}