using System.Collections;
using MiraAPI.Modifiers;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using UnityEngine;

namespace TownOfUs.Modifiers;

public sealed class DeathHandlerModifier : BaseModifier
{
    public override string ModifierName => "Death Handler";
    public override bool HideOnUi => true;
    public override bool ShowInFreeplay => false;
    // This will determine if another mira event should be able to modify the information
    public bool LockInfo { get; set; }
    // This will determine if symbols or anything are shown
    public bool DiedThisRound { get; set; } = true;
    // This will specify how the player died.
    public DeathReasonShow CauseOfDeath { get; set; } = DeathReasonShow.Alive;
    public DeathReasonShow SecondaryCauseOfDeath { get; set; } = DeathReasonShow.Alive;
    public DeathReasonShow ThirdCauseOfDeath { get; set; } = DeathReasonShow.Alive;
    // This will specify who killed the player, if any, such as; By Innersloth
    public string KilledBy { get; set; } = string.Empty;
    // This will specify who killed the player.
    public PlayerControl KillerPlayer { get; set; }

    public Color DeathColor { get; set; } = RoleColors.Town;
    public Color SecondaryDeathColor { get; set; } = RoleColors.Town;
    public Color ThirdDeathColor { get; set; } = RoleColors.Town;

    [MethodRpc((uint)AUSRpc.UpdateDeathHandler, SendImmediately = true)]
    public static void RpcUpdateDeathHandler(PlayerControl player, DeathReasonShow causeOfDeath = DeathReasonShow.None, DeathHandlerOverride diedThisRound = DeathHandlerOverride.Ignore, string killedBy = "null", DeathHandlerOverride lockInfo = DeathHandlerOverride.Ignore)
    {
        UpdateDeathHandler(player, causeOfDeath, diedThisRound, killedBy, lockInfo);
    }

    public static void UpdateDeathHandler(PlayerControl player, DeathReasonShow causeOfDeath = DeathReasonShow.None, DeathHandlerOverride diedThisRound = DeathHandlerOverride.Ignore, string killedBy = "null", DeathHandlerOverride lockInfo = DeathHandlerOverride.Ignore)
    {
        if (!player.HasModifier<DeathHandlerModifier>())
        {
            Logger<AUSPlugin>.Error("RpcUpdateDeathHandler - Player had no DeathHandlerModifier");
            player.AddModifier<DeathHandlerModifier>();
        }

        Coroutines.Start(CoWriteDeathHandler(player, causeOfDeath, diedThisRound, killedBy, lockInfo, GetColor(causeOfDeath)));
    }

    public static Color GetColor(DeathReasonShow causeOfDeath)
    {
        if (causeOfDeath == DeathReasonShow.LeftTown) return RoleColors.Auditor;

        if (causeOfDeath == DeathReasonShow.KilledByAMemberOfTheMafia || causeOfDeath == DeathReasonShow.KilledByAnAmbusher) return RoleColors.Mafia;
        //if (causeOfDeath == DeathReasonShow.IncineratedByAnArsonist) return AUSColors.Arsonist;
        //if (causeOfDeath == DeathReasonShow.KilledByAShroud) return AUSColors.Shroud;
        //if (causeOfDeath == DeathReasonShow.BittenByAVampire) return AUSColors.Vampire;
        //if (causeOfDeath == DeathReasonShow.AssassinatedByAJackal || causeOfDeath == DeathReasonShow.ARecruitOfTheJackalAndHaveFailedTheirTeammate) return AUSColors.Neutral;
        if (causeOfDeath == DeathReasonShow.ShotByAVeteran || causeOfDeath == DeathReasonShow.ShotByAVigilante || causeOfDeath == DeathReasonShow.KilledByABodyguard || causeOfDeath == DeathReasonShow.DiedWhileDefendingTheirTarget || causeOfDeath == DeathReasonShow.ShotByADeputy || causeOfDeath == DeathReasonShow.KilledByACrusader || causeOfDeath == DeathReasonShow.DishonoredTheTown) return RoleColors.Town;
        if (causeOfDeath == DeathReasonShow.KilledByTheCoven || causeOfDeath == DeathReasonShow.DisintegratedByAHexMaster || causeOfDeath == DeathReasonShow.KilledByAJinx || causeOfDeath == DeathReasonShow.KilledByARitualist) return RoleColors.Coven;
        if (causeOfDeath == DeathReasonShow.HauntedByAJester || causeOfDeath == DeathReasonShow.DiedOfBoredom) return RoleColors.Jester;
        if (causeOfDeath == DeathReasonShow.StabbedByASerialKiller) return RoleColors.SerialKiller();
        if (causeOfDeath == DeathReasonShow.MauledByAWerewolf) return RoleColors.Werewolf;
        return RoleColors.Apocalypse;
    }

    public static bool IsCoroutineRunning { get; set; }
    public static IEnumerator CoWriteDeathHandler(PlayerControl player, DeathReasonShow causeOfDeath, DeathHandlerOverride diedThisRound, string killedBy, DeathHandlerOverride lockInfo, Color col)
    {
        IsCoroutineRunning = true;
        yield return new WaitForSeconds(0.1f);
        var deathHandler = player.GetModifier<DeathHandlerModifier>()!;
        if (causeOfDeath != DeathReasonShow.None)
        {
            if (deathHandler.CauseOfDeath == DeathReasonShow.Alive) deathHandler.CauseOfDeath = causeOfDeath;
            else if (deathHandler.SecondaryCauseOfDeath == DeathReasonShow.Alive) deathHandler.SecondaryCauseOfDeath = causeOfDeath;
            else if (deathHandler.ThirdCauseOfDeath == DeathReasonShow.Alive) deathHandler.ThirdCauseOfDeath = causeOfDeath;
        }
        if (diedThisRound != DeathHandlerOverride.Ignore) deathHandler.DiedThisRound = diedThisRound is DeathHandlerOverride.SetTrue;
        if (killedBy != "null") deathHandler.KilledBy = killedBy;
        if (lockInfo != DeathHandlerOverride.Ignore) deathHandler.LockInfo = lockInfo is DeathHandlerOverride.SetTrue;

        if (deathHandler.DeathColor == RoleColors.Town) deathHandler.DeathColor = col;
        else if (deathHandler.SecondaryDeathColor == RoleColors.Town) deathHandler.SecondaryDeathColor = col;
        else if (deathHandler.ThirdDeathColor == RoleColors.Town) deathHandler.ThirdDeathColor = col;

        IsCoroutineRunning = false;
    }

    public static bool IsFullyDead(PlayerControl player)
    {
        if (!player.HasDied())
        {
            return false;
        }

        if (player.TryGetModifier<DeathHandlerModifier>(out var deathHandler))
        {
            return !deathHandler.DiedThisRound;
        }

        return false;
    }
}

public enum DeathHandlerOverride
{
    SetTrue,
    SetFalse,
    Ignore
}