using AmongUs.GameOptions;
using Hazel;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace AmongUsSalem.LifeImprovement;

public static class DisableTasks
{
    public static void DoRemove(ref List<NormalPlayerTask> usedTasks)
    {
        //if (!Options.DisableShortTasks.GetBool() && !Options.DisableCommonTasks.GetBool() && !Options.DisableLongTasks.GetBool() && !Options.DisableOtherTasks.GetBool()) return;

        List<NormalPlayerTask> disabledTasks = [];

        var s = OptionGroupSingleton<TaskOptions>.Instance;
        foreach (var task in usedTasks)
        {
            switch (task.TaskType)
            {
                //case TaskTypes.SwipeCard when Options.DisableSwipeCard.GetBool():
                //case TaskTypes.SubmitScan when Options.DisableSubmitScan.GetBool():
                //case TaskTypes.UnlockSafe when Options.DisableUnlockSafe.GetBool():
                case TaskTypes.UploadData when s.DisableDownloadUpload:
                //case TaskTypes.StartReactor when Options.DisableStartReactor.GetBool():
                //case TaskTypes.ResetBreakers when Options.DisableResetBreaker.GetBool():
                //case TaskTypes.VentCleaning when Options.DisableCleanVent.GetBool():
                //case TaskTypes.CalibrateDistributor when Options.DisableCalibrateDistributor.GetBool():
                //case TaskTypes.ChartCourse when Options.DisableChartCourse.GetBool():
                //case TaskTypes.StabilizeSteering when Options.DisableStabilizeSteering.GetBool():
                //case TaskTypes.CleanO2Filter when Options.DisableCleanO2Filter.GetBool():
                //case TaskTypes.UnlockManifolds when Options.DisableUnlockManifolds.GetBool():
                //case TaskTypes.PrimeShields when Options.DisablePrimeShields.GetBool():
                //case TaskTypes.MeasureWeather when Options.DisableMeasureWeather.GetBool():
                //case TaskTypes.BuyBeverage when Options.DisableBuyBeverage.GetBool():
                //case TaskTypes.AssembleArtifact when Options.DisableAssembleArtifact.GetBool():
                //case TaskTypes.ProcessData when Options.DisableProcessData.GetBool():
                //case TaskTypes.RunDiagnostics when Options.DisableRunDiagnostics.GetBool():
                //case TaskTypes.RepairDrill when Options.DisableRepairDrill.GetBool():
                //case TaskTypes.AlignTelescope when Options.DisableAlignTelescope.GetBool():
                //case TaskTypes.RecordTemperature when Options.DisableRecordTemperature.GetBool():
                //case TaskTypes.FillCanisters when Options.DisableFillCanisters.GetBool():
                //case TaskTypes.MonitorOxygen when Options.DisableMonitorTree.GetBool():
                //case TaskTypes.StoreArtifacts when Options.DisableStoreArtifacts.GetBool():
                //case TaskTypes.PutAwayPistols when Options.DisablePutAwayPistols.GetBool():
                //case TaskTypes.PutAwayRifles when Options.DisablePutAwayRifles.GetBool():
                //case TaskTypes.MakeBurger when Options.DisableMakeBurger.GetBool():
                //case TaskTypes.CleanToilet when Options.DisableCleanToilet.GetBool():
                //case TaskTypes.Decontaminate when Options.DisableDecontaminate.GetBool():
                //case TaskTypes.SortRecords when Options.DisableSortRecords.GetBool():
                //case TaskTypes.FixShower when Options.DisableFixShower.GetBool():
                //case TaskTypes.PickUpTowels when Options.DisablePickUpTowels.GetBool():
                //case TaskTypes.PolishRuby when Options.DisablePolishRuby.GetBool():
                //case TaskTypes.DressMannequin when Options.DisableDressMannequin.GetBool():
                //case TaskTypes.AlignEngineOutput when Options.DisableAlignEngineOutput.GetBool():
                //case TaskTypes.InspectSample when Options.DisableInspectSample.GetBool():
                //case TaskTypes.EmptyChute when Options.DisableEmptyChute.GetBool():
                //case TaskTypes.ClearAsteroids when Options.DisableClearAsteroids.GetBool():
                //case TaskTypes.WaterPlants when Options.DisableWaterPlants.GetBool():
                //case TaskTypes.OpenWaterways when Options.DisableOpenWaterways.GetBool():
                //case TaskTypes.ReplaceWaterJug when Options.DisableReplaceWaterJug.GetBool():
                //case TaskTypes.RebootWifi when Options.DisableRebootWifi.GetBool():
                //case TaskTypes.DevelopPhotos when Options.DisableDevelopPhotos.GetBool():
                //case TaskTypes.RewindTapes when Options.DisableRewindTapes.GetBool():
                //case TaskTypes.StartFans when Options.DisableStartFans.GetBool():
                //case TaskTypes.FixWiring when Options.DisableFixWiring.GetBool():
                //case TaskTypes.EnterIdCode when Options.DisableEnterIdCode.GetBool():
                //case TaskTypes.InsertKeys when Options.DisableInsertKeys.GetBool():
                //case TaskTypes.ScanBoardingPass when Options.DisableScanBoardingPass.GetBool():
                //case TaskTypes.EmptyGarbage when Options.DisableEmptyGarbage.GetBool():
                //case TaskTypes.FuelEngines when Options.DisableFuelEngines.GetBool():
                case TaskTypes.DivertPower when s.DisableDivertPower:
                //case TaskTypes.FixWeatherNode when Options.DisableActivateWeatherNodes.GetBool(): // Activate Weather Nodes
                //case TaskTypes.RoastMarshmallow when Options.DisableRoastMarshmallow.GetBool():
                //case TaskTypes.CollectSamples when Options.DisableCollectSamples.GetBool():
                //case TaskTypes.ReplaceParts when Options.DisableReplaceParts.GetBool():
                //case TaskTypes.CollectVegetables when Options.DisableCollectVegetables.GetBool():
                //case TaskTypes.MineOres when Options.DisableMineOres.GetBool():
                //case TaskTypes.ExtractFuel when Options.DisableExtractFuel.GetBool():
                //case TaskTypes.CatchFish when Options.DisableCatchFish.GetBool():
                //case TaskTypes.PolishGem when Options.DisablePolishGem.GetBool():
                //case TaskTypes.HelpCritter when Options.DisableHelpCritter.GetBool():
                //case TaskTypes.HoistSupplies when Options.DisableHoistSupplies.GetBool():
                //case TaskTypes.FixAntenna when Options.DisableFixAntenna.GetBool():
                //case TaskTypes.BuildSandcastle when Options.DisableBuildSandcastle.GetBool():
                //case TaskTypes.CrankGenerator when Options.DisableCrankGenerator.GetBool():
                //case TaskTypes.MonitorMushroom when Options.DisableMonitorMushroom.GetBool():
                //case TaskTypes.PlayVideogame when Options.DisablePlayVideoGame.GetBool():
                //case TaskTypes.TuneRadio when Options.DisableFindSignal.GetBool(): // Find Signal
                //case TaskTypes.TestFrisbee when Options.DisableThrowFisbee.GetBool(): // Throw Fisbee
                //case TaskTypes.LiftWeights when Options.DisableLiftWeights.GetBool():
                //case TaskTypes.CollectShells when Options.DisableCollectShells.GetBool():
                    disabledTasks.Add(task);
                    break;
            }
        }
        foreach (var task in disabledTasks.ToArray())
        {
            AUSPlugin.DebugLogMessage($"Deletion: {task.TaskType}" + " - Disable Tasks");
            usedTasks.Remove(task);
        }
    }
}

