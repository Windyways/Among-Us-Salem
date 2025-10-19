using AmongUs.GameOptions;
using HarmonyLib;
using Hazel;
using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Roles;
using Reactor.Utilities;
using AmongUsSalem.Events.TouEvents;
using AmongUsSalem.Options;
using AmongUsSalem.Roles;
using AmongUsSalem.Roles.Neutral;
using AmongUsSalem.Utilities;
using Random = UnityEngine.Random;

namespace AmongUsSalem.Patches;

[HarmonyPatch]
public static class TouRoleManagerPatches
{
    private static readonly List<RoleTypes> CrewmateGhostRolePool = [];
    private static readonly List<RoleTypes> ImpostorGhostRolePool = [];
    private static readonly List<RoleTypes> CustomGhostRolePool = [];

    public static bool ReplaceRoleManager;
    private static List<int> LastImps { get; set; } = [];

    private static void GhostRoleSetup()
    {
        // var ghostRoles = RoleManager.Instance.AllRoles.Where(x => x.IsDead);
        var ghostRoles = MiscUtils.GetRegisteredGhostRoles();

        if (AUSPlugin.IsDevBuild) Logger<AUSPlugin>.Warning($"GhostRoleSetup - ghostRoles Count: {ghostRoles.Count()}");
        CrewmateGhostRolePool.Clear();
        ImpostorGhostRolePool.Clear();
        CustomGhostRolePool.Clear();

        foreach (var role in ghostRoles)
        {
            if (AUSPlugin.IsDevBuild) Logger<AUSPlugin>.Warning($"GhostRoleSetup - ghostRoles role NiceName: {role.NiceName}");
            var data = MiscUtils.GetAssignData(role.Role);

            switch (data.Chance)
            {
                case 100:
                {
                    if (data.Count > 0)
                    {
                        if (role is ICustomRole { Team: ModdedRoleTeams.Custom })
                        {
                            CustomGhostRolePool.Add(role.Role);
                        }
                        else
                        {
                            switch (role.TeamType)
                            {
                                case RoleTeamTypes.Crewmate:
                                    CrewmateGhostRolePool.Add(role.Role);
                                    break;
                                case RoleTeamTypes.Impostor:
                                    ImpostorGhostRolePool.Add(role.Role);
                                    break;
                            }
                        }
                    }

                    break;
                }
                case > 0:
                {
                    if (data.Count > 0 && HashRandom.Next(101) < data.Chance)
                    {
                        if (role is ICustomRole { Team: ModdedRoleTeams.Custom })
                        {
                            CustomGhostRolePool.Add(role.Role);
                        }
                        else
                        {
                            switch (role.TeamType)
                            {
                                case RoleTeamTypes.Crewmate:
                                    CrewmateGhostRolePool.Add(role.Role);
                                    break;
                                case RoleTeamTypes.Impostor:
                                    ImpostorGhostRolePool.Add(role.Role);
                                    break;
                            }
                        }
                    }

                    break;
                }
            }
        }
    }

