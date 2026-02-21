namespace AmongUsSalem.MCI;

public class TI : BaseModifier
{
    public override string ModifierName => "TI";
    public override bool HideOnUi => !Debugger.IsDebuggerActive;
    public override string GetDescription()
    {
        return $"You have gotten the Towns trust by getting info!";
    }
}