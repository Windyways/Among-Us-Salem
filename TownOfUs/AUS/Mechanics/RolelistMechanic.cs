using AmongUs.GameOptions;
using TownOfUs.Options;

namespace AmongUsSalem.Mechanics;

public static class RolelistMechanic
{
    public static int CovenCount;
    public static void GenerateRoleListAndApplyRoles(List<NetworkedPlayerInfo> infected)
    {
        var impostors = MiscUtils.GetImpostors(infected);
        var crewmates = MiscUtils.GetCrewmates(impostors);

        var rolesAssigned = new List<ushort>();

        var buckets = GetBuckets();
        int mafiaCount = buckets.Count(x => x is RoleListOption.RandomMafia or RoleListOption.CommonMafia or RoleListOption.MafiaDeception or RoleListOption.MafiaKilling or RoleListOption.MafiaSupport);
        int guaranteedCovenCount = buckets.Count(x => x is RoleListOption.RandomCoven or RoleListOption.CommonCoven or RoleListOption.CovenDeception or RoleListOption.CovenKilling or RoleListOption.CovenOutlier or RoleListOption.CovenPower or RoleListOption.CovenUtility);

        CovenCount += guaranteedCovenCount;
        foreach (var bucket in buckets.OrderBy(x => x is RoleListOption.Any))
        {
            if (bucket is RoleListOption.TownExecutive) AssignTownRole(rolesAssigned, Alignment.TownExecutive);
            if (bucket is RoleListOption.TownGovernment) AssignTownRole(rolesAssigned, Alignment.TownGovernment);
            if (bucket is RoleListOption.TownInvestigative) AssignTownRole(rolesAssigned, Alignment.TownInvestigative);
            if (bucket is RoleListOption.TownKilling) AssignTownRole(rolesAssigned, Alignment.TownKilling);
            if (bucket is RoleListOption.TownOutlier) AssignTownRole(rolesAssigned, Alignment.TownOutlier);
            if (bucket is RoleListOption.TownProtective) AssignTownRole(rolesAssigned, Alignment.TownProtective);
            if (bucket is RoleListOption.TownSupport) AssignTownRole(rolesAssigned, Alignment.TownSupport);
            if (bucket is RoleListOption.RandomTown or RoleListOption.CommonTown) AssignTownRole(rolesAssigned, Alignment.None, bucket);
            if (bucket is RoleListOption.TE_TG) AssignTownRole(rolesAssigned, Alignment.TownExecutive, bucket);
            if (bucket is RoleListOption.TE_TG_TO) AssignTownRole(rolesAssigned, Alignment.TownOutlier, bucket);

            if (bucket is RoleListOption.NeutralApocalypse) AssignNeutralRole(rolesAssigned, Alignment.NeutralApocalypse);
            if (bucket is RoleListOption.NeutralBenign) AssignNeutralRole(rolesAssigned, Alignment.NeutralBenign);
            if (bucket is RoleListOption.NeutralChaos) AssignNeutralRole(rolesAssigned, Alignment.NeutralChaos);
            if (bucket is RoleListOption.NeutralEvil) AssignNeutralRole(rolesAssigned, Alignment.NeutralEvil);
            if (bucket is RoleListOption.NeutralKilling) AssignNeutralRole(rolesAssigned, Alignment.NeutralKilling);
            if (bucket is RoleListOption.NeutralOutlier) AssignNeutralRole(rolesAssigned, Alignment.NeutralOutlier);
            if (bucket is RoleListOption.NeutralPariah) AssignNeutralRole(rolesAssigned, Alignment.NeutralPariah);
            if (bucket is RoleListOption.RandomNeutral) AssignNeutralRole(rolesAssigned, Alignment.None, bucket);
            if (bucket is RoleListOption.NA_RN) AssignNeutralRole(rolesAssigned, Alignment.None, bucket);

            if (bucket is RoleListOption.MafiaDeception) AssignMafiaRole(rolesAssigned, Alignment.MafiaDeception);
            if (bucket is RoleListOption.MafiaKilling) AssignMafiaRole(rolesAssigned, Alignment.MafiaKilling);
            if (bucket is RoleListOption.MafiaSupport) AssignMafiaRole(rolesAssigned, Alignment.MafiaSupport);
            if (bucket is RoleListOption.RandomMafia or RoleListOption.CommonMafia) AssignMafiaRole(rolesAssigned, Alignment.None, bucket);

            if (bucket is RoleListOption.CovenDeception) AssignCovenRole(rolesAssigned, Alignment.CovenDeception);
            if (bucket is RoleListOption.CovenKilling) AssignCovenRole(rolesAssigned, Alignment.CovenKilling);
            if (bucket is RoleListOption.CovenOutlier) AssignCovenRole(rolesAssigned, Alignment.CovenOutlier);
            if (bucket is RoleListOption.CovenPower) AssignCovenRole(rolesAssigned, Alignment.CovenPower);
            if (bucket is RoleListOption.CovenUtility) AssignCovenRole(rolesAssigned, Alignment.CovenUtility);
            if (bucket is RoleListOption.RandomCoven or RoleListOption.CommonCoven) AssignCovenRole(rolesAssigned, Alignment.None, bucket);

            if (bucket is RoleListOption.NK_RC) AssignTownRole(rolesAssigned, Alignment.TownOutlier, bucket);
            if (bucket is RoleListOption.Any) AssignAnyRole(rolesAssigned, mafiaCount);
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

    public static void AssignAnyRole(List<ushort> rolesAssigned, int mafiaCount)
    {
        int maxCoven = (int)OptionGroupSingleton<CovenOptions>.Instance.MaxCoven;
        var allRoles = MiscUtils.AllRoles.Where(x => x is ICustomAURole && x is not ISpawnChange).ToList();

        if (CovenCount >= 4 && mafiaCount >= 4) allRoles = MiscUtils.AllRoles.Where(x => x is ICustomAURole customRole && x is not ISpawnChange &&
            customRole.Faction != Faction.Mafia && customRole.Faction != Faction.Coven && !rolesAssigned.Contains(RoleId.Get(customRole.GetType()))).ToList();
        else if (CovenCount >= 4) allRoles = MiscUtils.AllRoles.Where(x => x is ICustomAURole customRole && x is not ISpawnChange &&
            customRole.Faction != Faction.Coven && !rolesAssigned.Contains(RoleId.Get(customRole.GetType()))).ToList();
        else if (mafiaCount >= 4) allRoles = MiscUtils.AllRoles.Where(x => x is ICustomAURole customRole && x is not ISpawnChange &&
            customRole.Faction != Faction.Mafia && !rolesAssigned.Contains(RoleId.Get(customRole.GetType()))).ToList();

        allRoles.AddRange(allRoles.AddDuplicateRolesToPool());
        var rolesAssignable = allRoles.ToList();

        // Remove Outlier roles from the pool if they're disabled.
        var outliers = allRoles.RemoveOutliers();
        allRoles = allRoles.Except(outliers).ToList();

        bool gotRole = false;
        int attempts = 0;
        while (!gotRole && attempts < 50)
        {
            mafiaCount = rolesAssigned.Count(x => RoleManager.Instance.GetRole((RoleTypes)x).IsImpostor());
            if (rolesAssignable.Count == 0)
            {
                attempts++;
                rolesAssignable = allRoles.Where(x => !rolesAssigned.Contains(RoleId.Get(x.GetType()))).ToList();
                if (attempts < 5) AUSPlugin.DebugLogMessage($"No more roles to assign, refreshing!", AUSPlugin.MsgType.Warning);
            }

            var role = rolesAssignable.Random();
            var customRole = role as ICustomAURole;
            if (customRole != null && role != null)
            {
                var chance = customRole.GetChance();
                var count = customRole.GetCount();
                if (chance != null && count != null)
                {
                    // Make sure to assign Mafia roles first because of this bs wegthyj3wregthn.
                    if (rolesAssigned.Count(x => RoleManager.Instance.GetRole((RoleTypes)x).IsImpostor()) < TouRoleManagerPatches.MafiaCount && customRole.Faction != Faction.Mafia)
                    {
                        AUSPlugin.DebugLogMessage($"Failed to assign {customRole.RoleName} because Mafia roles need to be rolled first!", AUSPlugin.MsgType.Error);
                    }
                    else if (CalculatedVoting.ChanceIsNull(chance))
                    {
                        var roleUSHORT = RoleId.Get(customRole.GetType());
                        
                        if (rolesAssigned.Count(x => x.UshortToRole().NiceName == role.NiceName) >= count) AUSPlugin.DebugLogMessage($"Failed to assign {customRole.RoleName} because it has reached its max count!", AUSPlugin.MsgType.Warning);
                        else if (CovenCount > maxCoven && customRole.Faction == Faction.Coven) AUSPlugin.DebugLogMessage($"Failed to assign {customRole.RoleName} because max Coven members reached!");
                        else if (mafiaCount >= TouRoleManagerPatches.MafiaCount && customRole.Faction == Faction.Mafia) AUSPlugin.DebugLogMessage($"Failed to assign {customRole.RoleName} because max Mafia members reached!");
                        else
                        {
                            if (customRole.Faction == Faction.Coven) CovenCount++;

                            gotRole = true;
                            rolesAssigned.Add(roleUSHORT);
                            AUSPlugin.DebugLogMessage($"Assigned {customRole.RoleName}!");
                        }
                    }
                    else AUSPlugin.DebugLogMessage($"Failed to assign {customRole.RoleName} because it failed to roll chance!");
                }
            }

            rolesAssignable.Remove(role);
        }
    }

    public static void AssignNeutralRole(List<ushort> rolesAssigned, Alignment alignment, RoleListOption bucket = RoleListOption.None)
    {
        var faction = Faction.Neutral;
        if (alignment == Alignment.NeutralApocalypse) faction = Faction.Apocalypse;
        
        var allRoles = MiscUtils.AllRoles.Where(x => x is ICustomAURole customRole && x is not ISpawnChange &&
            customRole.Alignment == alignment).ToList();

        if (faction == Faction.Apocalypse)
        {
            allRoles = MiscUtils.AllRoles.Where(x => x is ICustomAURole customRole && x is not ISpawnChange &&
                customRole.Faction == faction).ToList();
        }

        if (bucket == RoleListOption.RandomNeutral) allRoles = MiscUtils.AllRoles.Where(x => x is ICustomAURole customRole && x is not ISpawnChange && customRole.IsNeutral()).ToList();
        if (bucket == RoleListOption.NA_RN) allRoles = MiscUtils.AllRoles.Where(x => x is ICustomAURole customRole && x is not ISpawnChange && customRole.IsNeutral(true)).ToList();

        allRoles.AddRange(allRoles.AddDuplicateRolesToPool());
        var rolesAssignable = allRoles.ToList();

        // Remove Outlier roles from the pool if they're disabled.
        var outliers = allRoles.RemoveOutliers(alignment, bucket);
        allRoles = allRoles.Except(outliers).ToList();

        bool gotRole = false;
        int attempts = 0;
        while (!gotRole && attempts < 50)
        {
            if (rolesAssignable.Count == 0)
            {
                attempts++;
                rolesAssignable = allRoles.Where(x => !rolesAssigned.Contains(RoleId.Get(x.GetType()))).ToList();
                if (attempts < 5) AUSPlugin.DebugLogMessage($"No more roles to assign, refreshing!", AUSPlugin.MsgType.Warning);
            }

            var role = rolesAssignable.Random();
            var customRole = role as ICustomAURole;
            if (customRole != null && role != null)
            {
                var chance = customRole.GetChance();
                var count = customRole.GetCount();
                if (chance != null && count != null)
                {
                    if (CalculatedVoting.ChanceIsNull(chance))
                    {
                        var roleUSHORT = RoleId.Get(customRole.GetType());
                        if (rolesAssigned.Count(x => x.UshortToRole().NiceName == role.NiceName) >= count) AUSPlugin.DebugLogMessage($"Failed to assign {customRole.RoleName} because it has reached its max count!", AUSPlugin.MsgType.Warning);
                        else
                        {
                            gotRole = true;
                            rolesAssigned.Add(roleUSHORT);
                            AUSPlugin.DebugLogMessage($"Assigned {customRole.RoleName}!");
                        }
                    }
                    else AUSPlugin.DebugLogMessage($"Failed to assign {customRole.RoleName} because it failed to roll chance!");
                }
            }

            rolesAssignable.Remove(role);
        }
    }

    public static void AssignMafiaRole(List<ushort> rolesAssigned, Alignment alignment, RoleListOption bucket = RoleListOption.None)
    {
        var allRoles = MiscUtils.AllRoles.Where(x => x is ICustomAURole customRole && x is not ISpawnChange &&
            customRole.Alignment == alignment).ToList();

        if (bucket == RoleListOption.RandomMafia) allRoles = MiscUtils.AllRoles.Where(x => x is ICustomAURole customRole && x is not ISpawnChange && customRole.Faction == Faction.Mafia).ToList();
        if (bucket == RoleListOption.CommonMafia) allRoles = MiscUtils.AllRoles.Where(x => x is ICustomAURole customRole && x is not ISpawnChange && customRole.Faction == Faction.Mafia && customRole.Alignment != Alignment.MafiaKilling).ToList();

        allRoles.AddRange(allRoles.AddDuplicateRolesToPool());
        var rolesAssignable = allRoles.ToList();

        // Remove Outlier roles from the pool if they're disabled.
        var outliers = allRoles.RemoveOutliers(alignment, bucket);
        allRoles = allRoles.Except(outliers).ToList();

        bool gotRole = false;
        int attempts = 0;
        while (!gotRole && attempts < 50)
        {
            if (rolesAssignable.Count == 0)
            {
                attempts++;
                rolesAssignable = allRoles.Where(x => !rolesAssigned.Contains(RoleId.Get(x.GetType()))).ToList();
                if (attempts < 5) AUSPlugin.DebugLogMessage($"No more roles to assign, refreshing!", AUSPlugin.MsgType.Warning);
            }

            var role = rolesAssignable.Random();
            var customRole = role as ICustomAURole;
            if (customRole != null && role != null)
            {
                var chance = customRole.GetChance();
                var count = customRole.GetCount();
                if (chance != null && count != null)
                {
                    if (CalculatedVoting.ChanceIsNull(chance))
                    {
                        var roleUSHORT = RoleId.Get(customRole.GetType());
                        if (rolesAssigned.Count(x => x.UshortToRole().NiceName == role.NiceName) >= count) AUSPlugin.DebugLogMessage($"Failed to assign {customRole.RoleName} because it has reached its max count!", AUSPlugin.MsgType.Warning);
                        else
                        {
                            gotRole = true;
                            rolesAssigned.Add(roleUSHORT);
                            AUSPlugin.DebugLogMessage($"Assigned {customRole.RoleName}!");
                        }
                    }
                    else AUSPlugin.DebugLogMessage($"Failed to assign {customRole.RoleName} because it failed to roll chance!");
                }
            }

            rolesAssignable.Remove(role);
        }
    }

    public static void AssignTownRole(List<ushort> rolesAssigned, Alignment alignment, RoleListOption bucket = RoleListOption.None)
    {
        var allRoles = MiscUtils.AllRoles.Where(x => x is ICustomAURole customRole && x is not ISpawnChange &&
            customRole.Alignment == alignment).ToList();

        if (bucket == RoleListOption.RandomTown) allRoles = MiscUtils.AllRoles.Where(x => x is ICustomAURole customRole && x is not ISpawnChange && customRole.Faction == Faction.Town).ToList();
        if (bucket == RoleListOption.CommonTown) allRoles = MiscUtils.AllRoles.Where(x => x is ICustomAURole customRole && x is not ISpawnChange && customRole.Faction == Faction.Town && !(customRole.Alignment == Alignment.TownExecutive || customRole.Alignment == Alignment.TownGovernment)).ToList();
        if (bucket == RoleListOption.TE_TG) allRoles = MiscUtils.AllRoles.Where(x => x is ICustomAURole customRole && x is not ISpawnChange && (customRole.Alignment == Alignment.TownGovernment || customRole.Alignment == Alignment.TownExecutive)).ToList();
        if (bucket == RoleListOption.TE_TG_TO) allRoles = MiscUtils.AllRoles.Where(x => x is ICustomAURole customRole && x is not ISpawnChange && (customRole.Alignment == Alignment.TownGovernment || customRole.Alignment == Alignment.TownExecutive || customRole.Alignment == Alignment.TownOutlier)).ToList();

        allRoles.AddRange(allRoles.AddDuplicateRolesToPool());
        var rolesAssignable = allRoles.ToList();

        // Remove Outlier roles from the pool if they're disabled.
        var outliers = allRoles.RemoveOutliers(alignment, bucket);
        allRoles = allRoles.Except(outliers).ToList();

        bool gotRole = false;
        int attempts = 0;
        while (!gotRole && attempts < 50)
        {
            if (rolesAssignable.Count == 0)
            {
                attempts++;
                rolesAssignable = allRoles.Where(x => !rolesAssigned.Contains(RoleId.Get(x.GetType()))).ToList();
                if (attempts < 5) AUSPlugin.DebugLogMessage($"No more roles to assign, refreshing!", AUSPlugin.MsgType.Warning);
            }

            var role = rolesAssignable.Random();
            var customRole = role as ICustomAURole;
            if (customRole != null && role != null)
            {
                var chance = customRole.GetChance();
                var count = customRole.GetCount();
                if (chance != null && count != null)
                {
                    if (CalculatedVoting.ChanceIsNull(chance))
                    {
                        var roleUSHORT = RoleId.Get(customRole.GetType());
                        if (rolesAssigned.Count(x => x.UshortToRole().NiceName == role.NiceName) >= count) AUSPlugin.DebugLogMessage($"Failed to assign {customRole.RoleName} because it has reached its max count!", AUSPlugin.MsgType.Warning);
                        else
                        {
                            gotRole = true;
                            rolesAssigned.Add(roleUSHORT);
                            AUSPlugin.DebugLogMessage($"Assigned {customRole.RoleName}!");
                        }
                    }
                    else AUSPlugin.DebugLogMessage($"Failed to assign {customRole.RoleName} because it failed to roll chance!");
                }
            }

            rolesAssignable.Remove(role);
        }
    }

    public static void AssignCovenRole(List<ushort> rolesAssigned, Alignment alignment, RoleListOption bucket = RoleListOption.None)
    {
        var allRoles = MiscUtils.AllRoles.Where(x => x is ICustomAURole customRole && x is not ISpawnChange &&
            customRole.Alignment == alignment).ToList();

        if (bucket == RoleListOption.RandomCoven) allRoles = MiscUtils.AllRoles.Where(x => x is ICustomAURole customRole && x is not ISpawnChange && customRole.Faction == Faction.Coven).ToList();
        if (bucket == RoleListOption.CommonCoven) allRoles = MiscUtils.AllRoles.Where(x => x is ICustomAURole customRole && x is not ISpawnChange && customRole.Faction == Faction.Coven && !(customRole.Alignment == Alignment.CovenPower || customRole.Alignment == Alignment.CovenKilling)).ToList();

        allRoles.AddRange(allRoles.AddDuplicateRolesToPool());
        var rolesAssignable = allRoles.ToList();

        // Remove Outlier roles from the pool if they're disabled.
        var outliers = allRoles.RemoveOutliers(alignment, bucket);
        allRoles = allRoles.Except(outliers).ToList();

        bool gotRole = false;
        int attempts = 0;
        while (!gotRole && attempts < 50)
        {
            if (rolesAssignable.Count == 0)
            {
                attempts++;
                rolesAssignable = allRoles.Where(x => !rolesAssigned.Contains(RoleId.Get(x.GetType()))).ToList();
                if (attempts < 5) AUSPlugin.DebugLogMessage($"No more roles to assign, refreshing!", AUSPlugin.MsgType.Warning);
            }

            var role = rolesAssignable.Random();
            var customRole = role as ICustomAURole;
            if (customRole != null && role != null)
            {
                var chance = customRole.GetChance();
                var count = customRole.GetCount();
                if (chance != null && count != null)
                {
                    if (CalculatedVoting.ChanceIsNull(chance))
                    {
                        var roleUSHORT = RoleId.Get(customRole.GetType());
                        if (rolesAssigned.Count(x => x.UshortToRole().NiceName == role.NiceName) >= count) AUSPlugin.DebugLogMessage($"Failed to assign {customRole.RoleName} because it has reached its max count!", AUSPlugin.MsgType.Warning);
                        else
                        {
                            gotRole = true;
                            rolesAssigned.Add(roleUSHORT);
                            AUSPlugin.DebugLogMessage($"Assigned {customRole.RoleName}!");
                        }
                    }
                    else AUSPlugin.DebugLogMessage($"Failed to assign {customRole.RoleName} because it failed to roll chance!");
                }
            }

            rolesAssignable.Remove(role);
        }
    }

    public static void AssignTG_TE_TORole(List<ushort> rolesAssigned, RoleListOption bucket = RoleListOption.None)
    {
        var allRoles = MiscUtils.AllRoles.Where(x => x is ICustomAURole customRole && x is not ISpawnChange &&
            (customRole.Alignment == Alignment.TownGovernment || customRole.Alignment == Alignment.TownExecutive || customRole.Alignment == Alignment.TownOutlier)).ToList();

        if (bucket == RoleListOption.RandomTown) allRoles = MiscUtils.AllRoles.Where(x => x is ICustomAURole customRole && x is not ISpawnChange && customRole.Faction == Faction.Town).ToList();
        if (bucket == RoleListOption.CommonTown) allRoles = MiscUtils.AllRoles.Where(x => x is ICustomAURole customRole && x is not ISpawnChange && customRole.Faction == Faction.Town && !(customRole.Alignment == Alignment.TownExecutive || customRole.Alignment == Alignment.TownGovernment)).ToList();

        allRoles.AddRange(allRoles.AddDuplicateRolesToPool());
        var rolesAssignable = allRoles.ToList();

        // Remove Outlier roles from the pool if they're disabled.
        var outliers = allRoles.RemoveOutliers(Alignment.TownOutlier, bucket);
        allRoles = allRoles.Except(outliers).ToList();

        bool gotRole = false;
        int attempts = 0;
        while (!gotRole && attempts < 50)
        {
            if (rolesAssignable.Count == 0)
            {
                attempts++;
                rolesAssignable = allRoles.Where(x => !rolesAssigned.Contains(RoleId.Get(x.GetType()))).ToList();
                if (attempts < 5) AUSPlugin.DebugLogMessage($"No more roles to assign, refreshing!", AUSPlugin.MsgType.Warning);
            }

            var role = rolesAssignable.Random();
            var customRole = role as ICustomAURole;
            if (customRole != null && role != null)
            {
                var chance = customRole.GetChance();
                var count = customRole.GetCount();
                if (chance != null && count != null)
                {
                    if (CalculatedVoting.ChanceIsNull(chance))
                    {
                        var roleUSHORT = RoleId.Get(customRole.GetType());
                        if (rolesAssigned.Count(x => x.UshortToRole().NiceName == role.NiceName) >= count) AUSPlugin.DebugLogMessage($"Failed to assign {customRole.RoleName} because it has reached its max count!", AUSPlugin.MsgType.Warning);
                        else
                        {
                            gotRole = true;
                            rolesAssigned.Add(roleUSHORT);
                            AUSPlugin.DebugLogMessage($"Assigned {customRole.RoleName}!");
                        }
                    }
                    else AUSPlugin.DebugLogMessage($"Failed to assign {customRole.RoleName} because it failed to roll chance!");
                }
            }

            rolesAssignable.Remove(role);
        }
    }

    public static List<RoleBehaviour> RemoveOutliers(this List<RoleBehaviour> list, Alignment alignment = Alignment.None, RoleListOption bucket = RoleListOption.None)
    {
        var allOutliers = new List<RoleBehaviour>();
        if (OptionGroupSingleton<RoleGenOptions>.Instance.EnableAllOutliers)
            return allOutliers;

        // Ignore this if the bucket is supposed to get an Outlier role.
        if (alignment == Alignment.TownOutlier ||
            alignment == Alignment.NeutralOutlier ||
            alignment == Alignment.CovenOutlier) return allOutliers;

        // Ignore this if the bucket is NOT a Random/Common slot.
        if (bucket == RoleListOption.RandomTown || bucket == RoleListOption.CommonTown ||
            bucket == RoleListOption.RandomNeutral ||
            bucket == RoleListOption.RandomCoven || bucket == RoleListOption.CommonCoven) return allOutliers;

        foreach (var role in list.ToList())
        {
            if (role is ICustomAURole customRole)
            {
                if (customRole.Alignment == Alignment.TownOutlier ||
                    customRole.Alignment == Alignment.NeutralOutlier ||
                    customRole.Alignment == Alignment.CovenOutlier)
                {
                    allOutliers.Add(role);
                }
            }
        }

        return allOutliers;
    }

    public static List<RoleBehaviour> AddDuplicateRolesToPool(this List<RoleBehaviour> list)
    {
        var allRoles = new List<RoleBehaviour>();
        if (!OptionGroupSingleton<RoleGenOptions>.Instance.LegacyRoleGen) return allRoles;

        foreach (var role in list.ToList())
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

        return allRoles;
    }

    public static List<RoleListOption> GetBuckets()
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

        return buckets;
    }

    public static RoleBehaviour UshortToRole(this ushort x)
    {
        return RoleManager.Instance.GetRole((RoleTypes)x);
    }
}