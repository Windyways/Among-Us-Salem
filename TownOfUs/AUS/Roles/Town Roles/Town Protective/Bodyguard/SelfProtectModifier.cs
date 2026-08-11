namespace AmongUsSalem.Modifiers;

public sealed class SelfProtectedModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Self Protected";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
    public override void OnMeetingStart()
    {
        Player.RpcRemoveModifier<SelfProtectedModifier>();
    }
}