namespace AmongUsSalem.Modifiers;

public sealed class ComparedModifier(PlayerControl c, PlayerControl ct, bool friends) : BaseModifier
{
    public override string ModifierName => "Compared";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
    public PlayerControl comparedTo => ct;
    public bool isFriends => friends;

    public override void OnDeath(DeathReason reason)
    {
        if (isFriends)
        {
            if (Player.Is(Faction.Town))
            {
                comparedTo.AddModifier<SoftCleared>();
                comparedTo.RemoveModifier<IncriminatingEvidence>();
            }
            else comparedTo.AddModifier<IncriminatingEvidence>();
        }
        else
        {
            if (Player.Is(Faction.Town)) comparedTo.AddModifier<IncriminatingEvidence>();
            else
            {
                comparedTo.AddModifier<SoftCleared>();
                comparedTo.RemoveModifier<IncriminatingEvidence>();
            }
        }
    }
}