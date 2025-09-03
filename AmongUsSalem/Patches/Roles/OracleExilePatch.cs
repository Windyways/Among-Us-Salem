using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MiraAPI.Modifiers;
using AmongUsSalem.Modifiers.Crewmate;
using AmongUsSalem.Utilities.Appearances;
using Object = Il2CppSystem.Object;

namespace AmongUsSalem.Patches.Roles;

[HarmonyPatch]
public static class OracleExilePatch
{
    [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames), typeof(Il2CppReferenceArray<Object>))]
    [HarmonyPostfix]
    public static void TranslationControllerGetStringPostfix(ref string __result, [HarmonyArgument(0)] StringNames name)
    {
        if (ExileController.Instance == null)
        {
            return;
        }

        if (ExileController.Instance.initData.networkedPlayer != null)
        {
            return;
        }

        foreach (var mod in ModifierUtils.GetActiveModifiers<OracleBlessedModifier>())
        {
            if (!mod.SavedFromExile)
            {
                continue;
            }

            mod.SavedFromExile = false;

            __result = $"{mod.Player.GetDefaultAppearance().PlayerName} was blessed by an Oracle!";
        }
    }
}