namespace AmongUsSalem.Modifiers;

public sealed class VestedModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Vested";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
    public override void OnMeetingStart()
    {
        Player.RpcRemoveModifier<VestedModifier>();
    }
}