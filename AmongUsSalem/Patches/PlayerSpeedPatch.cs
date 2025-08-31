using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using AmongUsSalem.Modifiers.Impostor.Venerer;
using AmongUsSalem.Options.Roles.Impostor;
using AmongUsSalem.Utilities.Appearances;

namespace AmongUsSalem.Patches;

[HarmonyPatch(typeof(LogicOptions), nameof(LogicOptions.GetPlayerSpeedMod))]
public static class PlayerSpeedPatch
{
    // ReSharper disable once InconsistentNaming
    public static void Postfix(PlayerControl pc, ref float __result)
    {
        __result *= pc.GetAppearance().Speed;


        if (pc.HasModifier<VenererSprintModifier>())
        {
            __result *= OptionGroupSingleton<VenererOptions>.Instance.NumSprintSpeed;
        }

        if (pc.TryGetModifier<VenererFreezeModifier>(out var freeze))
        {
            __result *= freeze.SpeedFactor;
        }
    }
}