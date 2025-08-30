using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using ObjectWorkshop.Modifiers;
using ObjectWorkshop.Options;
using ObjectWorkshop.Options.Modifiers.Alliance;
using ObjectWorkshop.Utilities;
using ObjectWorkshop.Utilities.Appearances;

namespace ObjectWorkshop.Patches.Options;

// Is there a better way I can do this??
[HarmonyPatch(typeof(ImpostorRole), nameof(ImpostorRole.IsValidTarget))]
public static class ImpostorTargeting
{
    public static void Postfix(ImpostorRole __instance, NetworkedPlayerInfo target, ref bool __result)
    {
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
        var loveOpt = OptionGroupSingleton<LoversOptions>.Instance;

        __result &=
            !(!loveOpt.LoversKillEachOther && target?.Object?.IsLover() == true && PlayerControl.LocalPlayer.IsLover()) &&
            !(target?.Object?.TryGetModifier<DisabledModifier>(out var mod) == true && !mod.CanBeInteractedWith) &&
            (target?.Object?.IsImpostor() == false ||
             genOpt.FFAImpostorMode ||
             (PlayerControl.LocalPlayer.IsLover() && loveOpt.LoverKillTeammates));
    }
}