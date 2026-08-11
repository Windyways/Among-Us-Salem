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
                MiscUtils.AddBotFakeChat(Caster, $"{Player.Name()} was Town, so {comparedTo.Name()} should be good...");
            }
            else
            {
                comparedTo.AddModifier<IncriminatingEvidence>();
                MiscUtils.AddBotFakeChat(Caster, $"Wait, {Player.Name()} was Town, so it has to be {comparedTo.Name()}!");
            }
        }
        else
        {
            if (Player.Is(Faction.Town))
            {
                comparedTo.AddModifier<IncriminatingEvidence>();
                MiscUtils.AddBotFakeChat(Caster, $"Wait, {Player.Name()} was Town, so it has to be {comparedTo.Name()}!");
            }
            else
            {
                comparedTo.AddModifier<SoftCleared>();
                comparedTo.RemoveModifier<IncriminatingEvidence>();
                MiscUtils.AddBotFakeChat(Caster, $"{Player.Name()} was evil, so {comparedTo.Name()} should be good...");
            }
        }
    }
}