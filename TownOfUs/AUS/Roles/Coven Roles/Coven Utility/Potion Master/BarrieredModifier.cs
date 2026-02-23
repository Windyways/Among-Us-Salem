namespace AmongUsSalem.Modifiers;

public sealed class BarrieredModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Barriered";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
    public override void OnMeetingStart()
    {
        Player.RpcRemoveModifier<BarrieredModifier>();
    }
}