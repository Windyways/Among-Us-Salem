using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers.Types;
using AmongUsSalem.Events.TouEvents;
using AmongUsSalem.Options.Roles.Neutral;

namespace AmongUsSalem.Modifiers.Neutral;

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