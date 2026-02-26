namespace AmongUsSalem.Modifiers;

public class HideGainedDefense : BaseModifier
{
    public override string ModifierName => "Hide Gained Defense";
    public override bool HideOnUi => !Debugger.IsDebuggerActive;
    public override string GetDescription()
    {
        return "A player gave you defense!";
    }

    public override void OnMeetingStart()
    {
        Player.RpcRemoveModifier<HideGainedDefense>();
    }
}