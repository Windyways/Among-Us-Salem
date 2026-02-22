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
        bool skip = Random.RandomRangeInt(-1, validTargets.Count) == -1;
        if (skip && canSkip) return SkipVote(player, __instance);

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
        lastMafiaVoteTarget = (byte.MinValue, false);

        var alivePlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied()).ToList();
        foreach (PlayerControl player in PlayerControl.AllPlayerControls)
        {
            if (!player.HasDied())
            {
                if (player.Is(Faction.Mafia)) MafiaVoting(player, __instance);
                else if (player.Is(Faction.Town)) TownVoting(player, __instance);
                else if (player.Is(Faction.Coven)) CovenVoting(player, __instance);
            }
        }
    }

    /// <summary>
    /// If a player is Confirmed Evil, vote them.
    /// If a player witnesses a kill, vote who they saw kill with a % chance. If failed, vote the witness.
    /// If there are less than 7 players, vote a random player that isn't yourself.
    /// Abstain otherwise.
    /// </summary>
    public static void TownVoting(PlayerControl player, MeetingHud __instance)
    {
        var allPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied()).ToList();
        var validPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x =>
            !x.HasDied() && x != player && !x.HasModifier<TI>() && !x.HasModifier<Confirmed>()).ToList();

        var seenKill = SeenKill.GetAll();
        var confirmedEvil = ConfirmedEvil.GetAll();
        var incriminatingEvidence = IncriminatingEvidence.GetAll();

        if (confirmedEvil != null) TryCastVote(player, __instance, confirmedEvil.Player);
        else if (incriminatingEvidence != null) TryCastVote(player, __instance, incriminatingEvidence.Player);
        else if (seenKill != null)
        {
            if (ChanceIs(seenKill.voteChance)) TryCastVote(player, __instance, seenKill.killer);
            else TryCastVote(player, __instance, seenKill.Player);
        }
        else if (allPlayers.Count < 12) RandomVote(player, __instance, validPlayers, allPlayers.Count < 7);
        else SkipVote(player, __instance);
    }

    /// <summary>
    /// Mafia has a 30% chance to vote with each other.
    /// If a Mafia is caught, vote the accuser.
    /// If there are less than 7 players, vote a random Non-Mafia.
    /// Abstain otherwise.
    /// </summary>
    public static (byte, bool) lastMafiaVoteTarget = (byte.MinValue, false);
    public static void MafiaVoting(PlayerControl player, MeetingHud __instance)
    {
        var allPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied()).ToList();
        var validPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x =>
            !x.HasDied() && !x.Is(Faction.Mafia)).ToList();

        var seenKill = SeenKill.GetAll();
        var incriminatingEvidence = IncriminatingEvidence.GetAll();

        if (lastMafiaVoteTarget.Item2 && ChanceIs(30)) TryCastVote(player, __instance, lastMafiaVoteTarget.Item1);
        else if (incriminatingEvidence != null && !incriminatingEvidence.Player.Is(Faction.Mafia)) TryCastVote(player, __instance, incriminatingEvidence.Player);
        else if (seenKill != null && seenKill.killer.Is(Faction.Mafia) && !seenKill.Player.HasDied()) lastMafiaVoteTarget = (TryCastVote(player, __instance, seenKill.Player.PlayerId), true);
        else if (seenKill != null)
        {
            if (ChanceIs(seenKill.voteChance)) TryCastVote(player, __instance, seenKill.killer);
            else TryCastVote(player, __instance, seenKill.Player);
        }
        else if (allPlayers.Count < 12) lastMafiaVoteTarget = (RandomVote(player, __instance, validPlayers), true);
        lastMafiaVoteTarget = (SkipVote(player, __instance), true);
    }

    /// <summary>
    /// Coven has a 30% chance to vote with each other.
    /// If a Coven is caught, vote the accuser.
    /// If there are less than 7 players, vote a random Non-Coven.
    /// Abstain otherwise.
    /// </summary>
    public static (byte, bool) lastCovenVoteTarget = (byte.MinValue, false);
    public static void CovenVoting(PlayerControl player, MeetingHud __instance)
    {
        var allPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied()).ToList();
        var validPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x =>
            !x.HasDied() && !x.Is(Faction.Coven)).ToList();

        var seenKill = SeenKill.GetAll();
        var incriminatingEvidence = IncriminatingEvidence.GetAll();

        if (lastCovenVoteTarget.Item2 && ChanceIs(30)) TryCastVote(player, __instance, lastCovenVoteTarget.Item1);
        else if (incriminatingEvidence != null && !incriminatingEvidence.Player.Is(Faction.Coven)) TryCastVote(player, __instance, incriminatingEvidence.Player);
        else if (seenKill != null && seenKill.killer.Is(Faction.Coven) && !seenKill.Player.HasDied()) lastCovenVoteTarget = (TryCastVote(player, __instance, seenKill.Player.PlayerId), true);
        else if (seenKill != null)
        {
            if (ChanceIs(seenKill.voteChance)) TryCastVote(player, __instance, seenKill.killer);
            else TryCastVote(player, __instance, seenKill.Player);
        }
        else if (allPlayers.Count < 12) lastCovenVoteTarget = (RandomVote(player, __instance, validPlayers), true);
        lastCovenVoteTarget = (SkipVote(player, __instance), true);
    }

    public static bool ChanceIs(int num)
    {
        var r = Random.Range(0, 100);
        return r < num;
    }
}