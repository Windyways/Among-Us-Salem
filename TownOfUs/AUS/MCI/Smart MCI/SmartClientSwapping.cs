using Reactor.Utilities.Extensions;

namespace AmongUsSalem.MCI.SmartMCI;

public static class SmartClientSwapping
{
    public static void SwapToCoven()
    {
        if (Debugger.IsDebuggerActive && Debugger.smartSwapping)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!player.HasDied() && player.Is(Faction.Coven) && OptionGroupSingleton<CovenOptions>.Instance.EnableNecroPassing)
                {
                    InstanceControlPatches.SwitchTo(player.PlayerId);
                    break;
                }
            }
        }
    }

    public static void RoundStart()
    {
        if (Debugger.IsDebuggerActive)
        {
            Keyboard_Joystick.RefreshSwapTargets();
            if (Debugger.smartSwapping)
            {
                Keyboard_Joystick.Switch(true);
            }
        }
    }
}
