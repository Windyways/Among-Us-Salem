namespace AmongUsSalem.Modifiers;

public class OverrideDefense(int def) : BaseModifier
{
    public override string ModifierName => "Override Defense";
    public override bool HideOnUi => !Debugger.IsDebuggerActive;
    public override string GetDescription()
    {
        return "Your defense has been overwritten!";
    }

    public Defense defense => (Defense)def;
}