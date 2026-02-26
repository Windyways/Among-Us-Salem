using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events.Vanilla.Meeting.Voting;
using System.Collections;
using System.Text;
using TownOfUs.Utilities.Appearances;
using UnityEngine;

namespace AmongUsSalem.Roles;

public sealed class Mayor(IntPtr cppPtr) : CrewmateRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Mayor";
    public string revealText => "is the leader of the town.";
    public string RoleDescription => "";
    public string RoleLongDescription => "You are the leader of the town.";
    public Color RoleColor { get; set; } = RoleColors.Town;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction Faction { get; set; } = Faction.Town;
    public Alignment Alignment => Alignment.TownGovernment;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.MayorRoleCard
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ICustomAURole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"Attack: {Attack}\n" +
            $"Defense: {Defense}\n" +
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can Reveal themself to gain a substantial amount of votes, helping Town getting majority a lot easier, as well as being a huge late-game threat to evils.\n" +
            "Hang every criminal and evildoer." +
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return
            $"- You cannot Reveal Day 1.\n" +
            $"- You will stop gaining additional votes during a TT Hunt.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Reveal",
            "You can Reveal during the Day.\n" +
            "Your role will be revealed to all, and you will gain +1 vote. You will gain +1 vote at the start of each Day.",
            AUSAssets.Mayor_Reveal),
    ];

    [MethodRpc((uint)AUSRpc.Mayor_Reveal)]
    public static void RpcMayor_Reveal(PlayerControl player)
    {
        if (player.Data.Role is not Mayor)
        {
            Logger<AUSPlugin>.Error("RpcMayor_Reveal - Invalid Mayor");
            return;
        }

        var mayor = player.GetRole<Mayor>();
        mayor.Player.AddModifier<GlobalReveal>();
        mayor.Votes++;

        AUSAssets.PlaySound(AUSAssets.Mayor_Reveal_SFX);

        mayor.Player.Notify(Info(NotificationType.Mayor_TownGainingConfidence, mayor.Player), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.MayorRoleCard.LoadAsset());
        PlayerControl.LocalPlayer.Notify(Info(NotificationType.Mayor_Reveal, mayor.Player), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.MayorRoleCard.LoadAsset());

        player.RemoveModifier<IncriminatingEvidence>();
        player.RemoveModifier<SeenKill>();
    }

    public static string Info(NotificationType type, PlayerControl player)
    {
        if (type == NotificationType.Mayor_TownGainingConfidence) return "The Town's confidence in you is growing... you've gained another vote!";
        return player.Name() + " has Revealed themself as The Mayor!";
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        if (Player.AmOwner())
        {
            meetingMenu = new MeetingMenu(
                this,
                ClickGuess,
                MeetingAbilityType.Click,
                AUSAssets.Mayor_Reveal,
                null!,
                IsExempt)
            {
                Position = new Vector3(-0.40f, 0f, -3f)
            };
        }
    }

    public override void OnMeetingStart()
    {
        AUSPlugin.DebugLogMessage("Mayor OnMeetingStart called!");
        SmartMayor.Start();

        if (Player.HasModifier<GlobalReveal>())
        {
            Votes++;
            Player.Notify(Info(NotificationType.Mayor_TownGainingConfidence, Player), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.MayorRoleCard.LoadAsset());
        }

        if (Player.AmOwner)
        {
            Coroutines.Start(GenButtons());
        }
    }

    public IEnumerator GenButtons(float delay = 3f)
    {
        yield return new WaitForSeconds(delay);
        meetingMenu.GenButtons(MeetingHud.Instance, Player.AmOwner && !Player.HasDied() && !Player.HasModifier<GlobalReveal>() && DayNightMechanic.DayCount >= 2);
    }

    public override void OnVotingComplete()
    {
        RoleBehaviourStubs.OnVotingComplete(this);

        if (Player.AmOwner)
        {
            meetingMenu.HideButtons();
        }
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);

        if (Player.AmOwner)
        {
            meetingMenu?.Dispose();
            meetingMenu = null!;
        }
    }

    public void ClickGuess(PlayerVoteArea voteArea, MeetingHud __)
    {
        RpcMayor_Reveal(Player);
        /*foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (!player.HasDied()) player.RpcAddModifier<JuryModifier>(Player);
        }*/

        if (Player.AmOwner)
        {
            meetingMenu?.HideButtons();
        }
    }

    public bool IsExempt(PlayerVoteArea voteArea)
    {
        return voteArea?.TargetPlayerId != Player.PlayerId || Player.Data.IsDead || voteArea!.AmDead;
    }

    public MeetingMenu meetingMenu;
    public int Votes = 1;
}

public static class Mayor_Events
{
    [RegisterEvent()]
    public static void HandleVoteEvent(HandleVoteEvent @event)
    {
        if (@event.VoteData.Owner.Data.Role is not Mayor mayor || !mayor.Player.HasModifier<GlobalReveal>()/* || mayor.Player.IsSilenced()*/)
        {
            return;
        }

        @event.VoteData.SetRemainingVotes(0);

        for (var i = 0; i < mayor.Votes; i++)
        {
            @event.VoteData.VoteForPlayer(@event.TargetId);
        }

        @event.Cancel();
    }
}