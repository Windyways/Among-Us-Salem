using AmongUs.GameOptions;
using Hazel;
using Reactor.Utilities.Extensions;
using TownOfUs.Events.TouEvents;
using TownOfUs.Options;
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

    public static int MafiaCount;
    public static void PerformAllAnyRoleGeneration(List<NetworkedPlayerInfo> infected)
    {
        var impostors = MiscUtils.GetImpostors(infected);
        var crewmates = MiscUtils.GetCrewmates(impostors);

        var rolesAssigned = new List<ushort>();
        var allRoles = MiscUtils.AllRoles.Where(x => x is ICustomAURole && x is not ISpawnChange).ToList();

        // Here we try to add duplicate roles if there are more than 1 of it.
        foreach (var role in allRoles.ToList())
        {
            if (role is ICustomAURole customRole)
            {
                var count = customRole.GetCount();
                if (count != null)
                {
                    for (int i = 1; i < count; i++)
                    {
                        allRoles.Add(role);
                    }
                }
            }
        }

        var rolesAssignable = allRoles.ToList();

        // Remove Outlier roles from the pool if they're disabled.
        if (!OptionGroupSingleton<RoleGenOptions>.Instance.EnableAllOutliers)
        {
            allRoles = allRoles.Where(x => x is ICustomAURole customRole && (customRole.Alignment == Alignment.TownOutlier || customRole.Alignment == Alignment.NeutralOutlier || customRole.Alignment == Alignment.CovenOutlier)).ToList();
        }

        var players = GameData.Instance.PlayerCount;

        // --- COVEN ---
        int maxCoven = (int)OptionGroupSingleton<CovenOptions>.Instance.MaxCoven;
        int covenCount = 0;

        // --- APOCALYPSE ---
        bool fourHorsemen = OptionGroupSingleton<ApocOptions>.Instance.EnableFourHorsemen;
        int apocCount = 0;

        while (rolesAssigned.Count < players)
        {
            int mafiaCount = rolesAssigned.Count(x => RoleManager.Instance.GetRole((RoleTypes)x).IsImpostor());
            if (rolesAssignable.Count == 0)
            {
                rolesAssignable = allRoles.Where(x => !rolesAssigned.Contains(RoleId.Get(x.GetType()))).ToList();
                AUSPlugin.DebugLogMessage($"No more roles to assign, refreshing!", AUSPlugin.MsgType.Warning);
            }

            var role = rolesAssignable.Random();
            var customRole = role as ICustomAURole;
            if (customRole != null)
            {
                var chance = customRole.GetChance();
                var count = customRole.GetCount();
                if (chance != null && count != null)
                {
                    // Make sure to assign Mafia roles first because of this bs wegthyj3wregthn.
                    if (rolesAssigned.Count(x => RoleManager.Instance.GetRole((RoleTypes)x).IsImpostor()) < MafiaCount && customRole.Faction != Faction.Mafia)
                    {
                        AUSPlugin.DebugLogMessage($"Failed to assign {customRole.RoleName} because Mafia roles need to be rolled first!", AUSPlugin.MsgType.Error);
                    }
                    else if (CalculatedVoting.ChanceIsNull(chance))
                    {
                        if (customRole.Faction == Faction.Coven) covenCount++;
                        if (customRole.Alignment == Alignment.NeutralApocalypse) apocCount++;

                        if (apocCount > 1 && customRole.Alignment == Alignment.NeutralApocalypse && !fourHorsemen) AUSPlugin.DebugLogMessage($"Failed to assign {customRole.RoleName} because max Apocalypse members reached!");
                        else if (covenCount > maxCoven && customRole.Faction == Faction.Coven) AUSPlugin.DebugLogMessage($"Failed to assign {customRole.RoleName} because max Coven members reached!");
                        else if (mafiaCount >= MafiaCount && customRole.Faction == Faction.Mafia) AUSPlugin.DebugLogMessage($"Failed to assign {customRole.RoleName} because max Mafia members reached!");
                        else
                        {
                            rolesAssigned.Add(RoleId.Get(customRole.GetType()));
                            AUSPlugin.DebugLogMessage($"Assigned {customRole.RoleName}!");
                        }
                    }
                    else AUSPlugin.DebugLogMessage($"Failed to assign {customRole.RoleName} because it failed to roll chance!");
                }
            }

            rolesAssignable.Remove(role);
        }

        foreach (var role in rolesAssigned.Where(x => !RoleManager.Instance.GetRole((RoleTypes)x).IsImpostor()))
        {
            var num = HashRandom.FastNext(crewmates.Count);
            var player = crewmates[num];

            player.RpcSetRole((RoleTypes)role);

            crewmates.RemoveAt(num);

            if (AUSPlugin.IsDevBuild) Logger<AUSPlugin>.Warning($"Assigning {RoleManager.Instance.GetRole((RoleTypes)role).NiceName} to {player.Data.PlayerName}.");
        }

        foreach (var role in rolesAssigned.Where(x => RoleManager.Instance.GetRole((RoleTypes)x).IsImpostor()))
        {
            if (impostors.Count == 0) break;
            var num = HashRandom.FastNext(impostors.Count);
            var player = impostors[num];

            player.RpcSetRole((RoleTypes)role);

            impostors.RemoveAt(num);

            if (AUSPlugin.IsDevBuild) Logger<AUSPlugin>.Warning($"Assigning {RoleManager.Instance.GetRole((RoleTypes)role).NiceName} to {player.Data.PlayerName}.");
        }

        // Assign vanilla roles to anyone who did not receive a role.
        foreach (var player in crewmates) player.RpcSetRole((RoleTypes)RoleId.Get<Pilgrim>());
        foreach (var player in impostors) player.RpcSetRole((RoleTypes)RoleId.Get<Mafioso>());
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

        if (AllAnyMode()) PerformAllAnyRoleGeneration(infected);
        else RolelistMechanic.GenerateRoleListAndApplyRoles(infected); //AssignRolesFromRoleList(infected);

        AssignTargets();

        return false;
    }

    public static bool AllAnyMode()
    {
        var playerCount = GameData.Instance.AllPlayers.Count;
        var opts = OptionGroupSingleton<RoleOptions>.Instance;
        List<RoleListOption> buckets =
        [
            (RoleListOption)opts.Slot1.Value, (RoleListOption)opts.Slot2.Value, (RoleListOption)opts.Slot3.Value,
            (RoleListOption)opts.Slot4.Value
        ];

        var anySlots = 0;

        if (playerCount > 4) buckets.Add((RoleListOption)opts.Slot5.Value);
        if (playerCount > 5) buckets.Add((RoleListOption)opts.Slot6.Value);
        if (playerCount > 6) buckets.Add((RoleListOption)opts.Slot7.Value);
        if (playerCount > 7) buckets.Add((RoleListOption)opts.Slot8.Value);
        if (playerCount > 8) buckets.Add((RoleListOption)opts.Slot9.Value);
        if (playerCount > 9) buckets.Add((RoleListOption)opts.Slot10.Value);
        if (playerCount > 10) buckets.Add((RoleListOption)opts.Slot11.Value);
        if (playerCount > 11) buckets.Add((RoleListOption)opts.Slot12.Value);
        if (playerCount > 12) buckets.Add((RoleListOption)opts.Slot13.Value);
        if (playerCount > 13) buckets.Add((RoleListOption)opts.Slot14.Value);
        if (playerCount > 14) buckets.Add((RoleListOption)opts.Slot15.Value);
        if (playerCount > 15)
        {
            for (var i = 0; i < playerCount - 15; i++) buckets.Add(RoleListOption.Any);
        }

        foreach (var roleOption in buckets)
            if (roleOption == RoleListOption.Any) anySlots += 1;

        return anySlots == playerCount;
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

        if (player == null || !player.Data.IsDead) // Logger<AUSPlugin>.Message($"AssignRoleOnDeathPatch - !player.Data.IsDead: '{!player.Data.IsDead}'");
        {
            return false;
        }

        /*if (specialRolesAllowed && !player.HasModifier<BasicGhostModifier>())
            // Logger<AUSPlugin>.Message($"AssignRoleOnDeathPatch - !player.Data.Role.IsImpostor: '{!player.Data.Role.IsImpostor}' specialRolesAllowed: {specialRolesAllowed}");
        {
            RoleManager.TryAssignSpecialGhostRoles(player, player.IsImpostor());
        }*/

        if (!RoleManager.IsGhostRole(player.Data.Role.Role))
        // Logger<AUSPlugin>.Message($"AssignRoleOnDeathPatch - !RoleManager.IsGhostRole(player.Data.Role.Role): '{!RoleManager.IsGhostRole(player.Data.Role.Role)}'");
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

        if (AllAnyMode()) // in AA, assign Mafia Count based on the rate of all roles and mafia roles.
        {
            impostors = 0;
            var effectiveRoles = new List<ICustomAURole>();

            foreach (var role in MiscUtils.AllRoles)
            {
                if (role is ICustomAURole customRole && role is not ISpawnChange)
                {
                    int count = customRole.GetCount() ?? 1;

                    for (int i = 0; i < count; i++)
                        effectiveRoles.Add(customRole);
                }
            }

            float totalWeight = effectiveRoles.Sum(r => r.GetChance() ?? 0f);

            float mafiaWeight = effectiveRoles
                .Where(r => r.Faction == Faction.Mafia)
                .Sum(r => r.GetChance() ?? 0f);

            float mafiaProbability = mafiaWeight / totalWeight;
            float mafiaPercent = mafiaProbability * 100f;

            var mafiaBucketCount = RolelistMechanic.GetBuckets().Count(x => impBuckets.Contains(x));
            impostors += mafiaBucketCount;

            var anyBucketCount = RolelistMechanic.GetBuckets().Count(x => x == RoleListOption.Any);
            for (int i = 0; i < anyBucketCount; i++)
            {
                if (impostors >= 4) break;
                if (UnityEngine.Random.value < mafiaProbability)
                    impostors++;
            }
        }

        MafiaCount = impostors;
        __result = impostors;
        return false;
    }
}