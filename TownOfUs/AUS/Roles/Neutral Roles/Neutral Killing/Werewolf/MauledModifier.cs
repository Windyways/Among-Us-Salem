namespace AmongUsSalem.Modifiers;

// Used to check if WW killed tonight cause im lazy to test a boolean.
public sealed class MauledModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Mauled";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
    public override void OnMeetingStart()
    {
        Player.GetModifiers<MauledModifier>().Do(x => Player.RemoveModifier(x));
    }
}