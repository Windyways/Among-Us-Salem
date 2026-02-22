namespace AmongUsSalem.Modifiers;

public sealed class OverchargedModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Overcharged";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public enum State { AppliedThisNight, ActiveThisNight }

    public State currentState = State.AppliedThisNight;
    public PlayerControl Caster => c;
    public override void OnMeetingStart()
    {
        if (currentState == State.ActiveThisNight) Player.RpcRemoveModifier<OverchargedModifier>();
        else
        {
            currentState = State.ActiveThisNight;
            Player.Notify(Catalyst.Info(), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.CatalystRoleCard.LoadAsset());
        }
    }
}