using System.Collections;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using Reactor.Utilities;
using ObjectWorkshop.Modifiers.Neutral;
using ObjectWorkshop.Options.Roles.Neutral;
using ObjectWorkshop.Roles.Neutral;
using UnityEngine;
using Object = UnityEngine.Object;
using Color = UnityEngine.Color;

namespace ObjectWorkshop.Events.Neutral;

public static class AmnesiacEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        if (!CustomRoleUtils.GetActiveRolesOfType<AmnesiacRole>().Any())
        {
            return;
        }

        if (!OptionGroupSingleton<AmnesiacOptions>.Instance.RememberArrows)
        {
            return;
        }

        Coroutines.Start(CoCreateArrow(@event.Target));
    }

    private static IEnumerator CoCreateArrow(PlayerControl target)
    {
        yield return new WaitForSeconds(OptionGroupSingleton<AmnesiacOptions>.Instance.RememberArrowDelay.Value);

        var deadBody = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == target.PlayerId);

        if (deadBody == null)
        {
            yield break;
        }

        foreach (var amne in CustomRoleUtils.GetActiveRolesOfType<AmnesiacRole>().Select(x => x.Player))
        {
            if (amne.AmOwner)
            {
                amne.AddModifier<AmnesiacArrowModifier>(deadBody, Color.white);
            }
        }
    }
}