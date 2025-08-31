using BepInEx.Unity.IL2CPP.Utils;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement;

public sealed class PlayerStats(byte playerId)
{
    public byte PlayerId { get; set; } = playerId;
    public int CorrectKills { get; set; }
    public int IncorrectKills { get; set; }
    public int CorrectAssassinKills { get; set; }
    public int IncorrectAssassinKills { get; set; }

    public List<PlayerControl> WitnessedKills = new List<PlayerControl>();
    public List<PlayerControl> EvidenceAgainst = new List<PlayerControl>();
    public List<PlayerControl> SlightSuspicion = new List<PlayerControl>();
    public bool ReceivedInformation;
    public List<byte> RevealedPlayers { get; set; } = [];
}

public static class PlayerStatsExtentions
{
    public static bool AddToRevealedPlayers(this PlayerControl player, PlayerControl target, bool showtext = true)
    {
        if (GameHistory.PlayerStats.TryGetValue(player.PlayerId, out var stats))
        {
            if (stats == null)
                return false;

            stats.RevealedPlayers.Add(target.PlayerId);
            if ((player.AmOwner || Debugger.IsDebuggerActive) && showtext)
            {
                MiscUtils.ShowNotification(MessageTexts.RevealRole(target), Color.white);
            }
        }
        return true;
    }

    public static List<byte> GetRevealedPlayers(this PlayerControl player)
    {
        if (GameHistory.PlayerStats.TryGetValue(player.PlayerId, out var stats))
        {
            if (stats == null)
                return new List<byte>();

            return stats.RevealedPlayers;
        }
        return new List<byte>();
    }

    #region ApplyWitnessedKills
    #endregion
    public static bool ApplyWitnessedKills(this PlayerControl player, PlayerControl target)
    {
        if (GameHistory.PlayerStats.TryGetValue(player.PlayerId, out var stats))
        {
            if (stats == null)
                return false;

            stats.WitnessedKills.Add(target);
        }
        return true;
    }

    #region ApplyEvidenceAgainst
    #endregion
    public static bool ApplyEvidenceAgainst(this PlayerControl player, PlayerControl target)
    {
        if (GameHistory.PlayerStats.TryGetValue(player.PlayerId, out var stats))
        {
            if (stats == null)
                return false;

            stats.EvidenceAgainst.Add(target);
        }
        return true;
    }

    #region RemoveWitnessedKills
    #endregion
    public static bool RemoveWitnessedKills(this PlayerControl player, PlayerControl target)
    {
        if (GameHistory.PlayerStats.TryGetValue(player.PlayerId, out var stats))
        {
            if (stats == null)
                return false;

            stats.WitnessedKills.Remove(target);
        }
        return true;
    }

    #region RemoveEvidenceAgainst
    #endregion
    public static bool RemoveEvidenceAgainst(this PlayerControl player, PlayerControl target)
    {
        if (GameHistory.PlayerStats.TryGetValue(player.PlayerId, out var stats))
        {
            if (stats == null)
                return false;

            stats.EvidenceAgainst.Remove(target);
        }
        return true;
    }

    public static List<PlayerControl> GetWitnessedKills(this PlayerControl player)
    {
        if (GameHistory.PlayerStats.TryGetValue(player.PlayerId, out var stats))
        {
            if (stats == null)
                return new List<PlayerControl>();

            return stats.WitnessedKills;
        }
        return new List<PlayerControl>();
    }

    public static List<PlayerControl> GetEvidenceAgainst(this PlayerControl player)
    {
        if (GameHistory.PlayerStats.TryGetValue(player.PlayerId, out var stats))
        {
            if (stats == null)
                return new List<PlayerControl>();

            return stats.EvidenceAgainst;
        }
        return new List<PlayerControl>();
    }

    public static PlayerStats GetPlayerStats(this PlayerControl player)
    {
        if (GameHistory.PlayerStats.TryGetValue(player.PlayerId, out var stats))
        {
            if (stats == null)
                return null;

            return stats;
        }
        return null;
    }
}