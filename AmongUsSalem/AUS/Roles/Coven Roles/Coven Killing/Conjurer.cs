using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement.Roles;

#region Conjurer
#endregion
public sealed class Conjurer(IntPtr cppPtr)
    : CovenRole(cppPtr), IWikiDiscoverable, ICustomAURole, ICovenRole
{
    public string RoleName { get; set; } = "Conjurer";
    public string revealText => "is able to pull items out of thin air.";
    public string RoleDescription => "Perform a murder at day.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.Coven;
    public Color RoleColor { get; set; } = AUSColors.Coven;
    public Alignment Alignment => Alignment.CovenKilling;
    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public NecronomiconPriority NecronomiconPriority => NecronomiconPriority.Conjurer;
    public bool Necronomicon { get; set; }

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.ConjurerRoleCard,
        CanUseSabotage = OptionGroupSingleton<CovenOptions>.Instance.CanSabotage,
        CanUseVent = OptionGroupSingleton<CovenOptions>.Instance.CanVent,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ICustomAURole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#B545FF>Conjurer</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#B545FF>Coven</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#B545FF>Coven</color> <color=#1e45d4>Killing</color>" +
            "\n<color=#fdbc00>Goal:</color> Kill all who would oppose the Coven." +
            $"\n\nAttributes:" +
            "\nYou cannot Conjure Day 1." +
            "\nWith the Necronomicon, you will also deal a Basic Attack to your target." +
            "\nYou will obtain the Necronomicon 2nd, after the <color=#B545FF>Coven Leader</color>." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Conjure",
            "You can Conjure a meteor upon a player during the Day.\n\n" +
            "You will deal a Powerful Attack to your target.",
            AUSAssets.Conjurer_Conjure)
    ];

    public bool WinConditionMet()
    {
        var aliveCoven = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.Is(Faction.Coven));
        if (aliveCoven == 0) return false;

        var result = MiscUtils.GetAlivePlayersToEnd().Count <= aliveCoven && MiscUtils.KillersAliveCount() == aliveCoven;
        return result || AmongUsSalem.Patches.LogicGameFlowPatches.EndGameEarlyCheck(this);
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet();
    }

    [MethodRpc((uint)AUSRpc.Conjurer_Conjure, SendImmediately = true)]
    public static void RpcConjurer_Conjure(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Conjurer)
        {
            Logger<AUSPlugin>.Error("RpcConjurer_Conjure - Invalid Conjurer");
            return;
        }

        if (target.HasDied())
        {
            MiscUtils.ShowNotification(ConjureInfo(target), Color.white, AUSAssets.ConjurerRoleCard.LoadAsset());
            MiscUtils.AddFakeChat(target.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Coven, "Conjurer Info"), ConjureInfo(target));
        }
        else
        {
            MiscUtils.ShowNotification(TMDInfo(target), Color.white, AUSAssets.ConjurerRoleCard.LoadAsset());
            MiscUtils.AddFakeChat(target.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Coven, "Conjurer Info"), TMDInfo(target));
        }

        var meetingHud = MeetingHud.Instance;
        foreach (PlayerVoteArea playerVoteArea in meetingHud.playerStates)
        {
            if (OptionGroupSingleton<Conjurer_Options>.Instance.ClearVotes)
            {
                playerVoteArea.UnsetVote();
                meetingHud.ClearVote();
            }
        }
    }

    public static string ConjureInfo(PlayerControl target)
    {
        return target.GetDefaultAppearance().PlayerName + " faces an untimely end!";
    }

    public static string TMDInfo(PlayerControl target)
    {
        return target.GetDefaultAppearance().PlayerName + " has defended the attack!";
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        base.Initialize(player);

        if (Player.AmOwner())
        {
            meetingMenu = new MeetingMenu(
                this,
                ClickGuess,
                MeetingAbilityType.Click,
                AUSAssets.Conjurer_Conjure,
                null!,
                IsExempt)
            {
                Position = new Vector3(-0.40f, 0f, -3f)
            };
        }
    }

    public override void OnMeetingStart()
    {
        AUSPlugin.DebugLogMessage("Conjurer OnMeetingStart called!");
        SmartConjurer.Start();

        if (Player.AmOwner)
        {
            Coroutines.Start(meetingMenu.GenButtonsDelay(MeetingHud.Instance, Player.AmOwner && !Player.HasDied() && Charges > 0 && DayNightMechanic.DayCount >= 2));
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
        var target = GameData.Instance.GetPlayerById(voteArea.TargetPlayerId).Object;

        if (Player.CanKill(target, Attack.Powerful))
        {
            MiscUtils.RpcApplyDeathReason(Player, target, DeathReasonShow.KilledByAConjurer, false, false);
            RpcConjurer_Conjure(Player, target);
            Charges--;
        }

        if (Player.AmOwner)
        {
            meetingMenu?.HideButtons();
        }
    }

    public bool IsExempt(PlayerVoteArea voteArea)
    {
        return voteArea?.TargetPlayerId == Player.PlayerId || Player.Data.IsDead || voteArea!.AmDead ||
               voteArea.GetPlayer().Is(Faction.Coven);
    }

    private MeetingMenu meetingMenu;
    public int Charges = (int)OptionGroupSingleton<Conjurer_Options>.Instance.Charges;

    public PlayerControl indocrinatedPlayer;
}

#region Conjurer_Attack
#endregion
public sealed class Conjurer_Attack : AmongUsSalemRoleButton<Conjurer, PlayerControl>
{
    public override string Name => "Attack";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Coven;
    public override float Cooldown => OptionGroupSingleton<CovenOptions>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.NecronomiconButton;

    public override void ClickHandler()
    {
        if (Target != null && Timer <= 0)
        {
            if (MiscUtils.SuccessfulVisit(Player, Target, true, true))
            {
                base.ClickHandler();
            }
        }
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        if (Player.CanKill(Target)) MiscUtils.RpcApplyDeathReason(Player, Target, DeathReasonShow.KilledByTheCoven);
        else
        {
            MiscUtils.ShowNotification(MessageTexts.TooMuchDefense(Player, Target), Color.white);
            MiscUtils.AddFakeChat(Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Neutral, "General Info"), MessageTexts.TooMuchDefense(Player, Target));
        }
        MiscUtils.PostSuccessfulVisit(Player, Target, true, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, predicate: x =>
            (x.Data.Role is ICustomAURole customRole && customRole.Faction != Role.Faction));
    }

    public override bool CanUse()
    {
        return base.CanUse() && Player.Data.Role is ICovenRole coven && coven.Necronomicon;
    }
}

#region Conjurer_Options
#endregion
public sealed class Conjurer_Options : AbstractOptionGroup<Conjurer>
{
    public override string GroupName => "Conjurer";

    [ModdedNumberOption("<color=#B545FF>Conjurer</color> Max <color=#4a86e8>Conjures</color>", 1f, 15f, 1f)]
    public float Charges { get; set; } = 1f;

    [ModdedToggleOption("<color=#B545FF>Conjurer</color> Clears Votes After <color=#4a86e8>Conjure</color>")]
    public bool ClearVotes { get; set; } = true;
}