[HarmonyPatch(typeof(NetworkedPlayerInfo), nameof(NetworkedPlayerInfo.RpcSetTasks))]
public static class RpcSetTasksPatch
{
    // Patch to overwrite the task just before the process of allocating the task and sending the RPC is performed
    // Does not interfere with the vanilla task allocation process itself

    /* TO DO:
     * Try to make players get different tasks from each other
     * InnerSloth uses task pool to achieve this.
     */

    public static List<byte> decidedCommonTasks = [];
    public static bool Prefix(NetworkedPlayerInfo __instance)
    {
        if (!AmongUsClient.Instance.AmHost) return false;
       //  if (GameStates.IsHideNSeek) return true;

        var player = __instance.Object;
        var roleNullable = player.Data.Role;
        if (roleNullable is not ICustomAURole) return true;
        //var role = player.Data.Role as ICustomAURole;

        // Default number of tasks
        bool hasCommonTasks = true;
        int NumLongTasks = GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks;
        int NumShortTasks = GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks;

        /*var owlSettings = OptionGroupSingleton<Owl_Options>.Instance;
        if (role is Owl && owlSettings.OverrideTasks)
        {
            // whether to assign a common task (normal task) or not.
            // If assigned, it will not be reassigned and the same common task will be assigned as the other crew members.
            hasCommonTasks = false; // Add a setting for this later perhaps

            NumLongTasks = owlSettings.LongTasks;
            NumShortTasks = owlSettings.ShortTasks;

            // Long and short tasks are always reallocated.
        }*/

        // Above is override task num
        /* --------------------------------------------------------------*/
        //Below is assign tasks

        // We completely igonre the tasks decided by ShipStatus and assign our own.
        List<NormalPlayerTask> commonTasks = ShipStatus.Instance.CommonTasks.Shuffle().ToList();
        List<NormalPlayerTask> shortTasks = ShipStatus.Instance.ShortTasks.Shuffle().ToList();
        List<NormalPlayerTask> longTasks = ShipStatus.Instance.LongTasks.Shuffle().ToList();

        if (!GameManager.Instance.LogicOptions.GetVisualTasks())
        {
            shortTasks.RemoveAll(x => x.TaskType == TaskTypes.SubmitScan);
            longTasks.RemoveAll(x => x.TaskType == TaskTypes.SubmitScan);
            // Niko admits this is shit.
        }

        // Remove all disabled tasks
        DisableTasks.DoRemove(ref commonTasks);
        DisableTasks.DoRemove(ref shortTasks);
        DisableTasks.DoRemove(ref longTasks);

        List<TaskTypes> usedTaskTypes = [];

        int defaultcommoncount = GameOptionsManager.Instance.currentNormalGameOptions.NumCommonTasks;
        int commonTasksNum = System.Math.Min(commonTasks.Count, defaultcommoncount);

        // Setting task num to 0 will make role description disappear from task panel for vanilla players and mod crews
        if (!hasCommonTasks && NumShortTasks + NumLongTasks < 1)
        {
            NumShortTasks = 1;
        }

        if (decidedCommonTasks.Count < 1)
        {
            for (int i = 0; i < commonTasksNum; i++)
            {
                decidedCommonTasks.Add((byte)commonTasks[i].Index);
            }
        }

        Il2CppSystem.Collections.Generic.List<byte> TasksList = new();

        if (hasCommonTasks)
        {
            if (__instance.Object != null)
            {
                if (__instance.Object.Is(Faction.Town))
                {
                    foreach (var id in decidedCommonTasks)
                        TasksList.Add(id);
                }
                else
                {
                    for (int i = 0; i < commonTasksNum; i++)
                    {
                        TasksList.Add((byte)commonTasks[i].Index);
                    }
                }
            }
            else
            {
                for (int i = 0; i < commonTasksNum; i++)
                {
                    TasksList.Add((byte)commonTasks[i].Index);
                }
            }
        }

        byte list = 0; byte assigned = 0;
        while (assigned < System.Math.Min(longTasks.Count, NumLongTasks))
        {
            if (!TasksList.Contains((byte)longTasks[list].Index))
            {
                if (usedTaskTypes.Contains(longTasks[list].TaskType))
                {
                    list++;
                }
                else
                {
                    usedTaskTypes.Add(longTasks[list].TaskType);
                    TasksList.Add((byte)longTasks[list].Index);
                    assigned++;
                    list++;
                }
            }
            else
            {
                list++;
            }

            if (list >= longTasks.Count - 1)
            {
                list = 0;
                longTasks.Shuffle();
                longTasks = longTasks.ToList();
                usedTaskTypes.Clear();
            }
        }

        list = 0; assigned = 0;

        usedTaskTypes.Clear();
        foreach (var task in longTasks)
        {
            if (TasksList.Contains((byte)task.Index))
            {
                if (!usedTaskTypes.Contains(task.TaskType))
                    usedTaskTypes.Add(task.TaskType);
            }
        }

        while (assigned < System.Math.Min(shortTasks.Count, NumShortTasks))
        {
            if (!TasksList.Contains((byte)shortTasks[list].Index))
            {
                if (usedTaskTypes.Contains(shortTasks[list].TaskType))
                {
                    list++;
                }
                else
                {
                    usedTaskTypes.Add(shortTasks[list].TaskType);
                    TasksList.Add((byte)shortTasks[list].Index);
                    assigned++;
                    list++;
                }
            }
            else
            {
                list++;
            }

            if (list >= shortTasks.Count - 1)
            {
                list = 0;
                shortTasks.Shuffle();
                shortTasks = shortTasks.ToList();
                usedTaskTypes.Clear();
            }
        }

        if (AmongUsClient.Instance.AmClient)
        {
            __instance.SetTasks((Il2CppStructArray<byte>)TasksList.ToArray());
        }

        MessageWriter messageWriter = AmongUsClient.Instance.StartRpcImmediately(__instance.NetId, (byte)RpcCalls.SetTasks, SendOption.Reliable);
        messageWriter.WriteBytesAndSize((Il2CppStructArray<byte>)TasksList.ToArray());
        messageWriter.EndMessage();
        return false;
    }
}