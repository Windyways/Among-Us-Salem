using BepInEx.Unity.IL2CPP.Utils;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AmongUsSalem.LifeImprovement.MCI.SmartMCI;

public static class CalculatedVoting
{
    public static List<PlayerControl> KillerContagious = new List<PlayerControl>();
    public static List<PlayerControl> EvidenceAgainst = new List<PlayerControl>();
    public static List<PlayerControl> RecievedInformation = new List<PlayerControl>();
    
    public static Dictionary<PlayerControl, PlayerControl> QueueKillerContagious = new Dictionary<PlayerControl, PlayerControl>();
    public static Dictionary<PlayerControl, PlayerControl> QueueEvidenceAgainst = new Dictionary<PlayerControl, PlayerControl>();
    public static List<PlayerControl> QueueRecievedInformation = new List<PlayerControl>();

    #region Vampire
    #endregion
    public static PlayerControl PairVampireVotingTarget;
    public static bool vampiresAreSkipping;
    public static void RandomVampireVoting(PlayerControl player, MeetingHud __instance)
    {
        var alive = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied());
        var vampire = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.IsRole<Vampire>() && x.HasModifier<VampireRecruit>());

        var alivePlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && !x.IsRole<Vampire>() && !x.HasModifier<VampireRecruit>()).ToList();
        if (alivePlayers.Count > 0)
        {
            var skipNum = Random.Range(0, 100);
            var pairNum = Random.Range(0, 100);
            if (skipNum <= 75 && vampiresAreSkipping)
            {
                SkipVote(player, __instance);
                vampiresAreSkipping = true;
            }
            else if (pairNum <= 75 && PairVampireVotingTarget != null && alive <= vampire)
            {
                __instance.CmdCastVote(player.PlayerId, PairVampireVotingTarget.PlayerId);
            }
            else
            {
                PairVampireVotingTarget = RandomVote(player, __instance, alivePlayers);
            }
        }
        else
        {
            SkipVote(player, __instance);
            vampiresAreSkipping = true;
        }
    }
    #region Recruit
    #endregion
    public static PlayerControl PairJackalRecruitVotingTarget;
    public static bool jackalRecruitsAreSkipping;
    public static void RandomJackalRecruitVoting(PlayerControl player, MeetingHud __instance)
    {
        var alive = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied());
        var recsJackal = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.IsRole<Jackal>() && x.HasModifier<JackalRecruit>());

        var alivePlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && !x.HasModifier<JackalRecruit>()).ToList();
        if (alivePlayers.Count > 0)
        {
            var skipNum = Random.Range(0, 100);
            var pairNum = Random.Range(0, 100);
            if (skipNum <= 75 && jackalRecruitsAreSkipping)
            {
                SkipVote(player, __instance);
                jackalRecruitsAreSkipping = true;
            }
            else if (pairNum <= 75 && PairJackalRecruitVotingTarget != null && alive <= recsJackal)
            {
                __instance.CmdCastVote(player.PlayerId, PairJackalRecruitVotingTarget.PlayerId);
            }
            else
            {
                PairJackalRecruitVotingTarget = RandomVote(player, __instance, alivePlayers);
            }
        }
        else
        {
            SkipVote(player, __instance);
            jackalRecruitsAreSkipping = true;
        }
    }
    
    public static void RandomJackalVoting(PlayerControl player, MeetingHud __instance)
    {
        var alive = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied());
        var recsJackal = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.IsRole<Jackal>() && x.HasModifier<JackalRecruit>());

        var alivePlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && !x.IsRole<Jackal>() && !x.HasModifier<JackalRecruit>()).ToList();
        if (alivePlayers.Count > 0)
        {
            var skipNum = Random.Range(0, 100);
            var pairNum = Random.Range(0, 100);
            if (skipNum <= 75 && jackalRecruitsAreSkipping)
            {
                SkipVote(player, __instance);
                jackalRecruitsAreSkipping = true;
            }
            else if (pairNum <= 75 && PairJackalRecruitVotingTarget != null && alive <= recsJackal)
            {
                __instance.CmdCastVote(player.PlayerId, PairJackalRecruitVotingTarget.PlayerId);
            }
            else
            {
                PairJackalRecruitVotingTarget = RandomVote(player, __instance, alivePlayers);
            }
        }
        else
        {
            SkipVote(player, __instance);
            jackalRecruitsAreSkipping = true;
        }
    }
    #region Coven
    #endregion
    public static PlayerControl PairCovenVotingTarget;
    public static bool covensAreSkipping;
    public static void RandomCovenVoting(PlayerControl player, MeetingHud __instance)
    {
        var alive = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied());
        var coven = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.Is(Faction.Coven));

        var alivePlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && !x.Is(Faction.Coven)).ToList();
        if (alivePlayers.Count > 0)
        {
            var skipNum = Random.Range(0, 100);
            var pairNum = Random.Range(0, 100);
            if (skipNum <= 75 && covensAreSkipping)
            {
                SkipVote(player, __instance);
                covensAreSkipping = true;
            }
            else if (pairNum <= 75 && PairCovenVotingTarget != null && alive <= coven)
            {
                __instance.CmdCastVote(player.PlayerId, PairCovenVotingTarget.PlayerId);
            }
            else
            {
                PairCovenVotingTarget = RandomVote(player, __instance, alivePlayers);
            }
        }
        else
        {
            SkipVote(player, __instance);
            covensAreSkipping = true;
        }
    }

    #region Mafia
    #endregion
    public static PlayerControl PairMafiaVotingTarget;
    public static bool mafiasAreSkipping;
    public static void RandomMafiaVoting(PlayerControl player, MeetingHud __instance)
    {
        var alive = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied());
        var mafia = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.Is(Faction.Mafia));

        var alivePlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && !x.Is(Faction.Mafia)).ToList();
        if (alivePlayers.Count > 0)
        {
            var skipNum = Random.Range(0, 100);
            var pairNum = Random.Range(0, 100);
            if (skipNum <= 75 && mafiasAreSkipping)
            {
                SkipVote(player, __instance);
                mafiasAreSkipping = true;
            }
            else if (pairNum <= 75 && PairMafiaVotingTarget != null && alive <= mafia)
            {
                __instance.CmdCastVote(player.PlayerId, PairMafiaVotingTarget.PlayerId);
            }
            else
            {
                PairMafiaVotingTarget = RandomVote(player, __instance, alivePlayers);
            }
        }
        else
        {
            SkipVote(player, __instance);
            mafiasAreSkipping = true;
        }
    }

    public static void SkipVote(PlayerControl player, MeetingHud __instance)
    {
        __instance.CmdCastVote(player.PlayerId, __instance.SkipVoteButton.TargetPlayerId);
    }

    public static PlayerControl RandomVote(PlayerControl player, MeetingHud __instance, List<PlayerControl> validTargets)
    {
        PlayerControl newTarget = validTargets[Random.RandomRangeInt(0, validTargets.Count)];
        validTargets.Remove(newTarget);
        __instance.CmdCastVote(player.PlayerId, newTarget.PlayerId);
        return newTarget;
    }

    #region Town
    #endregion
    public static void RandomTownVoting(PlayerControl player, MeetingHud __instance)
    {
        var alivePlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && x != player && !RecievedInformation.Contains(x) && 
            !(x.HasModifier<GlobalReveal>() && x.Is(Faction.Town))).ToList();

        if (alivePlayers.Count > 0)
        {
            var eaNum = Random.Range(0, 100);
            var kcNum = Random.Range(0, 100);
            if (EvidenceAgainst.Count > 0 && eaNum <= 80) __instance.CmdCastVote(player.PlayerId, EvidenceAgainst.Random().PlayerId);
            else if (KillerContagious.Count > 0 && kcNum <= 50) __instance.CmdCastVote(player.PlayerId, KillerContagious.Random().PlayerId);
            else RandomVote(player, __instance, alivePlayers);
        }
        else SkipVote(player, __instance);
    }

    #region Neutral
    #endregion
    public static void RandomNeutralVoting(PlayerControl player, MeetingHud __instance)
    {
        var alivePlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && x != player).ToList();
        if (alivePlayers.Count > 0)
        {
            RandomVote(player, __instance, alivePlayers);
        }
        else SkipVote(player, __instance);
    }

    #region Arsonist
    #endregion
    public static void RandomArsonistVoting(PlayerControl player, MeetingHud __instance)
    {
        var arsonist = player.GetRole<Arsonist>();
        var alivePlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && x != player && !arsonist.DousedPlayers.Contains(x.PlayerId)).ToList();

        if (alivePlayers.Count > 0)
        {
            var eaNum = Random.Range(0, 100);
            if (EvidenceAgainst.Count > 0 && eaNum <= 100) __instance.CmdCastVote(player.PlayerId, EvidenceAgainst.Random().PlayerId);
            else RandomVote(player, __instance, alivePlayers);
        }
        else SkipVote(player, __instance);
    }

    public static void DoVotes(MeetingHud __instance)
    {
        foreach (PlayerControl player in PlayerControl.AllPlayerControls)
        {
            if (!player.HasDied())
            {
                if (player.IsRole<Arsonist>()) CalculatedVoting.RandomArsonistVoting(player, __instance);
                else if (player.IsRole<Vampire>() || player.HasModifier<VampireRecruit>()) CalculatedVoting.RandomVampireVoting(player, __instance);
                else if (player.HasModifier<JackalRecruit>()) CalculatedVoting.RandomJackalRecruitVoting(player, __instance);
                else if (player.IsRole<Jackal>()) CalculatedVoting.RandomJackalVoting(player, __instance);
                else if (player.Is(Faction.Town)) CalculatedVoting.RandomTownVoting(player, __instance);
                else if (player.Is(Faction.Mafia)) CalculatedVoting.RandomMafiaVoting(player, __instance);
                else if (player.Is(Faction.Neutral)) CalculatedVoting.RandomNeutralVoting(player, __instance);
                else if (player.Is(Faction.Coven)) CalculatedVoting.RandomCovenVoting(player, __instance);
            }
        }
    }
}