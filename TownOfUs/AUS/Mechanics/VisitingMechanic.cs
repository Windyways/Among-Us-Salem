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

    // ---
    public static bool CheckVisit(PlayerControl player, PlayerControl target, int Button, bool isAttacking, bool isVisiting, bool occurVisit = true)
    {
        if (player.HasModifier<Astral>()) isVisiting = false;

        Coroutines.Start(PostSuccessfulVisit(player, target, isAttacking, isVisiting));
        int blockVisit = 0;
        bool wasAlive = true;

        // --- ROLEBLOCK INTERACTIONS ---

        // --- REDIRECT INTERACTIONS ---
        if (blockVisit < 100)
        {

        }

        var playerRole = player.GetRoleWhenAlive();
        var targetRole = target?.GetRoleWhenAlive();

        ResetCooldowns(player, target, Button, isAttacking);

        // --- UNKNOWN OBSTACLE INTERACTIONS ---

        if (blockVisit < 100)
        {
            // --- NON BLOCKING INTERACTIONS --- (ANY ORDER, user first tho since its neater)
            if (isVisiting && player.HasModifier<InfectedModifier>()) blockVisit += player.GetModifiers<InfectedModifier>().Sum(x => x.PerformInteraction(player, target));
            else if (isVisiting && target.HasModifier<InfectedModifier>()) blockVisit += target.GetModifiers<InfectedModifier>().Sum(x => x.PerformInteraction(player, target)); // Else bc it would prevent double Plagued message.

            if (isVisiting) blockVisit += player.GetModifiers<CursedModifier>().Sum(x => x.PerformInteraction(player, target));
            if (isVisiting) blockVisit += player.GetModifiers<TrackedModifier>().Sum(x => TrackedModifier.RpcPerformInteraction(x.Caster, player, target));
            if (isVisiting) blockVisit += target.GetModifiers<StackOfPestilenceModifier>().Sum(x => x.PerformInteraction(player, target));
            if (isVisiting && !player.HasModifier<Camouflage>()) blockVisit += target.GetModifiers<WatchedModifier>().Sum(x => WatchedModifier.RpcPerformInteraction(x.Caster, player, target));

            // --- PROTECTION INTERACTIONS ---
            if (isAttacking && isVisiting && target.TryGetModifier<GuardedModifier>(out var guarded) && !player.HasModifier<IllusionedModifier>())
                blockVisit += guarded.PerformInteraction(player, target);

            // --- COUNTERATTACK INTERACTIONS --- (Order does NOT matter!)
            if (isVisiting && target.TryGetModifier<FortifiedModifier>(out var fortified) && fortified.Caster != player) blockVisit += fortified.PerformInteraction(player, target, isAttacking);
            if (isVisiting && target.TryGetModifier<AmbushedModifier>(out var ambushed) && !player.Is(Faction.Mafia)) blockVisit += ambushed.PerformInteraction(player);
            if (isVisiting && target.TryGetModifier<JinxedModifier>(out var jinxed) && !player.Is(Faction.Coven)) blockVisit += jinxed.PerformInteraction(player);
            if (isVisiting && target.Data.Role is Werewolf werewolf) blockVisit += werewolf.PerformInteraction(player);
            if (isVisiting && target.Data.Role is Veteran veteran && veteran.isAlerted) blockVisit += veteran.PerformInteraction(player, isAttacking);

            if (!player.IsSameFaction(target, true)) player.RpcAddModifier<TrespassingModifier>();
            if (isAttacking && !player.HasModifier<IllusionedModifier>()) player.RpcAddModifier<MurderModifier>();
        }

        if (blockVisit > 0)
        {
            foreach (var players in PlayerControl.AllPlayerControls)
            {
                if (players.HasDied())
                {
                    var role = players.GetRoleWhenAlive();
                    if (role is ICustomAURole cr)
                    {
                        cr.Role_OnVisitFail(player, target, isAttacking, isVisiting, blockVisit);
                    }
                }
                else if (players.Data.Role is ICustomAURole customRole)
                {
                    customRole.Role_OnVisitFail(player, target, isAttacking, isVisiting, blockVisit);
                }
            }

            AUSPlugin.DebugLogMessage($"{player.Name()} has failed their visit.");
            return false; // Code below only runs if visit was successful.
        }

        if (occurVisit) SuccessfulVisit(player, target, Button);
        return true;
    }


    public static void SuccessfulVisit(PlayerControl player, PlayerControl target, int Button)
    {
        AUSPlugin.DebugLogMessage($"{player.Name()}'s visit was Successful!");

        var role = player.GetRoleWhenAlive();
        if (role is ICustomAURole customRole) customRole.Function(target, Button);
    }

    public static void ResetCooldowns(PlayerControl player, PlayerControl target, int Button, bool Attacking)
    {
        var role = player.GetRoleWhenAlive();
        var targetRole = target.GetRoleWhenAlive();

        if (player.AmOwner)
        {
            // --- Town ----
            // TE
            // TG
            if (role is Pacifist)
            {
                if (Button == 1) CustomButtonSingleton<Pacifist_Rally>.Instance.ResetCooldownAndOrEffect();
                if (Button == 2) CustomButtonSingleton<Pacifist_SelfReflection>.Instance.ResetCooldownAndOrEffect();
            }
            // TI
            if (role is Investigator) CustomButtonSingleton<Investigator_Investigate>.Instance.ResetCooldownAndOrEffect();
            if (role is Lookout) CustomButtonSingleton<Lookout_Watch>.Instance.ResetCooldownAndOrEffect();
            if (role is Seer seer)
            {
                if (Button == 1) CustomButtonSingleton<Seer_Intuit>.Instance.ResetCooldownAndOrEffect();
                if (Button == 2) CustomButtonSingleton<Seer_Gaze>.Instance.ResetCooldownAndOrEffect();
            }
            if (role is Sheriff) CustomButtonSingleton<Sheriff_Search>.Instance.ResetCooldownAndOrEffect();
            if (role is Tracker) CustomButtonSingleton<Tracker_Track>.Instance.ResetCooldownAndOrEffect();
            // TK
            if (role is Veteran) CustomButtonSingleton<Veteran_Alert>.Instance.ResetCooldownAndOrEffect();
            if (role is Vigilante) CustomButtonSingleton<Vigilante_Shoot>.Instance.ResetCooldownAndOrEffect();
            // TO
            if (role is Catalyst) CustomButtonSingleton<Catalyst_Overcharge>.Instance.ResetCooldownAndOrEffect();
            // TP
            if (role is Bodyguard)
            {
                CustomButtonSingleton<Bodyguard_Guard>.Instance.ResetCooldownAndOrEffect();
                CustomButtonSingleton<Bodyguard_SelfProtect>.Instance.ResetCooldownAndOrEffect();
            }
            if (role is Cleric)
            {
                CustomButtonSingleton<Cleric_Barrier>.Instance.ResetCooldownAndOrEffect();
                CustomButtonSingleton<Cleric_SelfBarrier>.Instance.ResetCooldownAndOrEffect();
            }
            if (role is Crusader) CustomButtonSingleton<Crusader_Fortify>.Instance.ResetCooldownAndOrEffect();
            // TS
            if (role is Admirer)
            {
                if (Button == 1) CustomButtonSingleton<Admirer_Admire>.Instance.ResetCooldownAndOrEffect();
                if (Button == 2) CustomButtonSingleton<Admirer_Care>.Instance.ResetCooldownAndOrEffect();
            }

            // --- Neutral ---
            // NA
            if (role is Berserker) CustomButtonSingleton<Berserker_Attack>.Instance.ResetCooldownAndOrEffect();
            if (role is Pestilence) CustomButtonSingleton<Pestilence_SpreadPestilence>.Instance.ResetCooldownAndOrEffect();
            if (role is Plaguebearer) CustomButtonSingleton<Plaguebearer_Infect>.Instance.ResetCooldownAndOrEffect();
            if (role is War)
            {
                if (Button == 1) CustomButtonSingleton<War_Attack>.Instance.ResetCooldownAndOrEffect();
                if (Button == 2) CustomButtonSingleton<War_Attack2>.Instance.ResetCooldownAndOrEffect();
            }
            if (role is Warlock) CustomButtonSingleton<Warlock_Curse>.Instance.ResetCooldownAndOrEffect();
            // NB
            if (role is Amnesiac) CustomButtonSingleton<Amnesiac_Remember>.Instance.ResetCooldownAndOrEffect();
            if (role is Survivor) CustomButtonSingleton<Survivor_Vest>.Instance.ResetCooldownAndOrEffect();
            // NC
            // NE
            if (role is Jester) CustomButtonSingleton<Jester_Haunt>.Instance.ResetCooldownAndOrEffect();
            // NK
            if (role is SerialKiller)
            {
                if (Button == 1) CustomButtonSingleton<SerialKiller_Attack>.Instance.ResetCooldownAndOrEffect();
                if (Button == 2) CustomButtonSingleton<SerialKiller_Cautious>.Instance.ResetCooldownAndOrEffect();
            }
            if (role is Werewolf)
            {
                CustomButtonSingleton<Werewolf_Maul>.Instance.ResetCooldownAndOrEffect();
                CustomButtonSingleton<Werewolf_TrackScent>.Instance.ResetCooldownAndOrEffect();
            }
            // NO
            // NP
            if (role is Starspawn)
            {
                CustomButtonSingleton<Starspawn_Isolate>.Instance.ResetCooldownAndOrEffect();
                CustomButtonSingleton<Starspawn_SelfIsolate>.Instance.ResetCooldownAndOrEffect();
            }

            // --- MAFIA ---
            // Mafia Deception
            if (role is Framer) CustomButtonSingleton<Framer_Frame>.Instance.ResetCooldownAndOrEffect();
            // Mafia Killing
            if (role is Ambusher) CustomButtonSingleton<Ambusher_Ambusher>.Instance.ResetCooldownAndOrEffect();
            if (role is Godfather)
            {
                CustomButtonSingleton<Godfather_Kill>.Instance.ResetCooldownAndOrEffect();
                CustomButtonSingleton<Godfather_Order>.Instance.ResetCooldownAndOrEffect();
            }
            if (role is Mafioso) CustomButtonSingleton<Mafioso_Kill>.Instance.ResetCooldownAndOrEffect();
            // Mafia Support
            if (role is Agent) CustomButtonSingleton<Agent_Stalk>.Instance.ResetCooldownAndOrEffect();
            if (role is Consigliere) CustomButtonSingleton<Consigliere_SizeUp>.Instance.ResetCooldownAndOrEffect();

            // --- COVEN ---
            // Coven Deception
            if (role is Illusionist)
            {
                CustomButtonSingleton<Illusionist_Cast>.Instance.ResetCooldownAndOrEffect();
                CustomButtonSingleton<Illusionist_SelfIllusion>.Instance.ResetCooldownAndOrEffect();
            }
            // Coven Killing
            if (role is Jinx) CustomButtonSingleton<Jinx_Jinx>.Instance.ResetCooldownAndOrEffect();
            if (role is Ritualist)
            {
                CustomButtonSingleton<Ritualist_Attack>.Instance.ResetCooldownAndOrEffect();
                CustomButtonSingleton<Ritualist_BloodRitual>.Instance.ResetCooldownAndOrEffect();
            }
            // Coven Outlier
            if (role is Covenite) CustomButtonSingleton<Covenite_Attack>.Instance.ResetCooldownAndOrEffect();
            // Coven Power
            if (role is HexMaster) CustomButtonSingleton<HexMaster_Hex>.Instance.ResetCooldownAndOrEffect();
            // Coven Utility
            if (role is PotionMaster)
            {
                CustomButtonSingleton<PotionMaster_Barrier>.Instance.ResetCooldownAndOrEffect();
                CustomButtonSingleton<PotionMaster_SelfBarrier>.Instance.ResetCooldownAndOrEffect();
                CustomButtonSingleton<PotionMaster_Harmful>.Instance.ResetCooldownAndOrEffect();
                CustomButtonSingleton<PotionMaster_Reveal>.Instance.ResetCooldownAndOrEffect();
            }
            if (role is Wildling) CustomButtonSingleton<Wildling_Sense>.Instance.ResetCooldownAndOrEffect();
        }
    }
}