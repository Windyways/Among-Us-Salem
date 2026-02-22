namespace AmongUsSalem.MCI.SmartMCI;

[HarmonyPatch(typeof(IntroCutscene._CoBegin_d__35), nameof(IntroCutscene._CoBegin_d__35.MoveNext))]
public static class SmartCovenStart
{
    public static void Postfix(HudManager __instance)
    {
        if (AUSPlugin.InGame())
        {
            NecronomiconPatch.GrantCovenNecroPassing();
            if (Debugger.IsDebuggerActive && Debugger.smartSwapping)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Is(Faction.Coven) && OptionGroupSingleton<CovenOptions>.Instance.EnableNecroPassing)
                    {
                        InstanceControlPatches.SwitchTo(player.PlayerId);
                        break;
                    }
                }
            }
        }
    }
}
