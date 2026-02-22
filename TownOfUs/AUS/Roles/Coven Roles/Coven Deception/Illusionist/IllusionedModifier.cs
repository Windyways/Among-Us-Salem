namespace AmongUsSalem.Modifiers;

public sealed class IllusionedModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Illusioned";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
    /*public override void OnActivate()
    {
        var player = ModifierUtils.GetPlayersWithModifier<IllusionedModifier>(x => x.Caster == Caster).FirstOrDefault();
        player?.RpcRemoveModifier<IllusionedModifier>();
    }*/

    public override void OnMeetingStart()
    {
        Player.RpcRemoveModifier<IllusionedModifier>();
    }
}