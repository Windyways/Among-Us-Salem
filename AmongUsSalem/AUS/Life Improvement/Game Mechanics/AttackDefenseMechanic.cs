namespace AmongUsSalem.LifeImprovement.GameMechanics;

public static class AttackDefenseMechanic
{
    public static bool CanKill(this PlayerControl player, PlayerControl target, Attack overrideAttack = Attack.None)
    {
        if (player.Data.Role is IAUSRole role && target.Data.Role is IAUSRole targetRole)
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
}

public enum Attack
{
    None,
    Basic,
    Powerful,
    Unstoppable,
    UnstoppablePlus
}

public enum Defense
{
    None,
    Basic,
    Powerful,
    Invincible
}

public enum EtherealDefense
{
    None,
    Basic,
    Powerful,
    Invincible
}