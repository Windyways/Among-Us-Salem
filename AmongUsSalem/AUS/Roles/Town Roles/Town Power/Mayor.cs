using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using Color = UnityEngine.Color;
using UnityEngine;

namespace AmongUsSalem.Roles;

#region Mayor
#endregion
public sealed class Mayor(IntPtr cppPtr) 
    : CrewmateRole(cppPtr), IAUSRole, IRevealable, IWikiDiscoverable, IContinueGame
{
    public bool continueGame => true;
    public string RoleName { get; set; } = TouLocale.Get(TouNames.Mayor, "Mayor");
    public string revealText => "is the leader of the town.";
    public string RoleDescription => "Placeholder.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction RoleFaction { get; set; } = Faction.Town;
    public Color RoleColor { get; set; } = AUSColors.Town;
    public Alignment Alignment => Alignment.TownPower;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;
    
    public DeathReasonShow deathReasonShow { get; set; } = DeathReasonShow.Alive;
    public bool IsRevealed { get; set; }

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.MayorRoleCard,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IAUSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#06E00C>Mayor</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#06E00C>Town</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#06E00C>Town</color> <color=#1e45d4>Power</color>" +
            "\n<color=#fdbc00>Goal:</color> Hang every criminal and evildoer." +
            $"\n\nAttributes:" +
            "\nYou cannot reveal on day one." +
            "\nYou will stop gaining additional votes during a TT Hunt." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Reveal",
            "Once you have revealed youtself as Mayor, you will gain an extra vote." +
            "\nEach day after you reveal, you will permanently gain an extra vote." +
            "\nYou cannot reveal if you are on trial.",
            AUSAssets.Mayor_Reveal)
    ];

    [MethodRpc((uint)AUSRpc.Mayor_Reveal, SendImmediately = true)]
    public static void RpcMayor_Reveal(PlayerControl player)
    {
        if (player.Data.Role is not Mayor)
        {
            Logger<AUSPlugin>.Error("RpcMayor_Reveal - Invalid Conjurer");
            return;
        }

        var mayor = player.GetRole<Mayor>();
        mayor.IsRevealed = true;
        mayor.Votes++;
        
        AUSAssets.PlaySound(AUSAssets.Mayor_Reveal_SFX, 2);
        
        MiscUtils.ShowNotification(Info(player), Color.white, AUSAssets.MayorRoleCard.LoadAsset());
        MiscUtils.AddFakeChat(player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Town, "Mayor Info"), Info(player));
    }

    public static string Info(PlayerControl player)
    {
        return player.GetDefaultAppearance().PlayerName + " has Revealed themself as The <b><color=#06E00C>Mayor</color></b>!";
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

        if (IsRevealed) Votes++;
        if (Player.AmOwner)
        {
            Coroutines.Start(meetingMenu.GenButtonsDelay(MeetingHud.Instance, Player.AmOwner && !Player.HasDied() && !IsRevealed && DayNightMechanic.DayCount >= 2));
        }
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
    [RegisterEvent]
    public static void HandleVoteEvent(HandleVoteEvent @event)
    {
        if (@event.VoteData.Owner.Data.Role is not Mayor mayor || !mayor.IsRevealed)
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