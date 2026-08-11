namespace AmongUsSalem.Modifiers;

public sealed class MurderModifier : BaseModifier
{
    public override string ModifierName => "Murder";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public override void OnMeetingStart()
    {
        Player.GetModifiers<MurderModifier>().Do(x => Player.RemoveModifier(x));
    }
}