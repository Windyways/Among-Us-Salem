namespace AmongUsSalem.Modifiers;

public sealed class HexedModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Hexed";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
    public override void OnDeath(DeathReason reason)
    {
        Player.RpcRemoveModifier<HexedModifier>();
    }
}