using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using TownOfUs.Patches;
using TownOfUs.Roles;

namespace TownOfUs.Events.Misc;

public static class ZoomEvents
{
    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            return;
        }

        if ((PlayerControl.LocalPlayer.Data.IsDead &&
             (PlayerControl.LocalPlayer.Data.Role is IGhostRole { Caught: true } ||
              PlayerControl.LocalPlayer.Data.Role is not IGhostRole)) ||
            TutorialManager.InstanceExists)
        {
            HudManagerPatches.ZoomButton.SetActive(true);
        }
    }

    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        if (TutorialManager.InstanceExists)
        {
            HudManagerPatches.ZoomButton.SetActive(true);
        }
    }

    [RegisterEvent]
    public static void StartMeetingEventHandler(StartMeetingEvent @event)
    {
        HudManagerPatches.ResetZoom();
        HudManagerPatches.ZoomButton.SetActive(false);
    }
}