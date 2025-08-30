using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using ObjectWorkshop.Buttons.Crewmate;
using ObjectWorkshop.Options.Roles.Crewmate;

namespace ObjectWorkshop.Events.Crewmate;

public static class EngineerEvents
{
    [RegisterEvent]
    public static void CompleteTaskEvent(CompleteTaskEvent @event)
    {
        if (@event.Player.AmOwner && @event.Player.Data.Role is EngineerRole &&
            OptionGroupSingleton<EngineerOptions>.Instance.TaskUses &&
            (int)OptionGroupSingleton<EngineerOptions>.Instance.MaxVents != 0)
        {
            var button = CustomButtonSingleton<EngineerVentButton>.Instance;
            ++button.UsesLeft;
            ++button.ExtraUses;
            button.SetUses(button.UsesLeft);
        }
    }
}