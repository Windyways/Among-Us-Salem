namespace AmongUsSalem.Modifiers;

public sealed class CautiousModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Cautious";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
    public override void OnActivate()
    {
        if (Player.AmOwner())
        {
            var cautiousButton = CustomButtonSingleton<SerialKiller_Cautious>.Instance;
            cautiousButton.OverrideName("Disable Cautious");
        }
    }

    public override void OnDeactivate()
    {
        if (Player.AmOwner())
        {
            var cautiousButton = CustomButtonSingleton<SerialKiller_Cautious>.Instance;
            cautiousButton.OverrideName("Cautious");
        }
    }
}