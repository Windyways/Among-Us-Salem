using Il2CppInterop.Runtime.Attributes;
using System.Collections;
using System.Text;
using UnityEngine;

namespace AmongUsSalem.Roles;

public sealed class Deputy(IntPtr cppPtr) : CrewmateRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Deputy";
    public string revealText => "is a powerful force for Justice.";
    public string RoleDescription => $"Town Of Salem 2 ({OptionGroupSingleton<Deputy_Options>.Instance.Mode.ToSpacedString()})";
    public string RoleLongDescription => "You are an enforcer of the law who won't hesitate to kill in broad daylight.";
    public Color RoleColor { get; set; } = RoleColors.Town;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction Faction { get; set; } = Faction.Town;
    public Alignment Alignment => Alignment.TownKilling;

    public Attack Attack { get => GetAttack(); set { } }
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get => GetAttack(); set { } }
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public Attack GetAttack()
    {
        if (OptionGroupSingleton<Deputy_Options>.Instance.Mode == DeputyMode.ShootAndReveal) return Attack.Powerful;
        return Attack.Basic;
    }

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
            $"Attack: {Attack}\n" +
            $"Defense: {Defense}\n" +
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can Shoot players during the day to kill them, being a threat to evils in case they get information stacked against them.\n" +
            "Hang every criminal and evildoer." +
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return ShowAttributes();
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities =>
    [
        new(GetAbilityName(), GetShootDescription(), AUSAssets.Deputy_HighNoon),
    ];

    private static string ShowAttributes()
    {
        if (OptionGroupSingleton<Deputy_Options>.Instance.Mode == DeputyMode.ShootAndReveal) return $"- You cannot Shoot Day 1.";
        return
            $"- You cannot Shoot Day 1.\n" +
            $"- Illusionists can make your targets appear innocent.\n" +
            $"- Enchanters, Warlocks, Soul Collectors, & Framers can make your targets appear evil.";
    }

    private static string GetAbilityName()
    {
        if (OptionGroupSingleton<Deputy_Options>.Instance.Mode == DeputyMode.ShootAndReveal) return "Shoot";
        return "High Noon";
    }

    private static string GetShootDescription()
    {
        var opt = OptionGroupSingleton<Deputy_Options>.Instance;

        if (opt.Mode == DeputyMode.ShootAndReveal)
        {
            return
                "You can Shoot a player during the Day.\n" +
                $"You will deal a Powerful Attack to your target.\n" +
                $"If you kill a Town member, you will be lynched by the town Hangman.\n" +
                $"Everyone will know your identity.\n" +
                $"If you are evil, you will not face Hangman penalties.";
        }

        return
            "You can Shoot a player during the Day.\n" +
            "If your target has defense or is a Town member, you will miss your shot. Else, your target will be dealt a Basic Attack.\n" +
            "Only one Deputy can Shoot per Day.";
    }

    [MethodRpc((uint)AUSRpc.Deputy_HighNoon)]
    public static void RpcDeputy_HighNoon(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Deputy)
        {
            Logger<AUSPlugin>.Error("RpcDeputy_HighNoon - Invalid Deputy");
            return;
        }

        var deputy = player.GetRole<Deputy>();
        if (OptionGroupSingleton<Deputy_Options>.Instance.Mode == DeputyMode.ShootAndReveal)
        {
            deputy.Player.AddModifier<GlobalReveal>();
            if (player.CanKill(target))
            {
                AUSAssets.PlaySound(AUSAssets.Deputy_HighNoon_SFX);
                PlayerControl.LocalPlayer.Notify(Info(NotificationType.Deputy_Shoot, target, deputy.Player), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.DeputyRoleCard.LoadAsset());

                player.RpcCustomMurder(target, teleportMurderer: false);
                if (player.AmOwner()) // Hopefully fixes it?
                {
                    VisitingMechanic.RpcAddDeathReason(target, (int)DeathReasonShow.ShotByADeputy);
                }

                if (target.Is(Faction.Town))
                {
                    player.RpcCustomMurder(player, teleportMurderer: false);
                    if (player.AmOwner()) VisitingMechanic.RpcAddDeathReason(player, (int)DeathReasonShow.DishonoredTheTown);
                }

                var meetingHud = MeetingHud.Instance;
                foreach (PlayerVoteArea playerVoteArea in meetingHud.playerStates)
                {
                    if (OptionGroupSingleton<Deputy_Options>.Instance.ClearVotes)
                    {
                        playerVoteArea.UnsetVote();
                        meetingHud.ClearVote();
                    }
                }
            }
            return;
        }

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
            if ((!target.Is(Faction.Town) || target.HasModifier<FramedModifier>() || target.HasModifier<WarlockFramedModifier>()) && !target.HasModifier<IllusionedModifier>())
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

    public static string Info(NotificationType type, PlayerControl target, PlayerControl deputy = null)
    {
        if (type == NotificationType.Deputy_Shoot)
        {
            if (OptionGroupSingleton<Deputy_Options>.Instance.Mode == DeputyMode.ShootAndReveal) return deputy.Name() + " decided to fire their Revolver!";
            return "A Deputy decided to fire their Revolver!";
        }
        return $"You missed your shot! This could be because {target.Name()} has Defense or is immune.";
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        var opt = OptionGroupSingleton<Deputy_Options>.Instance;
        if (opt.Mode == DeputyMode.ShootAndReveal) AttackDefenseMechanic.RpcApplyAttack(Player, Attack.Powerful, true, true);
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

    public void Role_OnMeetingStart()
    {
        if (Player.HasDied())
            return;

        SmartDeputy.Start();
        if (Player.AmOwner) Coroutines.Start(GenButtons());
    }

    public IEnumerator GenButtons(float delay = 3f)
    {
        yield return new WaitForSeconds(delay);
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
    public int Charges = OptionGroupSingleton<Deputy_Options>.Instance.Mode == DeputyMode.ShootAndReveal ?
        (int)OptionGroupSingleton<Deputy_Options>.Instance.Charges :
        (int)OptionGroupSingleton<Deputy_Options>.Instance.Charges_SAR;
}

public sealed class Deputy_Options : AbstractOptionGroup<Deputy>
{
    public override string GroupName => "Deputy";

    [ModdedEnumOption("Deputy Mode", typeof(DeputyMode), ["Shoot & Reveal", "High Noon"])]
    public DeputyMode Mode { get; set; } = DeputyMode.HighNoon;

    // --- SHOOT AND REVEAL ---
    public ModdedNumberOption Charges_SAR { get; } = new("Deputy Max Shots", 1f, 0f, 15f, 1f, MiraNumberSuffixes.None, zeroInfinity: true)
    { Visible = () => OptionGroupSingleton<Deputy_Options>.Instance.Mode == DeputyMode.ShootAndReveal };

    public ModdedToggleOption ClearVotes { get; } = new("Deputy Clears Votes After Shooting", true)
    { Visible = () => OptionGroupSingleton<Deputy_Options>.Instance.Mode == DeputyMode.ShootAndReveal };

    // --- HIGH NOON ---
    public ModdedNumberOption Charges { get; } = new("Deputy Max High Noons", 1f, 0f, 15f, 1f, MiraNumberSuffixes.None, zeroInfinity: true)
    { Visible = () => OptionGroupSingleton<Deputy_Options>.Instance.Mode == DeputyMode.HighNoon };
}

public enum DeputyMode
{
    ShootAndReveal,
    HighNoon
}