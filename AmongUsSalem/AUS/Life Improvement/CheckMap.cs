using System.Collections.Generic;
using System.IO;
using System.Linq;
using ObjectWorkshop.Patches;
using ObjectWorkshop.Roles;
using HarmonyLib;
using UnityEngine;

namespace ObjectWorkshop.LifeImprovement;

public static class CheckMap
{
    public static CurrentMap MapSelected;

    [HarmonyPatch(typeof(LobbyBehaviour), "Update")]
    public static class LobbyBehaviourUpdate
    {
        // Token: 0x06000A34 RID: 2612 RVA: 0x00047D38 File Offset: 0x00045F38
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (MapSelected != (CurrentMap)GameOptionsManager.Instance.currentNormalGameOptions.MapId)
            {
                MapSelected = (CurrentMap)GameOptionsManager.Instance.currentNormalGameOptions.MapId;
                ObjectWorkshopPlugin.DebugLogMessage($"Map set to {MapSelected}.", ObjectWorkshopPlugin.MsgType.Message);
            }
        }
    }

    public enum CurrentMap
    {
        Skeld,
        MiraHQ,
        Polus,
        Airship = 4,
        Fungle,
        Submerged,
        LevelImpostor
    }
}