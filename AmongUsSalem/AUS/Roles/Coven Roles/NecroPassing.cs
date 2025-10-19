using System.Collections;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AmongUsSalem.LifeImprovement.Roles;

#region NecroPassing
#endregion
public sealed class NecroPassing : BaseModifier
{
    public override string ModifierName => "Necro Passing";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public MeetingMenu meetingMenu;
    public List<byte> VotesToGetBook = new List<byte>();

    public override void OnActivate()
    {
        base.OnActivate();

        if (Player.AmOwner())
        {
            meetingMenu = new MeetingMenu(
                Player.Data.Role,
                ClickGuess,
                MeetingAbilityType.Click,
                AUSAssets.NecroPassing_PassNecronomicon,
                null!,
                IsExempt)
            {
                Position = new Vector3(1f, 0f, -3f) // originally -0.40
            };
        }
    }

    public override void OnMeetingStart()
    {
        AUSPlugin.DebugLogMessage("Necro Passing OnMeetingStart called!");
        SmartNecroPassing.Start(this);

        if (Player.AmOwner)
        {
            Coroutines.Start(GenButtons());
        }
    }

    public IEnumerator GenButtons()
    {
        yield return new WaitForSeconds(3f);
        meetingMenu.GenButtons(MeetingHud.Instance, Player.AmOwner && !Player.HasDied());
    }

    public override void OnDeactivate()
    {
        if (Player.AmOwner)
        {
            meetingMenu?.Dispose();
            meetingMenu = null!;
        }
    }

    public void ClickGuess(PlayerVoteArea voteArea, MeetingHud __)
    {
        var target = GameData.Instance.GetPlayerById(voteArea.TargetPlayerId).Object;
        if (target.HasModifier<NecroPassing>())
        {
            var necroPassing = target.GetModifier<NecroPassing>();
            if (!necroPassing.VotesToGetBook.Contains(Player.PlayerId))
            {
                RpcNecroPassing_PassNecronomicon(Player, target);
            }
        }
    }

    public bool IsExempt(PlayerVoteArea voteArea)
    {
        return Player.Data.IsDead || voteArea!.AmDead ||
               !(MiscUtils.PlayerById(voteArea.TargetPlayerId)?.Data.Role is ICustomAURole ausRole && Player.Data.Role is ICustomAURole ausRole2 && ausRole.Faction == ausRole2.Faction);
    }

    [MethodRpc((uint)AUSRpc.NecroPassing_PassNecronomicon, SendImmediately = true)]
    public static void RpcNecroPassing_PassNecronomicon(PlayerControl player, PlayerControl target)
    {
        if (!player.HasModifier<NecroPassing>())
        {
            Logger<AUSPlugin>.Error("RpcNecroPassing_PassNecronomicon - Invalid Necro Passing");
            return;
        }

        int votes = 0;
        foreach (var necroPassing in ModifierUtils.GetActiveModifiers<NecroPassing>())
        {
            if (necroPassing.VotesToGetBook.Contains(player.PlayerId))
            {
                necroPassing.VotesToGetBook.Remove(player.PlayerId);
            }
        }

        if (target.HasModifier<NecroPassing>())
        {
            var necroPassing = target.GetModifier<NecroPassing>();
            if (!necroPassing.VotesToGetBook.Contains(player.PlayerId))
            {
                necroPassing.VotesToGetBook.Add(player.PlayerId);
            }
            votes = necroPassing.VotesToGetBook.Count;
        }

        if (PlayerControl.LocalPlayer.Is(Faction.Coven))
        {
            MiscUtils.ShowNotification(Info(player, target, votes), Color.white, AUSAssets.Necronomicon.LoadAsset());
            MiscUtils.AddFakeChat(target.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Coven, "Necronomicon Info"), Info(player, target, votes));
        }
    }

    [MethodRpc((uint)AUSRpc.NecroPassing_AssignNecronomicon, SendImmediately = true)]
    public static void RpcNecroPassing_AssignNecronomicon()
    {
        var necroPassings = PlayerControl.AllPlayerControls.ToArray()
            .Where(x => !x.HasDied())
            .Select(x => x.GetModifier<NecroPassing>())
            .Where(necroPassing => necroPassing != null && necroPassing.VotesToGetBook.Count > 0)
            .ToList();

        var nextHolder = necroPassings.OrderBy(r => r.VotesToGetBook.Count).FirstOrDefault();
        if (nextHolder != null && nextHolder.Player.Data.Role is ICovenRole covenRole)
        {
            CovenNecronomiconMechanic.ClearNecronomicon();

            covenRole.Necronomicon = true;
            if (nextHolder.Player.Data.Role is ICustomAURole ausRole)
            {
                ausRole.Attack = Attack.Basic;
                ausRole.ogAttack = Attack.Basic;
                CovenNecronomiconMechanic.AdjustButtons(nextHolder.Player);
            }

            if (PlayerControl.LocalPlayer.Is(Faction.Coven) || nextHolder.Player.AmOwner())
            {
                MiscUtils.ShowNotification(CovenNecronomiconMechanic.Info(nextHolder.Player), Color.white, AUSAssets.Necronomicon.LoadAsset());
                MiscUtils.AddFakeChat(nextHolder.Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Coven, "Coven Info"), CovenNecronomiconMechanic.Info(nextHolder.Player));
            }
        }
    }

    public void OnVotingComplete()
    {
        if (Player.AmOwner)
        {
            meetingMenu?.Dispose();
        }
    }

    public static string Info(PlayerControl passingPlayer, PlayerControl target, int votes)
    {
        return passingPlayer.GetDefaultAppearance().PlayerName + " voted to pass the Necronomicon to " + target.GetDefaultAppearance().PlayerName + $" ({votes}).";
    }

    public static void OnRoundStart()
    {
        RpcNecroPassing_AssignNecronomicon();
    }

    public void ClearPassings()
    {
        VotesToGetBook.Clear();
    }

    public override void OnDeath(DeathReason reason)
    {
        foreach (var necroPassing in ModifierUtils.GetActiveModifiers<NecroPassing>())
        {
            if (necroPassing.VotesToGetBook.Contains(Player.PlayerId))
            {
                necroPassing.VotesToGetBook.Remove(Player.PlayerId);
            }
        }
    }
}

public static class NecroPassing_Events
{
    [RegisterEvent]
    public static void VotingCompleteHandler(VotingCompleteEvent @event)
    {
        ModifierUtils.GetActiveModifiers<NecroPassing>().Do(x => x.OnVotingComplete());
    }

    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            return; // Only run when game starts.
        }

        NecroPassing.OnRoundStart();
        ModifierUtils.GetActiveModifiers<NecroPassing>().Do(x => x.ClearPassings());
    }
}