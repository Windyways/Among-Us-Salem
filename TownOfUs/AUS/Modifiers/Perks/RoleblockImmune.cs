namespace AmongUsSalem.Modifiers;

public sealed class RoleBlockImmune(bool perma) : BaseModifier
{
    public override string ModifierName => "Roleblock Immunity";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public bool permanent => perma;

    public override void OnMeetingStart()
    {
        if (!permanent) Player.RpcRemoveModifier<RoleBlockImmune>();
    }
}