    private static void AssignRolesFromRoleList(List<NetworkedPlayerInfo> infected)
    {
        var impostors = MiscUtils.GetImpostors(infected);
        var crewmates = MiscUtils.GetCrewmates(impostors);

        var townRoles = new List<ushort>();
        var neutralRoles = new List<ushort>();
        var mafiaRoles = new List<ushort>();
        var covenRoles = new List<ushort>();
        var traitorRoles = new List<ushort>();

        var opts = OptionGroupSingleton<RoleOptions>.Instance;

        // sort out bad lists
        var players = impostors.Count + crewmates.Count;
        List<RoleListOption> crewNkBuckets =
        [
            RoleListOption.TownInvestigative, RoleListOption.TownKilling, RoleListOption.TownOutlier,
            RoleListOption.TownPower, RoleListOption.TownProtective, RoleListOption.TownSupport,
            RoleListOption.TownUtility,
            RoleListOption.RandomTown, RoleListOption.CommonTown, 
            RoleListOption.NeutralKilling
        ];
        List<RoleListOption> impBuckets =
        [
            RoleListOption.MafiaDeception, RoleListOption.MafiaKilling, RoleListOption.MafiaSupport,
            RoleListOption.RandomMafia, RoleListOption.CommonMafia
        ];
        List<RoleListOption> buckets =
        [
            (RoleListOption)opts.Slot1.Value, (RoleListOption)opts.Slot2.Value, (RoleListOption)opts.Slot3.Value,
            (RoleListOption)opts.Slot4.Value
        ];

        var impCount = 0;
        var anySlots = 0;

        if (players > 4) buckets.Add((RoleListOption)opts.Slot5.Value);
        if (players > 5) buckets.Add((RoleListOption)opts.Slot6.Value);
        if (players > 6) buckets.Add((RoleListOption)opts.Slot7.Value);
        if (players > 7) buckets.Add((RoleListOption)opts.Slot8.Value);
        if (players > 8) buckets.Add((RoleListOption)opts.Slot9.Value);
        if (players > 9) buckets.Add((RoleListOption)opts.Slot10.Value);
        if (players > 10) buckets.Add((RoleListOption)opts.Slot11.Value);
        if (players > 11) buckets.Add((RoleListOption)opts.Slot12.Value);
        if (players > 12) buckets.Add((RoleListOption)opts.Slot13.Value);
        if (players > 13) buckets.Add((RoleListOption)opts.Slot14.Value);
        if (players > 14) buckets.Add((RoleListOption)opts.Slot15.Value);
        if (players > 15)
        {
            for (var i = 0; i < players - 15; i++)
            {
                var random = Random.RandomRangeInt(0, 4);
                buckets.Add(random == 0 ? RoleListOption.RandomTown : RoleListOption.NotMafia);
            }
        }

        // imp issues
        foreach (var roleOption in buckets)
        {
            if (impBuckets.Contains(roleOption))
            {
                impCount += 1;
            }
            else if (roleOption == RoleListOption.Any)
            {
                anySlots += 1;
            }
        }

        while (impCount > impostors.Count)
        {
            buckets.Shuffle();
            buckets.Remove(buckets.FindLast(x => impBuckets.Contains(x)));
            buckets.Add(RoleListOption.NotMafia);
            impCount -= 1;
        }

        while (impCount + anySlots < impostors.Count)
        {
            buckets.Shuffle();
            buckets.RemoveAt(0);
            buckets.Add(RoleListOption.RandomMafia);
            impCount += 1;
        }

        while (buckets.Contains(RoleListOption.Any))
        {
            buckets.Shuffle();
            buckets.Remove(buckets.FindLast(x => x == RoleListOption.Any));
            if (impCount < impostors.Count)
            {
                buckets.Add(RoleListOption.RandomMafia);
                impCount += 1;
            }
            else
            {
                buckets.Add(RoleListOption.NotMafia);
            }
        }

        // crew and neut issues
        var noChange = false;
        var notInfiltrator = false;
        var randNeut = false;

        foreach (var roleOption in buckets)
        {
            if (crewNkBuckets.Contains(roleOption))
            {
                noChange = true;
                break;
            }

            if (roleOption == RoleListOption.RandomNeutral)
            {
                randNeut = true;
                break;
            }

            if (roleOption == RoleListOption.NotMafia)
            {
                notInfiltrator = true;
                break;
            }
        }

        if (!noChange)
        {
            List<RoleListOption> add = [RoleListOption.RandomTown, RoleListOption.NeutralKilling];
            add.Shuffle();

            if (randNeut)
            {
                buckets.Remove(RoleListOption.RandomNeutral);
                buckets.Add(RoleListOption.NeutralKilling);
            }
            else if (notInfiltrator)
            {
                buckets.Remove(RoleListOption.NotMafia);
                buckets.Add(add[0]);
            }
            else
            {
                buckets.Remove(buckets.FindLast(x => !impBuckets.Contains(x)));
                buckets.Add(add[0]);
            }
        }

        Func<RoleBehaviour, bool>? roleFilter = null;
        if ((MapNames)GameOptionsManager.Instance.GameHostOptions.MapId == MapNames.Fungle)
        {
            //roleFilter = x => x.Role != (RoleTypes)RoleId.Get<SpyRole>();
        }

        var excluded = MiscUtils.AllRoles.Where(x => x is ISpawnChange { NoSpawn: true }).Select(x => x.Role).ToList();

        // traitor buckets
        var traitorDeceptiveRoles = MiscUtils.GetRolesToAssign(Alignment.TraitorDeceptive, roleFilter);
        var traitorPowerRoles = MiscUtils.GetRolesToAssign(Alignment.TraitorPower, roleFilter);
        var traitorUtilityRoles = MiscUtils.GetRolesToAssign(Alignment.TraitorUtility, roleFilter);

        traitorRoles.AddRange(MiscUtils.ReadFromBucket(buckets, traitorDeceptiveRoles, RoleListOption.TraitorDeceptive, RoleListOption.CommonTraitor));
        traitorRoles.AddRange(MiscUtils.ReadFromBucket(buckets, traitorUtilityRoles, RoleListOption.TraitorUtility, RoleListOption.CommonTraitor));
        var commonTraitorRoles = traitorDeceptiveRoles;
        commonTraitorRoles.AddRange(traitorUtilityRoles);

        traitorRoles.AddRange(MiscUtils.ReadFromBucket(buckets, traitorPowerRoles, RoleListOption.TraitorPower, RoleListOption.RandomTraitor));
        var randomTraitorRoles = commonTraitorRoles;
        randomTraitorRoles.AddRange(traitorPowerRoles);

        traitorRoles.AddRange(MiscUtils.ReadFromBucket(buckets, commonTraitorRoles, RoleListOption.CommonTraitor, RoleListOption.RandomTraitor));
        traitorRoles.AddRange(MiscUtils.ReadFromBucket(buckets, randomTraitorRoles, RoleListOption.RandomTraitor, RoleListOption.RandomTraitor));


        // coven buckets
        var covenDeceptionRoles = MiscUtils.GetRolesToAssign(Alignment.CovenDeception, roleFilter);
        var covenKillingRoles = MiscUtils.GetRolesToAssign(Alignment.CovenKilling, roleFilter);
        var covenOutlierRoles = MiscUtils.GetRolesToAssign(Alignment.CovenOutlier, roleFilter);
        var covenPowerRoles = MiscUtils.GetRolesToAssign(Alignment.CovenPower, roleFilter);
        var covenUtilityRoles = MiscUtils.GetRolesToAssign(Alignment.CovenUtility, roleFilter);

        covenRoles.AddRange(MiscUtils.ReadFromBucket(buckets, covenDeceptionRoles, RoleListOption.CovenDeception, RoleListOption.CommonCoven));
        covenRoles.AddRange(MiscUtils.ReadFromBucket(buckets, covenKillingRoles, RoleListOption.CovenKilling, RoleListOption.CommonCoven));
        covenRoles.AddRange(MiscUtils.ReadFromBucket(buckets, covenUtilityRoles, RoleListOption.CovenUtility, RoleListOption.CommonCoven));
        var commonCovenRoles = covenDeceptionRoles;
        commonCovenRoles.AddRange(covenKillingRoles);
        commonCovenRoles.AddRange(covenUtilityRoles);

        covenRoles.AddRange(MiscUtils.ReadFromBucket(buckets, covenPowerRoles, RoleListOption.CovenPower, RoleListOption.RandomCoven));
        var randomCovenRoles = commonCovenRoles;
        randomCovenRoles.AddRange(covenPowerRoles);
        if (OptionGroupSingleton<AUSOptions>.Instance.EnableAllOutliers)
        {
            covenRoles.AddRange(MiscUtils.ReadFromBucket(buckets, randomCovenRoles, RoleListOption.CovenOutlier, RoleListOption.RandomCoven));
            randomCovenRoles.AddRange(covenOutlierRoles);
        }

        covenRoles.AddRange(MiscUtils.ReadFromBucket(buckets, covenOutlierRoles, RoleListOption.CovenOutlier, RoleListOption.CovenOutlier));
        covenRoles.AddRange(MiscUtils.ReadFromBucket(buckets, commonCovenRoles, RoleListOption.CommonCoven, RoleListOption.RandomCoven));
        covenRoles.AddRange(MiscUtils.ReadFromBucket(buckets, randomCovenRoles, RoleListOption.RandomCoven, RoleListOption.RandomCoven));

        // neutral buckets
        var neutralApocalypse = MiscUtils.GetRolesToAssign(Alignment.NeutralApocalypse, x => !excluded.Contains(x.Role));
        var neutralBenign = MiscUtils.GetRolesToAssign(Alignment.NeutralBenign, roleFilter);
        var neutralChaos = MiscUtils.GetRolesToAssign(Alignment.NeutralChaos, x => !excluded.Contains(x.Role));
        var neutralEvil = MiscUtils.GetRolesToAssign(Alignment.NeutralEvil, roleFilter);
        var neutralKilling = MiscUtils.GetRolesToAssign(Alignment.NeutralKilling, roleFilter);
        var neutralOutlier = MiscUtils.GetRolesToAssign(Alignment.NeutralOutlier, roleFilter);
        var neutralPariah = MiscUtils.GetRolesToAssign(Alignment.NeutralPariah, roleFilter);

        neutralRoles.AddRange(MiscUtils.ReadFromBucket(buckets, neutralApocalypse, RoleListOption.NeutralApocalypse, RoleListOption.RandomNeutral));
        neutralRoles.AddRange(MiscUtils.ReadFromBucket(buckets, neutralBenign, RoleListOption.NeutralBenign, RoleListOption.RandomNeutral));
        neutralRoles.AddRange(MiscUtils.ReadFromBucket(buckets, neutralChaos, RoleListOption.NeutralChaos, RoleListOption.RandomNeutral));
        neutralRoles.AddRange(MiscUtils.ReadFromBucket(buckets, neutralEvil, RoleListOption.NeutralEvil, RoleListOption.RandomNeutral));
        neutralRoles.AddRange(MiscUtils.ReadFromBucket(buckets, neutralKilling, RoleListOption.NeutralKilling, RoleListOption.RandomNeutral));
        neutralRoles.AddRange(MiscUtils.ReadFromBucket(buckets, neutralPariah, RoleListOption.NeutralPariah, RoleListOption.RandomNeutral));
        var randomNeutralRoles = neutralApocalypse;
        randomNeutralRoles.AddRange(neutralBenign);
        randomNeutralRoles.AddRange(neutralChaos);
        randomNeutralRoles.AddRange(neutralEvil);
        randomNeutralRoles.AddRange(neutralKilling);
        randomNeutralRoles.AddRange(neutralPariah);
        if (OptionGroupSingleton<AUSOptions>.Instance.EnableAllOutliers)
        {
            neutralRoles.AddRange(MiscUtils.ReadFromBucket(buckets, neutralOutlier, RoleListOption.NeutralOutlier, RoleListOption.RandomNeutral));
            randomNeutralRoles.AddRange(neutralOutlier);
        }

        neutralRoles.AddRange(MiscUtils.ReadFromBucket(buckets, neutralOutlier, RoleListOption.NeutralOutlier, RoleListOption.NeutralOutlier));
        neutralRoles.AddRange(MiscUtils.ReadFromBucket(buckets, randomNeutralRoles, RoleListOption.RandomNeutral, RoleListOption.RandomNeutral));

        // town buckets
        var townInvestigativeRoles = MiscUtils.GetRolesToAssign(Alignment.TownInvestigative, roleFilter);
        var townKillingRoles = MiscUtils.GetRolesToAssign(Alignment.TownKilling, roleFilter);
        var townOutlierRoles = MiscUtils.GetRolesToAssign(Alignment.TownOutlier, roleFilter);
        var townPowerRoles = MiscUtils.GetRolesToAssign(Alignment.TownPower, roleFilter);
        var townProtectiveRoles = MiscUtils.GetRolesToAssign(Alignment.TownProtective, roleFilter);
        var townSupportRoles = MiscUtils.GetRolesToAssign(Alignment.TownSupport, roleFilter);
        var townUtilityRoles = MiscUtils.GetRolesToAssign(Alignment.TownUtility, roleFilter);

        townRoles.AddRange(MiscUtils.ReadFromBucket(buckets, townInvestigativeRoles, RoleListOption.TownInvestigative, RoleListOption.CommonTown));
        townRoles.AddRange(MiscUtils.ReadFromBucket(buckets, townKillingRoles, RoleListOption.TownKilling, RoleListOption.CommonTown));
        townRoles.AddRange(MiscUtils.ReadFromBucket(buckets, townProtectiveRoles, RoleListOption.TownProtective, RoleListOption.CommonTown));
        townRoles.AddRange(MiscUtils.ReadFromBucket(buckets, townSupportRoles, RoleListOption.TownSupport, RoleListOption.CommonTown));
        townRoles.AddRange(MiscUtils.ReadFromBucket(buckets, townUtilityRoles, RoleListOption.TownUtility, RoleListOption.CommonTown));
        var commonTownRoles = townInvestigativeRoles;
        commonTownRoles.AddRange(townKillingRoles);
        commonTownRoles.AddRange(townProtectiveRoles);
        commonTownRoles.AddRange(townSupportRoles);
        commonTownRoles.AddRange(townUtilityRoles);
        
        townRoles.AddRange(MiscUtils.ReadFromBucket(buckets, townPowerRoles, RoleListOption.TownPower, RoleListOption.RandomTown));
        var randomTownRoles = commonTownRoles;
        randomTownRoles.AddRange(townPowerRoles);
        if (OptionGroupSingleton<AUSOptions>.Instance.EnableAllOutliers)
        {
            townRoles.AddRange(MiscUtils.ReadFromBucket(buckets, townOutlierRoles, RoleListOption.TownOutlier, RoleListOption.RandomTown));
            randomTownRoles.AddRange(townOutlierRoles);
        }

        townRoles.AddRange(MiscUtils.ReadFromBucket(buckets, townOutlierRoles, RoleListOption.TownOutlier, RoleListOption.TownOutlier));
        townRoles.AddRange(MiscUtils.ReadFromBucket(buckets, commonTownRoles, RoleListOption.CommonTown, RoleListOption.RandomTown));
        townRoles.AddRange(MiscUtils.ReadFromBucket(buckets, randomTownRoles, RoleListOption.RandomTown, RoleListOption.RandomTown));

        // imp buckets
        var mafiaDeception = MiscUtils.GetRolesToAssign(Alignment.MafiaDeception, roleFilter);
        var mafiaKilling = MiscUtils.GetRolesToAssign(Alignment.MafiaKilling, roleFilter);
        var mafiaSupport = MiscUtils.GetRolesToAssign(Alignment.MafiaSupport, roleFilter);

        mafiaRoles.AddRange(MiscUtils.ReadFromBucket(buckets, mafiaDeception, RoleListOption.MafiaDeception, RoleListOption.CommonMafia));
        mafiaRoles.AddRange(MiscUtils.ReadFromBucket(buckets, mafiaSupport, RoleListOption.MafiaSupport, RoleListOption.CommonMafia));
        var commonImpRoles = mafiaDeception;
        commonImpRoles.AddRange(mafiaSupport);

        var randomImpRoles = commonImpRoles;
        mafiaRoles.AddRange(MiscUtils.ReadFromBucket(buckets, mafiaKilling, RoleListOption.MafiaKilling, RoleListOption.CommonMafia));

        mafiaRoles.AddRange(MiscUtils.ReadFromBucket(buckets, commonImpRoles, RoleListOption.CommonMafia, RoleListOption.RandomMafia));
        mafiaRoles.AddRange(MiscUtils.ReadFromBucket(buckets, randomImpRoles, RoleListOption.RandomMafia, RoleListOption.RandomMafia));

        var randomNotMafiaRoles = randomTownRoles;
        randomNotMafiaRoles.AddRange(randomNeutralRoles);
        randomNotMafiaRoles.AddRange(randomCovenRoles);
        randomNotMafiaRoles.AddRange(randomTraitorRoles);
        townRoles.AddRange(MiscUtils.ReadFromBucket(buckets, randomNotMafiaRoles, RoleListOption.NotMafia));

        // Shuffle roles before handing them out.
        // This should ensure a statistically equal chance of all permutations of roles.
        townRoles.Shuffle();
        neutralRoles.Shuffle();
        mafiaRoles.Shuffle();
        covenRoles.Shuffle();
        traitorRoles.Shuffle();

        var chosenImpRoles = mafiaRoles.Take(impCount).ToList();
        chosenImpRoles = chosenImpRoles.Pad(impCount, (ushort)(RoleTypes)RoleId.Get<Mafioso>());

        var uniqueRole = MiscUtils.AllRoles.FirstOrDefault(x => x is ISpawnChange { NoSpawn: false });
        if (uniqueRole != null && chosenImpRoles.Contains(RoleId.Get(uniqueRole.GetType())))
        {
            impCount = 1;
            
            if (AUSPlugin.IsDevBuild) Logger<AUSPlugin>.Warning($"Removing Impostor Roles because of {uniqueRole.NiceName}");

            while (impostors.Count > impCount)
            {
                crewmates.Add(impostors.TakeFirst());
            }

            chosenImpRoles.RemoveAll(x => x != RoleId.Get(uniqueRole.GetType()));
        }

        foreach (var role in neutralRoles)
        {
            var num = HashRandom.FastNext(crewmates.Count);
            var player = crewmates[num];

            player.RpcSetRole((RoleTypes)role);

            crewmates.RemoveAt(num);

            if (AUSPlugin.IsDevBuild) Logger<AUSPlugin>.Warning($"Assigning {RoleManager.Instance.GetRole((RoleTypes)role).NiceName} to {player.Data.PlayerName}.");
        }

        foreach (var role in townRoles)
        {
            var num = HashRandom.FastNext(crewmates.Count);
            var player = crewmates[num];

            player.RpcSetRole((RoleTypes)role);

            crewmates.RemoveAt(num);
            
            if (AUSPlugin.IsDevBuild) Logger<AUSPlugin>.Warning($"Assigning {RoleManager.Instance.GetRole((RoleTypes)role).NiceName} to {player.Data.PlayerName}.");
        }

        int coven = 0;
        foreach (var role in covenRoles)
        {
            var num = HashRandom.FastNext(crewmates.Count);
            var player = crewmates[num];

            if (coven == 4)
            {
                ushort newRole = townRoles.LastOrDefault();
                player.RpcSetRole((RoleTypes)newRole);

                if (AUSPlugin.IsDevBuild) Logger<AUSPlugin>.Warning($"Assigning {RoleManager.Instance.GetRole((RoleTypes)newRole).NiceName} to {player.Data.PlayerName}.");
            }
            else
            {
                player.RpcSetRole((RoleTypes)role);
                coven++;

                if (AUSPlugin.IsDevBuild) Logger<AUSPlugin>.Warning($"Assigning {RoleManager.Instance.GetRole((RoleTypes)role).NiceName} to {player.Data.PlayerName}.");
            }

            crewmates.RemoveAt(num);
        }

        foreach (var role in traitorRoles)
        {
            var num = HashRandom.FastNext(crewmates.Count);
            var player = crewmates[num];

            player.RpcSetRole((RoleTypes)role);

            crewmates.RemoveAt(num);

            if (AUSPlugin.IsDevBuild) Logger<AUSPlugin>.Warning($"Assigning {RoleManager.Instance.GetRole((RoleTypes)role).NiceName} to {player.Data.PlayerName}.");
        }

        foreach (var role in chosenImpRoles)
        {
            var num = HashRandom.FastNext(impostors.Count);
            var player = impostors[num];

            player.RpcSetRole((RoleTypes)role);

            impostors.RemoveAt(num);

            if (AUSPlugin.IsDevBuild) Logger<AUSPlugin>.Warning($"Assigning {RoleManager.Instance.GetRole((RoleTypes)role).NiceName} to {player.Data.PlayerName}.");
        }

        // Assign vanilla roles to anyone who did not receive a role.
        foreach (var player in crewmates)
        {
            player.RpcSetRole((RoleTypes)RoleId.Get<Pilgrim>());
        }

        foreach (var player in impostors)
        {
            player.RpcSetRole((RoleTypes)RoleId.Get<Mafioso>());
        }
    }

