using HarmonyLib;
using MiraAPI.GameOptions;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using TownOfUs.Options;
using TownOfUs.Utilities;

namespace TownOfUs.Patches.Options;

[HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Begin))]

public static class AirshipSpawnPatch
{
    private static List<StringNames> RemovedSpawns = [];

    [HarmonyPrefix]

    public static void Prefix(SpawnInMinigame __instance)
    {
        
    }

    static StringNames EnumToType()
    {
        return 0;
    }

    [MethodRpc((uint)AUSRpc.RemoveSpawns, SendImmediately = true)]

    public static void RemoveSpawns(PlayerControl player, StringNames location, StringNames location2, StringNames location3)
    {
        RemovedSpawns = [location, location2, location3];
    }
}