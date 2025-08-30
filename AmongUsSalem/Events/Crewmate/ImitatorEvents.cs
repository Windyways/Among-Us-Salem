using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Modifiers;
using Reactor.Utilities;
using ObjectWorkshop.Events.TouEvents;
using ObjectWorkshop.Modifiers.Crewmate;
using ObjectWorkshop.Utilities;

namespace ObjectWorkshop.Events.Crewmate;

public static class ImitatorEvents
{
    [RegisterEvent(1001)]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            return;
        }

        var imitators = ModifierUtils.GetActiveModifiers<ImitatorCacheModifier>();

        if (!imitators.Any())
        {
            return;
        }

        foreach (var mod in imitators)
        {
            if (mod.Player.AmOwner)
            {
                mod.UpdateRole();
            }
        }
    }

    [RegisterEvent]
    public static void ChangeRoleHandler(ChangeRoleEvent @event)
    {
        if (!PlayerControl.LocalPlayer)
        {
            return;
        }

        var player = @event.Player;

        if (player.HasModifier<ImitatorCacheModifier>() && !@event.NewRole.IsCrewmate())
        {
            if (ObjectWorkshopPlugin.IsDevBuild) Logger<ObjectWorkshopPlugin>.Error($"Removed Imitator Cache Modifier On Role Change");
            player.RemoveModifier<ImitatorCacheModifier>();
        }
    }
}