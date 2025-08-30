using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using ObjectWorkshop.Buttons.Impostor;
using ObjectWorkshop.Options.Roles.Impostor;

namespace ObjectWorkshop.Events.Impostor;

public static class SwooperEvents
{
    [RegisterEvent]
    public static void EjectionEventEventHandler(EjectionEvent @event)
    {
        var button = CustomButtonSingleton<SwooperSwoopButton>.Instance;
        button.SetUses((int)OptionGroupSingleton<SwooperOptions>.Instance.MaxSwoops);

        if ((int)OptionGroupSingleton<SwooperOptions>.Instance.MaxSwoops == 0)
        {
            button.Button?.usesRemainingText.gameObject.SetActive(false);
            button.Button?.usesRemainingSprite.gameObject.SetActive(false);
        }
        else
        {
            button.Button?.usesRemainingText.gameObject.SetActive(true);
            button.Button?.usesRemainingSprite.gameObject.SetActive(true);
        }
    }
}