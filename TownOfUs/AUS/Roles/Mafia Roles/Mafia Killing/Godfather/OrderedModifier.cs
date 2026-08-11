namespace AmongUsSalem.Modifiers;

public sealed class OrderedModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Ordered";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
    public override void OnMeetingStart()
    {
        Player.GetModifiers<OrderedModifier>().Do(x => Player.RemoveModifier(x));
    }
}