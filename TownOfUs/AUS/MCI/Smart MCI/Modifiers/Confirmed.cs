namespace AmongUsSalem.MCI;

public class Confirmed : BaseModifier
{
    public override string ModifierName => "Confirmed";
    public override bool HideOnUi => !Debugger.IsDebuggerActive;
    public override string GetDescription()
    {
        return $"You are Confirmed from the Town!";
    }
}