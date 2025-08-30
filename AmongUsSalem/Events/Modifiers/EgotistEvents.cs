using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using ObjectWorkshop.Modifiers.Game;
using ObjectWorkshop.Modifiers.Game.Alliance;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Events.Modifiers;

public static class EgotistEvents
{
    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            return;
        }

        var ego = ModifierUtils.GetActiveModifiers<EgotistModifier>().FirstOrDefault(x => !x.Player.HasDied());
        if (ego != null && Helpers.GetAlivePlayers().Where(x =>
                    x.IsCrewmate() && !(x.TryGetModifier<AllianceGameModifier>(out var ally) && !ally.GetsPunished))
                .ToList().Count == 0)
        {
            if (ego.Player.AmOwner)
            {
                PlayerControl.LocalPlayer.RpcPlayerExile();
                var notif1 = Helpers.CreateAndShowNotification(
                    $"<b>You have successfully won as the {OWColors.Egotist.ToTextColor()}Egotist</color>, as no more crewmates remain!</b>",
                    Color.white, spr: TouModifierIcons.Egotist.LoadAsset());

                notif1.Text.SetOutlineThickness(0.35f);
                notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
            }
            else
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    $"<b>The {OWColors.Egotist.ToTextColor()}Egotist</color>, {ego.Player.Data.PlayerName}, has successfully won, as no more crewmates remain!</b>",
                    Color.white, spr: TouModifierIcons.Egotist.LoadAsset());

                notif1.Text.SetOutlineThickness(0.35f);
                notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
            }
        }
    }
}