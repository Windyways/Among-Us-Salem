namespace AmongUsSalem.Modifiers;

public sealed class Astral(bool perma) : BaseModifier
{
    public override string ModifierName => "Astral";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public bool permanent => perma;

    public override void OnMeetingStart()
    {
        if (!permanent) Player.RpcRemoveModifier<Astral>();
    }
}