using System.Collections;
using TownOfUs.Modifiers;
using UnityEngine;

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
            // --- NON BLOCKING INTERACTIONS --- (ANY ORDER, user first tho since its neater)
            if (isVisiting && user.TryGetModifier<InfectedModifier>(out var infected)) blockVisit += infected.PerformInteraction(user, target);
            if (isVisiting && user.TryGetModifier<CursedModifier>(out var cursed)) blockVisit += cursed.PerformInteraction(user, target);
            if (isVisiting && user.TryGetModifier<TrackedModifier>(out var tracked)) blockVisit += TrackedModifier.RpcPerformInteraction(tracked.Caster, user, target);
            if (isVisiting && target.TryGetModifier<StackOfPestilenceModifier>(out var pestiStack)) blockVisit += pestiStack.PerformInteraction(user, target);
            if (isVisiting && target.TryGetModifier<InfectedModifier>(out var infected2)) blockVisit += infected2.PerformInteraction(target, user);
            if (isVisiting && target.TryGetModifier<WatchedModifier>(out var watched) && !user.HasModifier<Camouflage>()) blockVisit += WatchedModifier.RpcPerformInteraction(watched.Caster, user, target);

            // --- PROTECTION INTERACTIONS ---
            if (isAttacking && isVisiting && target.TryGetModifier<GuardedModifier>(out var guarded) && !user.HasModifier<IllusionedModifier>())
                blockVisit += guarded.PerformInteraction(user, target);

            // --- COUNTERATTACK INTERACTIONS --- (Order does NOT matter!)
            if (isVisiting && target.TryGetModifier<FortifiedModifier>(out var fortified) && fortified.Caster != user) blockVisit += fortified.PerformInteraction(user, target, isAttacking);
            if (isVisiting && target.TryGetModifier<JinxedModifier>(out var jinxed) && !user.Is(Faction.Coven)) blockVisit += jinxed.PerformInteraction(user);
            if (isVisiting && target.Data.Role is Werewolf werewolf) blockVisit += werewolf.PerformInteraction(user);
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
}