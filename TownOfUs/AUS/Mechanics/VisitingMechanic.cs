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
        if (user.HasModifier<Astral>()) isVisiting = false;

        Coroutines.Start(PostSuccessfulVisit(user, target, isAttacking, isVisiting));
        int blockVisit = 0;
        bool wasAlive = true;

        // --- UNKNOWN OBSTACLE INTERACTIONS ---
        if (isVisiting) blockVisit += user.GetModifiers<IsolatedModifier>().Sum(x => x.PerformInteraction(button, user, target));

        if (blockVisit < 100)
        {
            // --- NON BLOCKING INTERACTIONS --- (ANY ORDER, user first tho since its neater)
            if (isVisiting && user.HasModifier<InfectedModifier>()) blockVisit += user.GetModifiers<InfectedModifier>().Sum(x => x.PerformInteraction(user, target));
            else if (isVisiting && target.HasModifier<InfectedModifier>()) blockVisit += target.GetModifiers<InfectedModifier>().Sum(x => x.PerformInteraction(user, target)); // Else bc it would prevent double Plagued message.

            if (isVisiting) blockVisit += user.GetModifiers<CursedModifier>().Sum(x => x.PerformInteraction(user, target));
            if (isVisiting) blockVisit += user.GetModifiers<TrackedModifier>().Sum(x => TrackedModifier.RpcPerformInteraction(x.Caster, user, target));
            if (isVisiting) blockVisit += target.GetModifiers<StackOfPestilenceModifier>().Sum(x => x.PerformInteraction(user, target));
            if (isVisiting && !user.HasModifier<Camouflage>()) blockVisit += target.GetModifiers<WatchedModifier>().Sum(x => WatchedModifier.RpcPerformInteraction(x.Caster, user, target));

            // --- PROTECTION INTERACTIONS ---
            if (isAttacking && isVisiting && target.TryGetModifier<GuardedModifier>(out var guarded) && !user.HasModifier<IllusionedModifier>())
                blockVisit += guarded.PerformInteraction(user, target);

            // --- COUNTERATTACK INTERACTIONS --- (Order does NOT matter!)
            if (isVisiting && target.TryGetModifier<FortifiedModifier>(out var fortified) && fortified.Caster != user) blockVisit += fortified.PerformInteraction(user, target, isAttacking);
            if (isVisiting && target.TryGetModifier<AmbushedModifier>(out var ambushed) && !user.Is(Faction.Mafia)) blockVisit += ambushed.PerformInteraction(user);
            if (isVisiting && target.TryGetModifier<JinxedModifier>(out var jinxed) && !user.Is(Faction.Coven)) blockVisit += jinxed.PerformInteraction(user);
            if (isVisiting && target.Data.Role is Werewolf werewolf) blockVisit += werewolf.PerformInteraction(user);
            if (isVisiting && target.Data.Role is Veteran veteran && veteran.isAlerted) blockVisit += veteran.PerformInteraction(user, isAttacking);

            if (!user.IsSameFaction(target, true)) user.RpcAddModifier<TrespassingModifier>();
            if (isAttacking && !user.HasModifier<IllusionedModifier>()) user.RpcAddModifier<MurderModifier>();
        }

        if (blockVisit > 0)
        {
            button.ResetCooldownAndOrEffect();
            return false; // Code below only runs if visit was successful.
        }

        if (wasAlive && user.HasDied() && button is IButtonClick buttonClick)
        {
            // Death patch, so ability still goes through if the user died while visiting.
            buttonClick.Click(user, target);
            return false;
        }

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