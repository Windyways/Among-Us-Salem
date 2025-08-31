using System;
using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement;

[HarmonyPatch(typeof(IntroCutscene._CoBegin_d__35), nameof(IntroCutscene._CoBegin_d__35.MoveNext))]
public static class OnGameStart
{
    public static void Postfix(HudManager __instance)
    {
        if (AUSPlugin.InGame())
        {
            SequenceCheck++;
            AUSPlugin.DebugLogMessage("Sequence Check: " + SequenceCheck, AUSPlugin.MsgType.Message);

            if (SequenceCheck == 3)
            {
                Sabotages.Start();
                Sabotages.TDoors.Start();
            }
        }
    }

    public static int SequenceCheck;
}
