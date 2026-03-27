namespace AmongUsSalem.Modifiers;

public sealed class ControlImmune(bool perma) : BaseModifier
{
    public override string ModifierName => "Control Immunity";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public bool permanent => perma;

    public override void OnMeetingStart()
    {
        if (!permanent) Player.RpcRemoveModifier<ControlImmune>();
    }
}