namespace AmongUsSalem.Modifiers;

public class VisualizeDefense(int def) : BaseModifier
{
    public override string ModifierName => "Visualize Defense";
    public override bool HideOnUi => !Debugger.IsDebuggerActive;
    public override string GetDescription()
    {
        return "Your defense has been overwritten!";
    }

    public Defense defense => (Defense)def;
    public override void OnMeetingStart()
    {
        Player.RpcRemoveModifier<VisualizeDefense>();
    }
}