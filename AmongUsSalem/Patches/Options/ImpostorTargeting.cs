using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using AmongUsSalem.Modifiers;
using AmongUsSalem.Options;
using AmongUsSalem.Options.Modifiers.Alliance;
using AmongUsSalem.Utilities;
using AmongUsSalem.Utilities.Appearances;

namespace AmongUsSalem.Patches.Options;

// Is there a better way I can do this??
[HarmonyPatch(typeof(ImpostorRole), nameof(ImpostorRole.IsValidTarget))]
public static class ImpostorTargeting
{
    public static void Postfix(ImpostorRole __instance, NetworkedPlayerInfo target, ref bool __result)
    {
        var loveOpt = OptionGroupSingleton<LoversOptions>.Instance;

        __result &=
            !(!loveOpt.LoversKillEachOther && target?.Object?.IsLover() == true && PlayerControl.LocalPlayer.IsLover()) &&
            !(target?.Object?.TryGetModifier<DisabledModifier>(out var mod) == true && !mod.CanBeInteractedWith) &&
            (target?.Object?.IsImpostor() == false ||
             (PlayerControl.LocalPlayer.IsLover() && loveOpt.LoverKillTeammates));
    }
}