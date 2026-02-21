using AmongUsSalem.Enums;
using TownOfUs.Modifiers;
using static UnityEngine.GraphicsBuffer;

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
        bool blockVisit = false;


        if (blockVisit) return false;
        return true;
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

    public static void AddDeathReason(this PlayerControl player, DeathReasonShow deathReasonShow)
    {
        DeathHandlerModifier.UpdateDeathHandler(player, deathReasonShow, DeathHandlerOverride.SetFalse);
    }
}