using AmongUsSalem.LifeImprovement.Roles;
using AmongUsSalem.Modifiers.Impostor;
using AmongUsSalem.Modules;
using AmongUsSalem.Options;
using AmongUsSalem.Roles;
using AmongUsSalem.Utilities;
using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using UnityEngine;

namespace AmongUsSalem.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
public static class ShipStatus_CalculateLightRadius
{
    public static bool NerfMe { get; set; }

    public static void Postfix(ShipStatus __instance, NetworkedPlayerInfo player, ref float __result)
    {
        if (player == null || player.IsDead)
        {
            __result = __instance.MaxLightRadius;
            return;
        }

        var visionFactor = 1f;

        var playerControl = player._object;

        if (player.Role.IsImpostor)
        {
            __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod * visionFactor;
            return;
        }

        if (playerControl.Data.Role is ICustomAURole ausRole)
        {
            if (playerControl.Is(Faction.Coven))
            {
                __result = __instance.MaxLightRadius * OptionGroupSingleton<CovenOptions>.Instance.Vision;
                return;
            }
            else if (playerControl.IsRole<Shroud>())
            {
                __result = __instance.MaxLightRadius * OptionGroupSingleton<Shroud_Options>.Instance.Vision;
                return;
            }
            else if (playerControl.IsRole<Arsonist>())
            {
                __result = __instance.MaxLightRadius * OptionGroupSingleton<Arsonist_Options>.Instance.Vision;
                return;
            }
            else if (playerControl.IsRole<Vampire>())
            {
                __result = __instance.MaxLightRadius * OptionGroupSingleton<Vampire_Options>.Instance.Vision;
                return;
            }
            else if (playerControl.IsRole<Jackal>())
            {
                __result = __instance.MaxLightRadius * OptionGroupSingleton<Jackal_Options>.Instance.Vision;
                return;
            }
        }

        if (ModCompatibility.IsSubmerged())
        {
            __result *= visionFactor;
        }
        else
        {
            SwitchSystem? switchSystem = null;

            if (__instance.Systems != null && __instance.Systems.TryGetValue(SystemTypes.Electrical, out var system))
            {
                switchSystem = system.TryCast<SwitchSystem>();
            }

            var t = switchSystem?.Level ?? 1;

            __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, t) *
                       GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod * visionFactor;
        }

        if (NerfMe && !PlayerControl.LocalPlayer.HasDied())
        {
            __result /= 2;
        }
    }
}