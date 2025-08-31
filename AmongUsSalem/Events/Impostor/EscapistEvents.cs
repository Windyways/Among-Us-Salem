using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Roles;
using AmongUsSalem.Roles.Impostor;

namespace AmongUsSalem.Events.Impostor;

public static class EscapistEvents
{
    [RegisterEvent]
    public static void EjectionEventEventHandler(EjectionEvent @event)
    {
        CustomRoleUtils.GetActiveRolesOfType<EscapistRole>().Do(x => x.MarkedLocation = null);
    }
}