namespace AmongUsSalem.Modifiers;

public sealed class FramedModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Framed";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
    public override void OnMeetingStart()
    {
        if (!OptionGroupSingleton<Framer_Options>.Instance.RemovedOnVisit) 
            Player.RpcRemoveModifier<FramedModifier>();
    }
}