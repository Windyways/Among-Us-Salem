using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using ObjectWorkshop.Modifiers.Game.Crewmate;
using ObjectWorkshop.Modifiers.Impostor;
using ObjectWorkshop.Modules;
using ObjectWorkshop.Options;
using ObjectWorkshop.Roles;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Patches;

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
        
        var darkCloud = DarkCloud.GetAll();
        if (darkCloud != null)
        {
            if (darkCloud.IsInRange(playerControl) && !playerControl.IsRole<Canopy>())
            {
                __result = __instance.MaxLightRadius * 0f;
                return;
            }
        }

        if (playerControl.IsDueling())
        {
            __result = __instance.MaxLightRadius * 0.25f;
            return;
        }

        if (playerControl.IsOnFire())
        {
            __result = __instance.MaxLightRadius * 1f;
            return;
        }

        if (player._object.Data.Role is IOWRole owRole)
        {
            if (owRole.Team == ModdedRoleTeams.Custom)
            {
                __result = __instance.MaxLightRadius * owRole.visionValue;
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


            if (player._object.HasModifier<TorchModifier>() && !player._object.HasModifier<EclipsalBlindModifier>())
            {
                t = 1;
            }

            __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, t) *
                       GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod * visionFactor;
        }

        if (NerfMe && !PlayerControl.LocalPlayer.HasDied())
        {
            __result /= 2;
        }
    }
}