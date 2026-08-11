using Random = UnityEngine.Random;

namespace AmongUsSalem.MCI.SmartMCI;

public static class CalculatedVoting
{
    public static byte SkipVote(PlayerControl player, MeetingHud __instance)
    {
        __instance.CmdCastVote(player.PlayerId, __instance.SkipVoteButton.TargetPlayerId);
        return __instance.SkipVoteButton.TargetPlayerId;
    }

    public static byte RandomVote(PlayerControl player, MeetingHud __instance, List<PlayerControl> validTargets, bool canSkip = true)
    {
        bool anyDeath = PlayerControl.AllPlayerControls.ToArray().Any(x => x.IsRole<Death>() && !x.HasDied());
        bool skip = Random.RandomRangeInt(-1, validTargets.Count) == -1;
        if (skip && canSkip && !anyDeath) return SkipVote(player, __instance);

        PlayerControl newTarget = validTargets[Random.RandomRangeInt(0, validTargets.Count)];
        validTargets.Remove(newTarget);
        __instance.CmdCastVote(player.PlayerId, newTarget.PlayerId);
        return newTarget.PlayerId;
    }

    public static byte TryCastVote(PlayerControl player, MeetingHud __instance, byte target)
    {
        __instance.CmdCastVote(player.PlayerId, target);
        return target;
    }

    public static byte TryCastVote(PlayerControl player, MeetingHud __instance, PlayerControl target)
    {
        if (target == null) return SkipVote(player, __instance);

        __instance.CmdCastVote(player.PlayerId, target.PlayerId);
        return target.PlayerId;
    }

    public static void DoVotes(MeetingHud __instance)
    {
        declaredTownTarget = (byte.MinValue, false);
        lastMafiaVoteTarget = (byte.MinValue, false);
        lastCovenVoteTarget = (byte.MinValue, false);
        lastApocVoteTarget = (byte.MinValue, false);

        // var alivePlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied()).ToList();
        bool anyProtest = ModifierUtils.GetActiveModifiers<ProtestModifier>().Any();
        foreach (PlayerControl player in PlayerControl.AllPlayerControls)
        {
            byte voted = 0;
            if (!player.HasDied() && !player.HasModifier<ProtestModifier>())
            {
                if (!player.Is(Faction.Town) && anyProtest) voted = ProtestVoting(player, __instance);
                else if (player.Is(Faction.Mafia)) voted = MafiaVoting(player, __instance);
                else if (player.IsRole<Admirer>()) voted = AdmirerVoting(player, __instance);
                else if (player.Is(Faction.Town)) voted = TownVoting(player, __instance);
                else if (player.IsRole<HexMaster>()) voted = HexMasterVoting(player, __instance);
                else if (player.Is(Faction.Coven)) voted = CovenVoting(player, __instance);
                else if (player.IsRole<Survivor>()) voted = SurvivorVoting(player, __instance);
                else if (player.IsRole<Jester>()) voted = JesterVoting(player, __instance);
                else if (player.Is(Faction.Apocalypse)) voted = ApocVoting(player, __instance);
                else if (player.Is(Faction.SerialKiller)) voted = SerialKillerVoting(player, __instance);
                else if (player.IsRole<Starspawn>()) voted = StarspawnVoting(player, __instance);
                else if (player.Is(Faction.Werewolf)) voted = WerewolfVoting(player, __instance);
                else if (player.IsRole<Amnesiac>()) voted = AmnesiacVoting(player, __instance); // NB Amnesiac!

                if (voted == MeetingHud.Instance.SkipVoteButton.TargetPlayerId) AUS_AfterVoteEvent.RoleFunctionOnSkip(player);
                else AUS_AfterVoteEvent.RoleFunctionOnVote(player, MiscUtils.PlayerById(voted));
            }
            else if (player.HasModifier<ProtestModifier>()) __instance.discussionTimer += 100;
        }
    }

