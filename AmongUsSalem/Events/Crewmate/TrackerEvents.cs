using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Roles;
using AmongUsSalem.Buttons.Crewmate;
using AmongUsSalem.Options.Roles.Crewmate;
using AmongUsSalem.Roles.Crewmate;

namespace AmongUsSalem.Events.Crewmate;

public static class TrackerEvents
{
    [RegisterEvent]
    public static void CompleteTaskEvent(CompleteTaskEvent @event)
    {
        if (@event.Player.AmOwner && @event.Player.Data.Role is TrackerRole &&
            OptionGroupSingleton<TrackerOptions>.Instance.TaskUses &&
            !OptionGroupSingleton<TrackerOptions>.Instance.ResetOnNewRound)
        {
            var button = CustomButtonSingleton<TrackerTrackButton>.Instance;
            ++button.UsesLeft;
            ++button.ExtraUses;
            button.SetUses(button.UsesLeft);
        }
    }

    [RegisterEvent]
    public static void EjectionEventEventHandler(EjectionEvent @event)
    {
        if (!OptionGroupSingleton<TrackerOptions>.Instance.ResetOnNewRound)
        {
            return;
        }

        foreach (var tracker in CustomRoleUtils.GetActiveRolesOfType<TrackerTouRole>())
        {
            tracker.Clear();
        }

        var button = CustomButtonSingleton<TrackerTrackButton>.Instance;
        button.SetUses((int)OptionGroupSingleton<TrackerOptions>.Instance.MaxTracks);
    }
}