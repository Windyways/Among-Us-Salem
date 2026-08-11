using System.Collections;
using MiraAPI.Events.Vanilla.Meeting.Voting;
using UnityEngine;

namespace AmongUsSalem.CovenRoles;

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
        //SmartNecroPassing.Start(this);

        if (Player.AmOwner)
        {
            Coroutines.Start(GenButtons());
        }
    }

    public IEnumerator GenButtons(float delay = 3f)
    {
        yield return new WaitForSeconds(delay);
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

    [MethodRpc((uint)AUSRpc.NecroPassing_PassNecronomicon)]
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
            PlayerControl.LocalPlayer
                .Notify(Info(player, target, votes), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.NecronomiconIcon.LoadAsset(), basePlayer: target.CachedPlayerData);
        }
    }

    [MethodRpc((uint)AUSRpc.NecroPassing_AssignNecronomicon)]
    public static void RpcNecroPassing_AssignNecronomicon()
    {
        var necroPassings = PlayerControl.AllPlayerControls.ToArray()
            .Where(x => !x.HasDied())
            .Select(x => x.GetModifier<NecroPassing>())
            .Where(necroPassing => necroPassing != null && necroPassing.VotesToGetBook.Count > 0)
            .ToList();

        var nextHolder = necroPassings.OrderBy(r => r.VotesToGetBook.Count).FirstOrDefault();
        if (nextHolder != null) nextHolder.Player.AddModifier<Necronomicon>();

        ModifierUtils.GetActiveModifiers<NecroPassing>().Do(x => x.ClearPassings());
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
        return passingPlayer.Name() + " voted to pass the Necronomicon to " + target.Name() + $" ({votes}).";
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
    public static void VotingCompleteEvent(VotingCompleteEvent @event)
    {
        NecroPassing.RpcNecroPassing_AssignNecronomicon();
        ModifierUtils.GetActiveModifiers<NecroPassing>().Do(x => x.OnVotingComplete());
    }
}