    public static void AssignTargets()
    {
        foreach (var role in MiscUtils.AllRoles.Where(x => x is IAssignableTargets)
                     .OrderBy(x => (x as IAssignableTargets)!.Priority))
        {
            if (role is IAssignableTargets assignRole)
            {
                assignRole.AssignTargets();
            }
        }

        foreach (var modifier in MiscUtils.AllModifiers.Where(x => x is IAssignableTargets)
                     .OrderBy(x => (x as IAssignableTargets)!.Priority))
        {
            if (modifier is IAssignableTargets assignMod)
            {
                assignMod.AssignTargets();
            }
        }

        GhostRoleSetup();
    }

    [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SelectRoles))]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)]
    public static bool SelectRolesPatch(RoleManager __instance)
    {
        if (AUSPlugin.IsDevBuild) Logger<AUSPlugin>.Error($"RoleManager.SelectRoles - ReplaceRoleManager: {ReplaceRoleManager}");

        if (TutorialManager.InstanceExists || ReplaceRoleManager)
        {
            return true;
        }

        //Logger<AUSPlugin>.Error($"RoleManager.SelectRoles 2");

        var players = GameData.Instance.AllPlayers.ToArray().ToList();
        players.Shuffle();

        var impCount = GameOptionsManager.Instance.CurrentGameOptions.GetAdjustedNumImpostors(players.Count);
        List<NetworkedPlayerInfo> infected = [];

        infected.AddRange(players.Take(impCount));

        LastImps = [.. infected.Select(x => x.ClientId)];

        AssignRolesFromRoleList(infected);
        AssignTargets();

        return false;
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetRole))]
    [HarmonyPrefix]
    public static bool RpcSetRolePatch(PlayerControl __instance, [HarmonyArgument(0)] RoleTypes roleType,
        [HarmonyArgument(1)] bool canOverrideRole = false)
    {
        if (AmongUsClient.Instance.AmClient)
        {
            __instance.StartCoroutine(__instance.CoSetRole(roleType, canOverrideRole));
        }

        var messageWriter =
            AmongUsClient.Instance.StartRpcImmediately(__instance.NetId, (byte)RpcCalls.SetRole, SendOption.Reliable);
        messageWriter.Write((ushort)roleType);
        messageWriter.Write(canOverrideRole);
        AmongUsClient.Instance.FinishRpcImmediately(messageWriter);

        var changeRoleEvent = new ChangeRoleEvent(__instance, null, RoleManager.Instance.GetRole(roleType));
        MiraEventManager.InvokeEvent(changeRoleEvent);

        return false;
    }

    [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.AssignRoleOnDeath))]
    [HarmonyPrefix]
    public static bool AssignRoleOnDeathPatch(RoleManager __instance, PlayerControl player, bool specialRolesAllowed)
    {
        // Note: I know this is a like for like recreation of the AssignRoleOnDeath function but for some reason
        // the original won't spawn the Phantom and just spawns Neutral Ghost instead

        if (player == null || !player.Data.IsDead) // Logger<TownOfUsPlugin>.Message($"AssignRoleOnDeathPatch - !player.Data.IsDead: '{!player.Data.IsDead}'");
        {
            return false;
        }

        /*if (specialRolesAllowed && !player.HasModifier<BasicGhostModifier>())
            // Logger<TownOfUsPlugin>.Message($"AssignRoleOnDeathPatch - !player.Data.Role.IsImpostor: '{!player.Data.Role.IsImpostor}' specialRolesAllowed: {specialRolesAllowed}");
        {
            RoleManager.TryAssignSpecialGhostRoles(player, player.IsImpostor());
        }*/

        if (!RoleManager.IsGhostRole(player.Data.Role.Role))
            // Logger<TownOfUsPlugin>.Message($"AssignRoleOnDeathPatch - !RoleManager.IsGhostRole(player.Data.Role.Role): '{!RoleManager.IsGhostRole(player.Data.Role.Role)}'");
        {
            player.RpcSetRole(player.Data.Role.DefaultGhostRole);
        }

        return false;
    }

    [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.TryAssignSpecialGhostRoles))]
    [HarmonyPrefix]
    public static bool TryAssignSpecialGhostRolesPatch(RoleManager __instance, PlayerControl player)
    {
        if (AUSPlugin.IsDevBuild) Logger<AUSPlugin>.Warning($"TryAssignSpecialGhostRolesPatch - Player: '{player.Data.PlayerName}'");
        var ghostRole = RoleTypes.CrewmateGhost;

        if (player.IsCrewmate() && CrewmateGhostRolePool.Count > 0)
        {
            ghostRole = CrewmateGhostRolePool.TakeFirst();
        }
        else if (player.IsImpostor() && ImpostorGhostRolePool.Count > 0)
        {
            ghostRole = ImpostorGhostRolePool.TakeFirst();
        }
        else if (player.IsNeutral() && CustomGhostRolePool.Count > 0)
        {
            ghostRole = CustomGhostRolePool.TakeFirst();
        }

        if (ghostRole != RoleTypes.CrewmateGhost && ghostRole != RoleTypes.ImpostorGhost &&
            ghostRole != (RoleTypes)RoleId.Get<NeutralGhostRole>())
            // var newRole = RoleManager.Instance.GetRole(ghostRole);
            // Logger<AUSPlugin>.Message($"TryAssignSpecialGhostRolesPatch - ghostRoles role: {newRole.NiceName}");
        {
            player.RpcChangeRole((ushort)ghostRole);
        }

        return false;
    }

    //[HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SetRole))]
    //[HarmonyPostfix]
    //public static void SetRolePatch(RoleManager __instance, [HarmonyArgument(0)] PlayerControl targetPlayer, [HarmonyArgument(1)] RoleTypes roleType)
    //{
    //    GameHistory.RegisterRole(targetPlayer, targetPlayer.Data.Role);
    //}
    [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.GetAdjustedNumImpostors))]
    [HarmonyPrefix]
    public static bool GetAdjustedImposters(IGameOptions __instance, ref int __result)
    {
        if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return true;

        var players = GameData.Instance.PlayerCount;
        var impostors = 0;
        var list = OptionGroupSingleton<RoleOptions>.Instance;
        var maxSlots = players < 15 ? players : 15;
        List<RoleListOption> impBuckets =
        [
            RoleListOption.MafiaDeception, RoleListOption.MafiaKilling, RoleListOption.MafiaSupport,
            RoleListOption.CommonMafia, RoleListOption.RandomMafia
        ];
        List<RoleListOption> buckets = [];
        var anySlots = 0;

        for (var i = 0; i < maxSlots; i++)
        {
            var slotValue = i switch
            {
                0 => list.Slot1,
                1 => list.Slot2,
                2 => list.Slot3,
                3 => list.Slot4,
                4 => list.Slot5,
                5 => list.Slot6,
                6 => list.Slot7,
                7 => list.Slot8,
                8 => list.Slot9,
                9 => list.Slot10,
                10 => list.Slot11,
                11 => list.Slot12,
                12 => list.Slot13,
                13 => list.Slot14,
                14 => list.Slot15,
                _ => -1
            };
            buckets.Add((RoleListOption)slotValue);
        }


        foreach (var roleOption in buckets)
        {
            if (impBuckets.Contains(roleOption)) impostors += 1;
            else if (roleOption == RoleListOption.Any) anySlots += 1;
        }

        int impProbability = (int)Math.Floor((double)players / anySlots * 5 / 3);
        for (int i = 0; i < anySlots; i++)
        {
            var random = Random.RandomRangeInt(0, 100);
            if (random < impProbability) impostors += 1;
            impProbability += 3;
        }

        /*if (players < 7 || impostors == 0) impostors = 1;
        else if (players < 10 && impostors > 2) impostors = 2;
        else if (players < 14 && impostors > 3) impostors = 3;
        else if (players < 19 && impostors > 4) impostors = 4;
        else if (impostors > 5) impostors = 5;*/
        if (impostors > 4) impostors = 4;
        __result = impostors;
        return false;
    }
}