using BepInEx.Unity.IL2CPP.Utils;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ObjectWorkshop.LifeImprovement.MCI.SmartMCI;

public static class CalculatedVoting
{
    public static PlayerControl Detective;
    public static PlayerControl Inquisitor;
    public static PlayerControl FortuneTeller;

    public static PlayerControl FortuneSuspicion; // Fortune Teller

    public static PlayerControl PairInfiltratorVotingTarget;
    public static bool infiltratorsAreSkipping;

    public static PlayerControl KillerContagious;
    public static PlayerControl EvidenceAgainst;
    public static float VoteChance = 30f;

    #region Infiltrator
    #endregion
    public static void RandomInfiltratorVoting(PlayerControl player, MeetingHud __instance)
    {
        var alivePlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && !x.IsFaction(Faction.Infiltrator)).ToList();
        var mimics = 0; //PlayerControl.AllPlayerControls.ToArray().Where(x => x.IsRole<Mimic>()).ToList();

        if (mimics > 0/*mimics.Count > 0*/)
        {
            alivePlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && x != player).ToList();
        }

        if (alivePlayers.Count > 0)
        {
            int num = Random.Range(0, 100);

            PlayerControl? playerToVote = null;
            if ((PairInfiltratorVotingTarget != null && num <= 25) || (PairInfiltratorVotingTarget != null && alivePlayers.Count <= 6) || (infiltratorsAreSkipping && num <= 25) && PairInfiltratorVotingTarget != null)
            {
                playerToVote = PairInfiltratorVotingTarget;
            }
            else
            {
                if (num <= 80 && FortuneSuspicion != null && !FortuneSuspicion.IsFaction(Faction.Infiltrator) && !FortuneSuspicion.HasDied() && FortuneSuspicion != null)
                {
                    playerToVote = FortuneSuspicion;
                }
                else
                {
                    if (num <= VoteChance && KillerContagious != null && !KillerContagious.IsFaction(Faction.Infiltrator) && !KillerContagious.HasDied() && KillerContagious != null)
                    {
                        playerToVote = KillerContagious;
                    }
                    else
                    {
                        if (Random.Range(0, 100) <= 5)
                        {
                            infiltratorsAreSkipping = true;
                            __instance.CmdCastVote(player.PlayerId, __instance.SkipVoteButton.TargetPlayerId);
                        }
                        else
                        {
                            PlayerControl newTarget = alivePlayers[Random.RandomRangeInt(0, alivePlayers.Count)];
                            alivePlayers.Remove(newTarget);
                            playerToVote = newTarget;
                            PairInfiltratorVotingTarget = newTarget;
                        }
                    }
                }
            }

            if (playerToVote == null) __instance.CmdCastVote(player.PlayerId, __instance.SkipVoteButton.TargetPlayerId);
            else __instance.CmdCastVote(player.PlayerId, playerToVote.PlayerId);
        }
    }

    #region Crewmate
    #endregion
    public static void RandomCrewmateVoting(PlayerControl player, MeetingHud __instance)
    {
        var alivePlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && x != player && !x.ReceivedInformation()).ToList();

        if (player.GetRevealedPlayers().Count > 0)
        {
            foreach (var revealed in player.GetRevealedPlayers())
            {
                var revealedPlayer = MiscUtils.PlayerById(revealed);
                if (revealedPlayer != null && revealedPlayer.IsFaction(Faction.Crewmate, true)) alivePlayers.Remove(revealedPlayer);
            }
        }
        
        if (alivePlayers.Count > 0)
        {
            int num = Random.Range(0, 100);
            //if (player.Is(RoleEnum.Mimic)) num = 0;

            PlayerControl? playerToVote = null;
            if (num <= 80 && EvidenceAgainst != null && EvidenceAgainst != player && !EvidenceAgainst.HasDied())
            {
                int num2 = Random.Range(0, 100);
                if (num2 <= 20 && Detective != player && Detective != null && !Detective.HasDied() && Detective != null)
                {
                    playerToVote = Detective;
                }
                else
                {
                    if (num2 <= 20 && Inquisitor != player && Inquisitor != null && !Inquisitor.HasDied() && Inquisitor != null)
                    {
                        playerToVote = Inquisitor;
                    }
                    else
                    {
                        playerToVote = EvidenceAgainst;
                    }
                }
            }
            else
            {
                if (num <= VoteChance && KillerContagious != null && KillerContagious != player && !KillerContagious.HasDied() && KillerContagious != null)
                {
                    playerToVote = KillerContagious;
                }
                else
                {
                    int num3 = Random.Range(0, 100);
                    if (num3 <= 80 && FortuneSuspicion != null && FortuneSuspicion != player && !FortuneSuspicion.HasDied() && FortuneSuspicion != null)
                    {
                        if (num3 <= 10 && FortuneTeller != player) playerToVote = FortuneTeller;
                        else playerToVote = FortuneSuspicion;
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
            var stats2 = player.GetPlayerStats();
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

                //if (player.IsRole<Detective>()) Detective = player;

                if (EvidenceAgainst.HasDied())
                {
                    stats.EvidenceAgainst.Remove(EvidenceAgainst);
                    EvidenceAgainst = null;
                    Detective = null;
                    Inquisitor = null;
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

    /*public static void SetFortuneSuspicion(PlayerControl player)
    {
        FortuneTeller role = Role.GetRole<FortuneTeller>(player);
        bool flag = role.EvidenceAgainst.Count > 0 && !player.IsSilenced();
        if (flag)
        {
            role.Suspects.Shuffle<PlayerControl>();
            MCIPlugin.SameVoteAll.FortuneSuspicion = role.Suspects.First<PlayerControl>();
            MCIPlugin.SameVoteAll.FortuneTeller = role.Player;
            bool flag2 = Role.Dead(MCIPlugin.SameVoteAll.FortuneSuspicion);
            if (flag2)
            {
                role.Suspects.Remove(MCIPlugin.SameVoteAll.FortuneSuspicion);
                MCIPlugin.SameVoteAll.FortuneSuspicion = null;
                MCIPlugin.SameVoteAll.FortuneTeller = null;
                MCIPlugin.SetFortuneSuspicion(player);
            }
        }
    }*/
}