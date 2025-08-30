using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers.Types;
using ObjectWorkshop.Events.TouEvents;
using ObjectWorkshop.Options.Roles.Impostor;

namespace ObjectWorkshop.Modifiers.Impostor.Venerer;

public sealed class VenererSprintModifier : TimedModifier, IVenererModifier
{
    public override string ModifierName => "Sprint";
    public override bool AutoStart => true;
    public override float Duration => OptionGroupSingleton<VenererOptions>.Instance.AbilityDuration;

    public override void OnActivate()
    {
        var touAbilityEvent = new TouAbilityEvent(AbilityType.VenererSprintAbility, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
    }
}