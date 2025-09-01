using BepInEx.Unity.IL2CPP.Utils;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AmongUsSalem.LifeImprovement.MCI.SmartMCI;

public static class CalculatedVoting
{
    public static List<PlayerControl>  KillerContagious;
    public static List<PlayerControl> EvidenceAgainst;
    public static float VoteChance = 75f;

    #region Coven
    #endregion
    public static PlayerControl PairCovenVotingTarget;
    public static bool covensAreSkipping;
    public static void RandomCovenVoting(PlayerControl player, MeetingHud __instance)
    {
        KillerContagious.Shuffle();
        var killerContagiousToVote = KillerContagious[0];
        
        var alivePlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && !x.Is(Faction.Coven)).ToList();
        if (alivePlayers.Count > 0)
        {
            int num = Random.Range(0, 100);

            PlayerControl? playerToVote = null;
            if ((PairCovenVotingTarget != null && num <= 25) || (PairCovenVotingTarget != null && alivePlayers.Count <= 6) || (covensAreSkipping && num <= 25) && PairCovenVotingTarget != null)
            {
                playerToVote = PairCovenVotingTarget;
            }
            else
            {

                if (num <= VoteChance && killerContagiousToVote != null && !killerContagiousToVote.Is(Faction.Coven) && !killerContagiousToVote.HasDied())
                {
                    playerToVote = killerContagiousToVote;
                }
                else
                {
                    if (Random.Range(0, 100) <= 5)
                    {
                        covensAreSkipping = true;
                        __instance.CmdCastVote(player.PlayerId, __instance.SkipVoteButton.TargetPlayerId);
                    }
                    else
                    {
                        PlayerControl newTarget = alivePlayers[Random.RandomRangeInt(0, alivePlayers.Count)];
                        alivePlayers.Remove(newTarget);
                        playerToVote = newTarget;
                        PairCovenVotingTarget = newTarget;
                    }
                }
            }

            if (playerToVote == null) __instance.CmdCastVote(player.PlayerId, __instance.SkipVoteButton.TargetPlayerId);
            else __instance.CmdCastVote(player.PlayerId, playerToVote.PlayerId);
        }
    }

    #region Mafia
    #endregion
    public static PlayerControl PairMafiaVotingTarget;
    public static bool mafiasAreSkipping;
    public static void RandomMafiaVoting(PlayerControl player, MeetingHud __instance)
    {
        KillerContagious.Shuffle();
        var killerContagiousToVote = KillerContagious[0];
        
        var alivePlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && !x.Is(Faction.Mafia)).ToList();
        if (alivePlayers.Count > 0)
        {
            int num = Random.Range(0, 100);

            PlayerControl? playerToVote = null;
            if ((PairMafiaVotingTarget != null && num <= 25) || (PairMafiaVotingTarget != null && alivePlayers.Count <= 6) || (mafiasAreSkipping && num <= 25) && PairMafiaVotingTarget != null)
            {
                playerToVote = PairMafiaVotingTarget;
            }
            else
            {

                if (num <= VoteChance && killerContagiousToVote != null && !killerContagiousToVote.Is(Faction.Mafia) && !killerContagiousToVote.HasDied())
                {
                    playerToVote = killerContagiousToVote;
                }
                else
                {
                    if (Random.Range(0, 100) <= 5)
                    {
                        mafiasAreSkipping = true;
                        __instance.CmdCastVote(player.PlayerId, __instance.SkipVoteButton.TargetPlayerId);
                    }
                    else
                    {
                        PlayerControl newTarget = alivePlayers[Random.RandomRangeInt(0, alivePlayers.Count)];
                        alivePlayers.Remove(newTarget);
                        playerToVote = newTarget;
                        PairMafiaVotingTarget = newTarget;
                    }
                }
            }

            if (playerToVote == null) __instance.CmdCastVote(player.PlayerId, __instance.SkipVoteButton.TargetPlayerId);
            else __instance.CmdCastVote(player.PlayerId, playerToVote.PlayerId);
        }
    }

    #region Town
    #endregion
    public static void RandomTownVoting(PlayerControl player, MeetingHud __instance)
    {
        KillerContagious.Shuffle();
        var killerContagiousToVote = KillerContagious[0];
        
        EvidenceAgainst.Shuffle();
        var evidenceAgainstToVote = EvidenceAgainst[0];
        
        var alivePlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && x != player && !x.ReceivedInformation()).ToList();

        if (player.GetRevealedPlayers().Count > 0)
        {
            foreach (var revealed in player.GetRevealedPlayers())
            {
                var revealedPlayer = MiscUtils.PlayerById(revealed);
                if (revealedPlayer != null && revealedPlayer.Is(Faction.Town)) alivePlayers.Remove(revealedPlayer);
            }
        }
        
        if (alivePlayers.Count > 0)
        {
            int num = Random.Range(0, 100);

            PlayerControl? playerToVote = null;
            if (num <= 80 && evidenceAgainstToVote != null && evidenceAgainstToVote != player && !evidenceAgainstToVote.HasDied())
            {
                playerToVote = evidenceAgainstToVote;
            }
            else
            {
                if (num <= VoteChance && killerContagiousToVote != null && killerContagiousToVote != player && !killerContagiousToVote.HasDied())
                {
                    playerToVote = killerContagiousToVote;
                }
                else
                {
                    int num4 = Random.Range(0, 100);
                    if (num4 <= 5 && alivePlayers.Count > 5)
                    {
                        __instance.CmdCastVote(player.PlayerId, __instance.SkipVoteButton.TargetPlayerId);
                    }
                    else
                    {
                        PlayerControl newTarget = alivePlayers[Random.RandomRangeInt(0, alivePlayers.Count)];
                        alivePlayers.Remove(newTarget);
                        playerToVote = newTarget;
                    }
                }
            }

            if (playerToVote == null) __instance.CmdCastVote(player.PlayerId, __instance.SkipVoteButton.TargetPlayerId);
            else __instance.CmdCastVote(player.PlayerId, playerToVote.PlayerId);
        }
    }

    #region Neutral
    #endregion
    public static void RandomNeutralVoting(PlayerControl player, MeetingHud __instance)
    {
        var alivePlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && x != player).ToList();
        if (alivePlayers.Count > 0)
        {
            PlayerControl? playerToVote = null;

            PlayerControl newTarget = alivePlayers[Random.RandomRangeInt(0, alivePlayers.Count)];
            alivePlayers.Remove(newTarget);
            playerToVote = newTarget;

            if (playerToVote == null) __instance.CmdCastVote(player.PlayerId, __instance.SkipVoteButton.TargetPlayerId);
            else __instance.CmdCastVote(player.PlayerId, playerToVote.PlayerId);
        }
    }

    #region KillerContagious
    #endregion
    public static void SetKillerContagious(PlayerControl player)
    {
        var role = player.GetOWRole();
        if (role != null)
            return;

        var stats = player.GetPlayerStats();
        if (stats != null)
        {
            if (stats.WitnessedKills.Count > 0)
            {
                foreach (var witnessed in stats.WitnessedKills)
                {
                    if (!KillerContagious.Contains(witnessed)) KillerContagious.Add(witnessed);
                    if (witnessed.HasDied())
                    {
                        stats.WitnessedKills.Remove(witnessed);
                        KillerContagious.Remove(witnessed);
                    }
                }
            }
        }
    }

    #region EvidenceAgainst
    #endregion
    public static void SetEvidenceAgainst(PlayerControl player)
    {
        var stats = player.GetPlayerStats();
        if (stats != null)
        {
            if (stats.EvidenceAgainst.Count > 0)
            {
                foreach (var witnessed in stats.EvidenceAgainst)
                {
                    if (!EvidenceAgainst.Contains(witnessed)) EvidenceAgainst.Add(witnessed);
                    if (witnessed.HasDied())
                    {
                        stats.EvidenceAgainst.Remove(witnessed);
                        EvidenceAgainst.Remove(witnessed);
                    }
                }
            }
        }
    }

    #region ReceivedInformation
    #endregion
    public static bool ReceivedInformation(this PlayerControl player)
    {
        if (GameHistory.PlayerStats.TryGetValue(player.PlayerId, out var stats))
        {
            if (stats == null)
                return false;

            if (stats.ReceivedInformation) return true;
        }
        return false;
    }
}