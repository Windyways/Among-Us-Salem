namespace AmongUsSalem.Modifiers;

public sealed class RalliedModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Rallied";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
    public override void OnDeath(DeathReason reason)
    {
        Player.RpcRemoveModifier<RalliedModifier>();
    }
}