    /// <summary>
    /// If a player is Confirmed Evil, vote them.
    /// If a player witnesses a kill, vote who they saw kill with a % chance. If failed, vote the witness.
    /// If there are less than 7 players, vote a random player that isn't yourself.
    /// Abstain otherwise.
    /// </summary>
    public static (byte, bool) declaredTownTarget = (byte.MinValue, false);
    public static byte TownVoting(PlayerControl player, MeetingHud __instance)
    {
        var allPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied()).ToList();
        var validPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x =>
            !x.HasDied() && x != player && !x.HasModifier<TI>() && !x.HasModifier<Confirmed>() && !x.HasModifier<SoftCleared>() && 
            !(x.Is(Faction.Town) && x.HasModifier<GlobalReveal>())).ToList();

        var seenKill = SeenKill.GetAll();
        var confirmedEvil = ConfirmedEvil.GetAll();
        var incriminatingEvidence = ModifierUtils.GetPlayersWithModifier<IncriminatingEvidence>(x => validPlayers.Contains(x.Player)).ToList();
        var icTarget = incriminatingEvidence.Random();

        if (declaredTownTarget.Item2 && ChanceIs(80)) return TryCastVote(player, __instance, declaredTownTarget.Item1);
        else if (confirmedEvil != null) declaredTownTarget = (TryCastVote(player, __instance, confirmedEvil.Player), true);
        else if (icTarget != null && !icTarget.HasDied()) declaredTownTarget = (TryCastVote(player, __instance, icTarget), true);
        else if (seenKill != null)
        {
            var a = seenKill.killer;
            var b = seenKill.Player;

            int suspicionA = SeenKill.GetSuspicion(a);
            int suspicionB = SeenKill.GetSuspicion(b);

            // Bias by the witness credibility
            if (ChanceIs(seenKill.voteChance)) suspicionA += 25;
            else suspicionB += 25;

            if (suspicionA > suspicionB) return TryCastVote(player, __instance, a);
            else if (suspicionB > suspicionA) return TryCastVote(player, __instance, b);
            else return TryCastVote(player, __instance, UnityEngine.Random.value < 0.5f ? a : b);
        }
        else if (allPlayers.Count < 12 && validPlayers.Count > 0) return RandomVote(player, __instance, validPlayers, allPlayers.Count >= 7);
        else return SkipVote(player, __instance);

        return declaredTownTarget.Item1;
    }

    /// <summary>
    /// Mafia has a 30% chance to vote with each other.
    /// If a Mafia is caught, vote the accuser.
    /// If there are less than 7 players, vote a random Non-Mafia.
    /// Abstain otherwise.
    /// </summary>
    public static (byte, bool) lastMafiaVoteTarget = (byte.MinValue, false);
    public static byte MafiaVoting(PlayerControl player, MeetingHud __instance)
    {
        var allPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied()).ToList();
        var validPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x =>
            !x.HasDied() && !x.Is(Faction.Mafia)).ToList();

        var seenKill = SeenKill.GetAll();

        var incriminatingEvidence = IncriminatingEvidence.GetAll();
        if (incriminatingEvidence != null && incriminatingEvidence.Count >= 2)
        {
            // If one is Mafia and one isn't -> vote the non-Mafia
            var nonMafiaTarget = incriminatingEvidence.FirstOrDefault(p => !p.Player.Is(Faction.Mafia));
            if (nonMafiaTarget != null)
            {
                lastMafiaVoteTarget = (TryCastVote(player, __instance, nonMafiaTarget.Player), true);
                return lastMafiaVoteTarget.Item1;
            }
        }

        if (lastMafiaVoteTarget.Item2 && ChanceIs(30)) return TryCastVote(player, __instance, lastMafiaVoteTarget.Item1);
        else if (seenKill != null && !seenKill.killer.Is(Faction.Mafia) && !seenKill.Player.HasDied()) lastMafiaVoteTarget = (TryCastVote(player, __instance, seenKill.killer.PlayerId), true);
        else if (allPlayers.Count < 12 && validPlayers.Count > 0) lastMafiaVoteTarget = (RandomVote(player, __instance, validPlayers, allPlayers.Count >= 7), true);
        else lastMafiaVoteTarget = (SkipVote(player, __instance), true);

        return lastMafiaVoteTarget.Item1;
    }

    /// <summary>
    /// Coven has a 30% chance to vote with each other.
    /// If a Coven is caught, vote the accuser.
    /// If there are less than 7 players, vote a random Non-Coven.
    /// Abstain otherwise.
    /// </summary>
    public static (byte, bool) lastCovenVoteTarget = (byte.MinValue, false);
    public static byte CovenVoting(PlayerControl player, MeetingHud __instance)
    {
        var allPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied()).ToList();
        var validPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x =>
            !x.HasDied() && !x.Is(Faction.Coven)).ToList();
        
        var seenKill = SeenKill.GetAll();

        var incriminatingEvidence = IncriminatingEvidence.GetAll();
        if (incriminatingEvidence != null && incriminatingEvidence.Count >= 2)
        {
            // If one is Coven and one isn't -> vote the non-Coven
            var nonCovenTarget = incriminatingEvidence.FirstOrDefault(p => !p.Player.Is(Faction.Coven));
            if (nonCovenTarget != null)
            {
                lastCovenVoteTarget = (TryCastVote(player, __instance, nonCovenTarget.Player), true);
                return lastCovenVoteTarget.Item1;
            }
        }

        if (lastCovenVoteTarget.Item2 && ChanceIs(30)) return TryCastVote(player, __instance, lastCovenVoteTarget.Item1);
        else if (seenKill != null && !seenKill.killer.Is(Faction.Coven) && !seenKill.Player.HasDied()) lastCovenVoteTarget = (TryCastVote(player, __instance, seenKill.killer.PlayerId), true);
        else if (allPlayers.Count < 12 && validPlayers.Count > 0) lastCovenVoteTarget = (RandomVote(player, __instance, validPlayers, allPlayers.Count >= 7), true);
        else lastCovenVoteTarget = (SkipVote(player, __instance), true);

        return lastCovenVoteTarget.Item1;
    }

    public static byte HexMasterVoting(PlayerControl player, MeetingHud __instance)
    {
        var allPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied()).ToList();
        var validPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x =>
            !x.HasDied() && !x.Is(Faction.Coven) && !x.HasModifier<HexedModifier>(x => !x.Caster.Is(Faction.Coven))).ToList();

        var seenKill = SeenKill.GetAll();

        var incriminatingEvidence = IncriminatingEvidence.GetAll();
        if (incriminatingEvidence != null && incriminatingEvidence.Count >= 2)
        {
            // If one is Coven and one isn't -> vote the non-Coven
            var nonCovenTarget = incriminatingEvidence.FirstOrDefault(p => !p.Player.Is(Faction.Coven));
            if (nonCovenTarget != null)
            {
                lastCovenVoteTarget = (TryCastVote(player, __instance, nonCovenTarget.Player), true);
                return lastCovenVoteTarget.Item1;
            }
        }

        if (lastCovenVoteTarget.Item2 && ChanceIs(30)) return TryCastVote(player, __instance, lastCovenVoteTarget.Item1);
        else if (seenKill != null && !seenKill.killer.Is(Faction.Coven) && !seenKill.Player.HasDied()) lastCovenVoteTarget = (TryCastVote(player, __instance, seenKill.killer.PlayerId), true);
        else if (allPlayers.Count < 12 && validPlayers.Count > 0) lastCovenVoteTarget = (RandomVote(player, __instance, validPlayers, allPlayers.Count >= 7), true);
        else lastCovenVoteTarget = (SkipVote(player, __instance), true);

        return lastCovenVoteTarget.Item1;
    }

    public static byte SurvivorVoting(PlayerControl player, MeetingHud __instance)
    {
        var allPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied()).ToList();
        var validPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x =>
            !x.HasDied() && x != player).ToList();

        var seenKill = SeenKill.GetAll();
        var confirmedEvil = ConfirmedEvil.GetAll();
        var incriminatingEvidence = IncriminatingEvidence.GetRandom();

        if (declaredTownTarget.Item2 && ChanceIs(50)) return TryCastVote(player, __instance, declaredTownTarget.Item1);
        else if (lastCovenVoteTarget.Item2 && ChanceIs(15)) return TryCastVote(player, __instance, lastCovenVoteTarget.Item1);
        else if (lastMafiaVoteTarget.Item2 && ChanceIs(15)) return TryCastVote(player, __instance, lastMafiaVoteTarget.Item1);
        else if (confirmedEvil != null) return TryCastVote(player, __instance, confirmedEvil.Player);
        else if (incriminatingEvidence != null) return TryCastVote(player, __instance, incriminatingEvidence.Player);
        else if (seenKill != null)
        {
            if (ChanceIs(50)) return TryCastVote(player, __instance, seenKill.killer);
            else return TryCastVote(player, __instance, seenKill.Player);
        }
        else if (allPlayers.Count < 12 && validPlayers.Count > 0) return RandomVote(player, __instance, validPlayers);
        return SkipVote(player, __instance);
    }

    public static byte JesterVoting(PlayerControl player, MeetingHud __instance)
    {
        var allPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied()).ToList();
        if (allPlayers.Count < 12 && DayNightMechanic.DayCount > 1) return TryCastVote(player, __instance, player);
        return SkipVote(player, __instance);
    }

    public static (byte, bool) lastApocVoteTarget = (byte.MinValue, false);
    public static byte ApocVoting(PlayerControl player, MeetingHud __instance)
    {
        var allPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied()).ToList();
        var validPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x =>
            !x.HasDied() && !x.Is(Faction.Apocalypse)).ToList();

        var seenKill = SeenKill.GetAll();

        var incriminatingEvidence = IncriminatingEvidence.GetAll();
        if (incriminatingEvidence != null && incriminatingEvidence.Count >= 2)
        {
            // If one is Apoc and one isn't -> vote the non-Apoc
            var nonApocTarget = incriminatingEvidence.FirstOrDefault(p => !p.Player.Is(Faction.Apocalypse));
            if (nonApocTarget != null)
            {
                lastApocVoteTarget = (TryCastVote(player, __instance, nonApocTarget.Player), true);
                return lastApocVoteTarget.Item1;
            }
        }

        if (lastApocVoteTarget.Item2 && ChanceIs(30)) return TryCastVote(player, __instance, lastApocVoteTarget.Item1);
        else if (seenKill != null && !seenKill.killer.Is(Faction.Apocalypse) && !seenKill.Player.HasDied()) lastApocVoteTarget = (TryCastVote(player, __instance, seenKill.killer.PlayerId), true);
        else if (allPlayers.Count < 12 && validPlayers.Count > 0) lastApocVoteTarget = (RandomVote(player, __instance, validPlayers, allPlayers.Count >= 7), true);
        else lastApocVoteTarget = (SkipVote(player, __instance), true);

        return lastApocVoteTarget.Item1;
    }

    public static byte SerialKillerVoting(PlayerControl player, MeetingHud __instance)
    {
        var allPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied()).ToList();
        var validPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x =>
            !x.HasDied() && x != player).ToList();

        var seenKill = SeenKill.GetAll();

        var incriminatingEvidence = IncriminatingEvidence.GetAll();
        if (incriminatingEvidence != null && incriminatingEvidence.Count >= 2)
        {
            // If one is Coven and one isn't -> vote the non-Coven
            var target = incriminatingEvidence.FirstOrDefault(p => p.Player != player);
            if (target != null) return TryCastVote(player, __instance, target.Player);
        }

        if (seenKill != null && !seenKill.killer.IsRole<SerialKiller>() && !seenKill.Player.HasDied()) return TryCastVote(player, __instance, seenKill.killer.PlayerId);
        else if (allPlayers.Count < 12 && validPlayers.Count > 0) return RandomVote(player, __instance, validPlayers, allPlayers.Count >= 7);
        
        return SkipVote(player, __instance);
    }

    public static byte WerewolfVoting(PlayerControl player, MeetingHud __instance)
    {
        var allPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied()).ToList();
        var validPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x =>
            !x.HasDied() && x != player).ToList();

        var seenKill = SeenKill.GetAll();

        var incriminatingEvidence = IncriminatingEvidence.GetAll();
        if (incriminatingEvidence != null && incriminatingEvidence.Count >= 2)
        {
            // If one is Coven and one isn't -> vote the non-Coven
            var target = incriminatingEvidence.FirstOrDefault(p => p.Player != player);
            if (target != null) return TryCastVote(player, __instance, target.Player);
        }

        if (seenKill != null && !seenKill.killer.IsRole<Werewolf>() && !seenKill.Player.HasDied()) return TryCastVote(player, __instance, seenKill.killer.PlayerId);
        else if (allPlayers.Count < 12 && validPlayers.Count > 0) return RandomVote(player, __instance, validPlayers, allPlayers.Count >= 7);

        return SkipVote(player, __instance);
    }

    public static byte StarspawnVoting(PlayerControl player, MeetingHud __instance)
    {
        var allPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied()).ToList();
        var validPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x =>
            !x.HasDied() && x != player && !x.HasModifier<IncriminatingEvidence>() && !x.HasModifier<ConfirmedEvil>()).ToList();

        var seenKill = SeenKill.GetAll();
        if (lastCovenVoteTarget.Item2 && ChanceIs(90)) return TryCastVote(player, __instance, lastCovenVoteTarget.Item1);
        else if (lastMafiaVoteTarget.Item2 && ChanceIs(90)) return TryCastVote(player, __instance, lastMafiaVoteTarget.Item1);
        else if (lastApocVoteTarget.Item2 && ChanceIs(90)) return TryCastVote(player, __instance, lastApocVoteTarget.Item1);
        else if (seenKill != null)
        {
            if (ChanceIs(10)) return TryCastVote(player, __instance, seenKill.killer);
            else return TryCastVote(player, __instance, seenKill.Player);
        }
        else if (allPlayers.Count < 12 && validPlayers.Count > 0) return RandomVote(player, __instance, validPlayers);
        return SkipVote(player, __instance);
    }

    public static byte ProtestVoting(PlayerControl player, MeetingHud __instance)
    {
        var validPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x =>
            !x.HasDied() && x != player && !x.HasModifier<IncriminatingEvidence>() && !x.HasModifier<ConfirmedEvil>() &&
            !x.HasModifier<SeenKill>() && !x.HasModifier<TI>() &&
            !x.HasModifier<ProtestModifier>()).ToList();

        if (lastCovenVoteTarget.Item2 && ChanceIs(90) && player.Is(Faction.Coven)) return TryCastVote(player, __instance, lastCovenVoteTarget.Item1);
        else if (lastMafiaVoteTarget.Item2 && ChanceIs(90) && player.Is(Faction.Mafia)) return TryCastVote(player, __instance, lastMafiaVoteTarget.Item1);
        else if (lastApocVoteTarget.Item2 && ChanceIs(90) && player.Is(Faction.Apocalypse)) return TryCastVote(player, __instance, lastApocVoteTarget.Item1);
        else if (validPlayers.Count > 0) return RandomVote(player, __instance, validPlayers, false);
        return SkipVote(player, __instance);
    }

    public static byte AmnesiacVoting(PlayerControl player, MeetingHud __instance) => SurvivorVoting(player, __instance);
    public static byte AdmirerVoting(PlayerControl player, MeetingHud __instance)
    {
        var admirer = player.GetRole<Admirer>();
        var allPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied()).ToList();
        var validPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x =>
            !x.HasDied() && x != player && !x.HasModifier<TI>() && !x.HasModifier<Confirmed>() && !x.HasModifier<SoftCleared>() &&
            !(x.Is(Faction.Town) && x.HasModifier<GlobalReveal>()) &&
            !(admirer.Obsession == x && admirer.foundObsession)).ToList();

        var seenKill = SeenKill.GetAll();
        var confirmedEvil = ConfirmedEvil.GetAll();
        var incriminatingEvidence = ModifierUtils.GetPlayersWithModifier<IncriminatingEvidence>(x => validPlayers.Contains(x.Player)).ToList();
        var icTarget = incriminatingEvidence.Random();

        if (declaredTownTarget.Item2 && ChanceIs(80)) return TryCastVote(player, __instance, declaredTownTarget.Item1);
        else if (confirmedEvil != null) declaredTownTarget = (TryCastVote(player, __instance, confirmedEvil.Player), true);
        else if (icTarget != null && !icTarget.HasDied()) declaredTownTarget = (TryCastVote(player, __instance, icTarget), true);
        else if (seenKill != null)
        {
            var a = seenKill.killer;
            var b = seenKill.Player;

            int suspicionA = SeenKill.GetSuspicion(a);
            int suspicionB = SeenKill.GetSuspicion(b);

            // Bias by the witness credibility
            if (ChanceIs(seenKill.voteChance)) suspicionA += 25;
            else suspicionB += 25;

            if (suspicionA > suspicionB) return TryCastVote(player, __instance, a);
            else if (suspicionB > suspicionA) return TryCastVote(player, __instance, b);
            else return TryCastVote(player, __instance, UnityEngine.Random.value < 0.5f ? a : b);
        }
        else if (allPlayers.Count < 12 && validPlayers.Count > 0) return RandomVote(player, __instance, validPlayers, allPlayers.Count >= 7);
        else return SkipVote(player, __instance);

        return declaredTownTarget.Item1;
    }

    public static bool ChanceIsNull(int? num)
    {
        var r = Random.Range(0, 100);
        return r < num;
    }

    public static bool ChanceIsFloat(float num)
    {
        var r = Random.Range(0f, 100f);
        return r < num;
    }

    public static bool ChanceIs(int num)
    {
        var r = Random.Range(0, 100);
        return r < num;
    }
}