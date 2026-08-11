using HarmonyLib;
using TownOfUs.Modules;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.Patches.Misc;

[HarmonyPatch]
public static class ShowVentsPatch
{
    public static readonly List<List<Vent>> VentNetworks = [];

    public static readonly Dictionary<int, GameObject> VentIcons = [];
    public static readonly Dictionary<int, GameObject> BodyIcons = [];

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
    [HarmonyPostfix]
    public static void Postfix()
    {
        BodyIcons.Clear();
        VentIcons.Clear();
        VentNetworks.Clear();
    }

    public static List<Vent>? GetNetworkFor(Vent vent)
    {
        return VentNetworks.FirstOrDefault(x =>
            x.Any(y => y == vent || y == vent.Left || y == vent.Center || y == vent.Right));
    }

    public static bool AllVentsRegistered()
    {
        foreach (var vent in ShipStatus.Instance.AllVents)
        {
            if (!vent.isActiveAndEnabled)
            {
                continue;
            }

            if (vent.name.StartsWith("MinerVent-", StringComparison.Ordinal))
            {
                continue;
            }

            var network = GetNetworkFor(vent);
            if (network == null || !network.Any(x => x == vent))
            {
                return false;
            }
        }

        return true;
    }

    public static void HandleMiraOrSub()
    {
        if (VentNetworks.Count != 0)
        {
            return;
        }

        if (MiscUtils.IsMap(1))
        {
            var vents = ShipStatus.Instance.AllVents.Where(x => !x.name.Contains("MinerVent"));
            VentNetworks.Add(vents.ToList());
            return;
        }

        if (ShipStatus.Instance.Type == ModCompatibility.SubmergedMapType)
        {
            var vents = ShipStatus.Instance.AllVents.Where(x => x.Id is 12 or 13 or 15 or 16);
            VentNetworks.Add(vents.ToList());
        }
    }
}