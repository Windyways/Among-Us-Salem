using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers.Types;
using ObjectWorkshop.Events.TouEvents;
using ObjectWorkshop.Options.Roles.Crewmate;
using ObjectWorkshop.Roles.Crewmate;
using UnityEngine;

namespace ObjectWorkshop.Modifiers.Crewmate;

public sealed class HunterStalkedModifier(PlayerControl hunter) : TimedModifier
{
    public override string ModifierName => "Stalked";
    public override bool HideOnUi => true;
    public override float Duration => OptionGroupSingleton<HunterOptions>.Instance.HunterStalkDuration;
    public PlayerControl Hunter { get; set; } = hunter;

    public override void OnActivate()
    {
        base.OnActivate();
        var touAbilityEvent = new TouAbilityEvent(AbilityType.HunterStalk, Hunter, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (PlayerControl.LocalPlayer.Data.Role is HunterRole)
        {
            Player?.cosmetics.SetOutline(true, new Il2CppSystem.Nullable<Color>(OWColors.Hunter));
        }
    }

    public override void OnDeactivate()
    {
        Player.cosmetics.SetOutline(false, new Il2CppSystem.Nullable<Color>(OWColors.Hunter));
    }

    public override void OnDeath(DeathReason reason)
    {
        Player.cosmetics.SetOutline(false, new Il2CppSystem.Nullable<Color>(OWColors.Hunter));
    }
}