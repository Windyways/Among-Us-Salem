namespace AmongUsSalem.MCI.SmartMCI;

[HarmonyPatch(typeof(IntroCutscene._CoBegin_d__35), nameof(IntroCutscene._CoBegin_d__35.MoveNext))]
public static class SmartCovenStart
{
    public static void Postfix(HudManager __instance)
    {
        if (AUSPlugin.InGame())
        {
            NecronomiconPatch.GrantCovenNecroPassing();
            SmartClientSwapping.SwapToCoven();
        }
    }
}
