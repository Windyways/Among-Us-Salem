using BepInEx.Unity.IL2CPP.Utils;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AmongUsSalem.LifeImprovement.MCI.SmartMCI;

public static class CalculatedVoting
{
    public static PlayerControl PairMafiaVotingTarget;
    public static bool mafiasAreSkipping;

    public static PlayerControl KillerContagious;
    public static PlayerControl EvidenceAgainst;
    public static float VoteChance = 30f;

    #region Mafia
    #endregion
    public static void RandomMafiaVoting(PlayerControl player, MeetingHud __instance)
    {
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

                if (num <= VoteChance && KillerContagious != null && !KillerContagious.Is(Faction.Mafia) && !KillerContagious.HasDied() && KillerContagious != null)
                {
                    playerToVote = KillerContagious;
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
            //if (player.Is(RoleEnum.Mimic)) num = 0;

            PlayerControl? playerToVote = null;
            if (num <= 80 && EvidenceAgainst != null && EvidenceAgainst != player && !EvidenceAgainst.HasDied())
            {
                playerToVote = EvidenceAgainst;
            }
            else
            {
                if (num <= VoteChance && KillerContagious != null && KillerContagious != player && !KillerContagious.HasDied() && KillerContagious != null)
                {
                    playerToVote = KillerContagious;
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
        if (stats.WitnessedKills.Count > 0)// && !player.IsSilenced())
        {
            KillerContagious = stats.WitnessedKills[0];
            if (KillerContagious.HasDied())
            {
                stats.WitnessedKills.Remove(KillerContagious);
                KillerContagious = null;
                SetKillerContagious(player);
            }
        }
        }

        int setEvidenceAgainst = 0;
        foreach (PlayerControl players in PlayerControl.AllPlayerControls)
        {
            var stats2 = players.GetPlayerStats();
            if (stats2 != null && KillerContagious != null)
            {
                if (stats2.WitnessedKills.Contains(KillerContagious))
                {
                    setEvidenceAgainst++;
                }
            }
        }

        if (setEvidenceAgainst >= 2) VoteChance = 50f;
        if (setEvidenceAgainst >= 3) VoteChance = 75f;
        if (setEvidenceAgainst >= 4) VoteChance = 90f;
    }

    #region EvidenceAgainst
    #endregion
    public static void SetEvidenceAgainst(PlayerControl player)
    {
        if (GameHistory.PlayerStats.TryGetValue(player.PlayerId, out var stats))
        {
            if (stats == null)
                return;

            if (stats.EvidenceAgainst.Count > 0)// && !player.IsSilenced())
            {
                EvidenceAgainst = stats.EvidenceAgainst[0];

                if (EvidenceAgainst.HasDied())
                {
                    stats.EvidenceAgainst.Remove(EvidenceAgainst);
                    EvidenceAgainst = null;
                    SetEvidenceAgainst(player);
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