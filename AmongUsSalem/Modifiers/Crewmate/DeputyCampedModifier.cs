using MiraAPI.Events;
using MiraAPI.Modifiers;
using ObjectWorkshop.Events.TouEvents;
using UnityEngine;

namespace ObjectWorkshop.Modifiers.Crewmate;

public sealed class DeputyCampedModifier(PlayerControl deputy) : BaseModifier
{
    public override string ModifierName => "Camped";
    public override bool HideOnUi => true;

    public PlayerControl Deputy { get; } = deputy;

    public override void OnActivate()
    {
        base.OnActivate();

        var touAbilityEvent = new TouAbilityEvent(AbilityType.DeputyCamp, Deputy, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (Player == null || Deputy == null)
        {
            ModifierComponent?.RemoveModifier(this);
            return;
        }

        if (Deputy.AmOwner)
        {
            Player?.cosmetics.SetOutline(true, new Il2CppSystem.Nullable<Color>(OWColors.Deputy));
        }
    }

    public override void OnDeath(DeathReason reason)
    {
        Player.cosmetics.SetOutline(false, new Il2CppSystem.Nullable<Color>(OWColors.Deputy));
    }

    public override void OnDeactivate()
    {
        Player.cosmetics.SetOutline(false, new Il2CppSystem.Nullable<Color>(OWColors.Deputy));
    }
}