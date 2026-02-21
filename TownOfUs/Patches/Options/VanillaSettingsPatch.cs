using AmongUs.GameOptions;
using HarmonyLib;

namespace TownOfUs.Patches.Options;

[HarmonyPatch]
public static class VanillaSettingsPatch
{
    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.CreateSettings))]
    [HarmonyPostfix]
    public static void Postfix(GameOptionsMenu __instance)
    {
        if (__instance.gameObject.name == "GAME SETTINGS TAB")
        {
            try
            {
                var commonTasks = __instance.Children.ToArray()
                    ?.FirstOrDefault(x => x.TryCast<NumberOption>()?.intOptionName == Int32OptionNames.NumCommonTasks)
                    ?.Cast<NumberOption>();
                if (commonTasks != null)
                {
                    commonTasks.ValidRange = new FloatRange(0f, 4f);
                }

                var shortTasks = __instance.Children.ToArray()
                    ?.FirstOrDefault(x => x.TryCast<NumberOption>()?.intOptionName == Int32OptionNames.NumShortTasks)
                    ?.Cast<NumberOption>();
                if (shortTasks != null)
                {
                    shortTasks.ValidRange = new FloatRange(0f, 8f);
                }

                var longTasks = __instance.Children.ToArray()
                    ?.FirstOrDefault(x => x.TryCast<NumberOption>()?.intOptionName == Int32OptionNames.NumLongTasks)
                    ?.Cast<NumberOption>();
                if (longTasks != null)
                {
                    longTasks.ValidRange = new FloatRange(0f, 4f);
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}