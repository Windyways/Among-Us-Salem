using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers.Types;
using ObjectWorkshop.Events.TouEvents;
using ObjectWorkshop.Options.Roles.Neutral;

namespace ObjectWorkshop.Modifiers.Neutral;

public sealed class SurvivorVestModifier : TimedModifier
{
    public override float Duration => OptionGroupSingleton<SurvivorOptions>.Instance.VestDuration;
    public override string ModifierName => "Vested";
    public override bool AutoStart => true;
    public override bool HideOnUi => true;

    public override void OnActivate()
    {
        base.OnActivate();

        var touAbilityEvent = new TouAbilityEvent(AbilityType.SurvivorVest, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
    }
}