using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events.Vanilla.Meeting.Voting;
using System.Collections;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace AmongUsSalem.Roles;

public sealed class Deputy(IntPtr cppPtr) : CrewmateRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Deputy";
    public string revealText => "is a powerful force for Justice.";
    public string RoleDescription => "";
    public string RoleLongDescription => "";
    public Color RoleColor { get; set; } = RoleColors.Town;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction Faction { get; set; } = Faction.Town;
    public Alignment Alignment => Alignment.TownKilling;

    public Attack Attack { get; set; } = Attack.Basic;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.Basic;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.DeputyRoleCard
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ICustomAURole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can Shoot players during the day to kill them, being a threat to evils in case they get information stacked against them.\n" +
            "Hang every criminal and evildoer." +
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return
            $"- You cannot Shoot Day 1.\n" +
            $"- Illusionists can make your targets appear innocent.\n" +
            $"- Enchanters & Framers can make your targets appear evil.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("High Noon",
            "You can Shoot a player during the Day.\n" +
            "If your target has defense or is a Town member, you will miss your shot. Else, your target will be dealt a Basic Attack.\n" +
            "Only one Deputy can Shoot per Day.",
            AUSAssets.Deputy_HighNoon),
    ];

    [MethodRpc((uint)AUSRpc.Deputy_HighNoon)]
    public static void RpcDeputy_HighNoon(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Deputy)
        {
            Logger<AUSPlugin>.Error("RpcDeputy_HighNoon - Invalid Deputy");
            return;
        }

        var deputy = player.GetRole<Deputy>();

        // Prevent other Deputies from shooting today.
        foreach (var player2 in PlayerControl.AllPlayerControls)
        {
            if (player2.Data.Role is Deputy deputy2)
            {
                if (player2.AmOwner)
                {
                    deputy2.meetingMenu?.HideButtons();
                }
            }
        }

        if (player.CanKill(target))
        {
            if ((!target.Is(Faction.Town) || target.HasModifier<FramedModifier>()) && !target.HasModifier<IllusionedModifier>())
            {
                deputy.Player.AddModifier<Confirmed>();
                AUSAssets.PlaySound(AUSAssets.Deputy_HighNoon_SFX);

                PlayerControl.LocalPlayer.Notify(Info(NotificationType.Deputy_Shoot, target), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.DeputyRoleCard.LoadAsset());

                player.RpcCustomMurder(target, teleportMurderer: false);
                if (player.AmOwner()) // Hopefully fixes it?
                {
                    VisitingMechanic.RpcAddDeathReason(target, (int)DeathReasonShow.ShotByADeputy);
                }
                return;
            }
        }

        deputy.Player.Notify(Info(NotificationType.Deputy_MissedShot, target), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.DeputyRoleCard.LoadAsset());
    }

    public static string Info(NotificationType type, PlayerControl target)
    {
        if (type == NotificationType.Deputy_Shoot) return "A Deputy decided to fire their Revolver!";
        return $"You missed your shot! This could be because {target.Name()} has Defense or is immune.";
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
                AUSAssets.Deputy_HighNoon,
                null!,
                IsExempt)
            {
                Position = new Vector3(-0.40f, 0f, -3f)
            };
        }
    }

    public override void OnMeetingStart()
    {
        AUSPlugin.DebugLogMessage("Deputy OnMeetingStart called!");
        SmartDeputy.Start();

        if (Player.AmOwner)
        {
            Coroutines.Start(GenButtons());
        }
    }

    public IEnumerator GenButtons()
    {
        yield return new WaitForSeconds(3f);
        meetingMenu.GenButtons(MeetingHud.Instance, Player.AmOwner && !Player.HasDied() && Charges > 0 && DayNightMechanic.DayCount >= 2);
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
        var target = GameData.Instance.GetPlayerById(voteArea.TargetPlayerId).Object;

        Charges--;
        RpcDeputy_HighNoon(Player, target);

        if (Player.AmOwner)
        {
            meetingMenu?.HideButtons();
        }
    }

    public bool IsExempt(PlayerVoteArea voteArea)
    {
        return voteArea?.TargetPlayerId == Player.PlayerId || Player.Data.IsDead || voteArea!.AmDead;
    }

    public MeetingMenu meetingMenu;
    public int Charges = (int)OptionGroupSingleton<Deputy_Options>.Instance.Charges;
}

public sealed class Deputy_Options : AbstractOptionGroup<Deputy>
{
    public override string GroupName => "Deputy";

    [ModdedNumberOption("Deputy Max High Noons", 0f, 30f, 1f, MiraNumberSuffixes.None, zeroInfinity: true)]
    public float Charges { get; set; } = 1;
}