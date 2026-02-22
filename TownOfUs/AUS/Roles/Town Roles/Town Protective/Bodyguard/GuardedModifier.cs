namespace AmongUsSalem.Modifiers;

public sealed class GuardedModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Guarded";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public Bodyguard bodyguard => Caster?.Data?.Role as Bodyguard;
    public PlayerControl Caster => c;
    public override void OnMeetingStart()
    {
        Player.RpcRemoveModifier<GuardedModifier>();
    }
}