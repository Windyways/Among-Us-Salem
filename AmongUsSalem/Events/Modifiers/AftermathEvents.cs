using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using ObjectWorkshop.Buttons;
using ObjectWorkshop.Modifiers.Game.Crewmate;
using UnityEngine;

namespace ObjectWorkshop.Events.Modifiers;

public static class AftermathEvents
{
    [RegisterEvent]
    public static void AftermathDeathEvent(AfterMurderEvent @event)
    {
        var source = @event.Source;

        if (!@event.Target.HasModifier<AftermathModifier>() || !source.AmOwner || MeetingHud.Instance)
        {
            return;
        }

        var button = CustomButtonManager.Buttons.Where(x => x.Enabled(source.Data.Role) && x.Timer <= 0)
            .OfType<IAftermathableButton>().FirstOrDefault();
        if (button == null)
        {
            return;
        }

        var notif1 = Helpers.CreateAndShowNotification(
            $"<b>{OWColors.Aftermath.ToTextColor()}{@event.Target.Data.PlayerName} was an Aftermath, forcing you to use your ability.</color></b>",
            Color.white, spr: TouModifierIcons.Aftermath.LoadAsset());

        notif1.Text.SetOutlineThickness(0.35f);
        notif1.transform.localPosition = new Vector3(0f, 1f, -20f);

        switch (button)
        {
            case IAftermathablePlayerButton playerButton:
                playerButton.Target = source;
                break;
            case IAftermathableBodyButton bodyButton:
                bodyButton.Target =
                    source.GetNearestDeadBody(2f); // By logic, the closest body *should* be the one that just appeared
                break;
        }

        button.ClickHandler();
    }
}