using HarmonyLib;
using InnerNet;
using MiraAPI.GameOptions;
using TMPro;
using ObjectWorkshop.Options;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ObjectWorkshop.Patches;

[HarmonyPatch]
public static class GameTimerPatch
{
    public static bool TriggerEndGame { get; set; }
}