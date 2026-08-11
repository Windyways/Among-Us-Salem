namespace AmongUsSalem.Modifiers;

public sealed class IlluminatedModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Illuminated";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public int NightCount;
    public PlayerControl Caster => c;
    public override void OnMeetingStart()
    {
        NightCount++;
        if (NightCount == 3) Player.RpcRemoveModifier<IlluminatedModifier>();
    }
}