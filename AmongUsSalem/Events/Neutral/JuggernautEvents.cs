using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Hud;
using AmongUsSalem.Buttons.Neutral;
using AmongUsSalem.Roles.Neutral;

namespace AmongUsSalem.Events.Neutral;

public static class JuggernautEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;
        if (!source.AmOwner || source.Data.Role is not JuggernautRole juggernaut || MeetingHud.Instance)
        {
            return;
        }

        juggernaut.KillCount++;
        CustomButtonSingleton<JuggernautKillButton>.Instance.ResetCooldownAndOrEffect();
    }
}