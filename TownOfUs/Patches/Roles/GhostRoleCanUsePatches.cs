using System.Reflection;
using HarmonyLib;
using TownOfUs.Roles;
using Object = Il2CppSystem.Object;

namespace TownOfUs.Patches.Roles;

[HarmonyPatch]
public static class GhostRoleCanUsePatches
{
    public static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(Console), nameof(Console.CanUse));
        yield return AccessTools.Method(typeof(Ladder), nameof(Ladder.CanUse));
        yield return AccessTools.Method(typeof(PlatformConsole), nameof(PlatformConsole.CanUse));
        yield return AccessTools.Method(typeof(OpenDoorConsole), nameof(OpenDoorConsole.CanUse));
        yield return AccessTools.Method(typeof(DoorConsole), nameof(DoorConsole.CanUse));
        yield return AccessTools.Method(typeof(ZiplineConsole), nameof(ZiplineConsole.CanUse));
        yield return AccessTools.Method(typeof(DeconControl), nameof(DeconControl.CanUse));
    }

    [HarmonyPriority(Priority.Last)]
    [HarmonyPrefix]
    public static bool CanUsePrefixPatch(Object __instance, [HarmonyArgument(0)] NetworkedPlayerInfo pc,
        ref bool __state)
    {
        __state = false;
        var playerControl = pc.Object;

        if (playerControl.Data.Role is IGhostRole ghost && ghost.GhostActive && pc.IsDead)
        {
            // Logger<AUSPlugin>.Message($"CanUsePrefixPatch IsDead");
            pc.IsDead = false; // TODO: find a better way
            __state = true;
        }

        return true;
    }

    [HarmonyPriority(Priority.Last)]
    [HarmonyPostfix]
    public static void CanUsePostfixPatch(Object __instance, [HarmonyArgument(0)] NetworkedPlayerInfo pc,
        ref bool __state)
    {
        if (__state)
            // Logger<AUSPlugin>.Message($"CanUsePostfixPatch IsDead");
        {
            pc.IsDead = true;
        }
    }
}