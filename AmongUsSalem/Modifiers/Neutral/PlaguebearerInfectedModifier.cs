using MiraAPI.Events;
using MiraAPI.Modifiers;
using ObjectWorkshop.Events.TouEvents;
using ObjectWorkshop.Roles.Neutral;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Modifiers.Neutral;

public sealed class PlaguebearerInfectedModifier(byte plaguebearerId) : BaseModifier
{
    private readonly Color color = new(0.9f, 1f, 0.7f, 1f);
    public override string ModifierName => "Infected";
    public override bool HideOnUi => true;

    public byte PlagueBearerId { get; } = plaguebearerId;

    public override void OnActivate()
    {
        base.OnActivate();

        var pb = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.PlayerId == PlagueBearerId);
        var touAbilityEvent = new TouAbilityEvent(AbilityType.PlaguebearerInfect, pb!, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
    }

    public override void FixedUpdate()
    {
        if (PlayerControl.LocalPlayer.IsRole<PlaguebearerRole>() && Player != PlayerControl.LocalPlayer)
        {
            Player?.cosmetics.SetOutline(true, new Il2CppSystem.Nullable<Color>(color));
        }
    }

    public override void OnDeactivate()
    {
        Player.cosmetics.SetOutline(false, new Il2CppSystem.Nullable<Color>(color));
    }
}