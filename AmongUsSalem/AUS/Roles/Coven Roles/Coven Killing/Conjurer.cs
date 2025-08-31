using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement.Roles;

#region Conjurer
#endregion
public sealed class Conjurer(IntPtr cppPtr)
    : NeutralRole(cppPtr), IWikiDiscoverable, IAUSRole, ICovenRole
{
    public string RoleName => TouLocale.Get(TouNames.Conjurer, "Conjurer");
    public string revealText => "is able to pull items out of thin air.";
    public string RoleDescription => "Placeholder.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction RoleFaction => Faction.Coven;
    public Color RoleColor => AUSColors.Coven;
    public Alignment Alignment => Alignment.CovenKilling;
    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public DeathReasonShow deathReasonShow { get; set; } = DeathReasonShow.Alive;

    public NecronomiconPriority NecronomiconPriority => NecronomiconPriority.Conjurer;
    public bool Necronomicon { get; set; }

    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = false,
        Icon = AUSAssets.ConjurerRoleCard,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IAUSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#ab42ef>Conjurer</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color> <color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#ab42ef>Coven</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#ab42ef>Coven</color> <color=#1e45d4>Killing</color>" +
            "\n<color=#fdbc00>Goal:</color> Kill all who would oppose the Coven." +
            $"\n\nAttributes:" +
            "\nYou can not conjure a meteor Day One." +
            "\nWith the Necronomicon you may Basic Attack someone at Night." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Conjure",
            "Conjure a meteor to attack a player during the day." +
            "\n\nThe meteor will deal a Powerful Attack to your target." +
            "\n\nYour identity will not be revealed when you attack.",
            AUSAssets.Conjurer_Conjure)
    ];

    public bool WinConditionMet()
    {
        var alivePlayers = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied());
        var aliveCoven = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.Is(Faction.Coven));
        return alivePlayers == aliveCoven;
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
        RoleBehaviourStubs.OnMeetingStart(this);

        if (Player.AmOwner)
        {
            meetingMenu.GenButtons(MeetingHud.Instance, Player.AmOwner && !Player.HasDied() && Charges > 0 && DayNightMechanic.DayCount >= 2);
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
}

#region Conjurer_Attack
#endregion
public sealed class Conjurer_Attack : AmongUsSalemRoleButton<Conjurer, PlayerControl>
{
    public override string Name => "Attack";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Coven;
    public override float Cooldown => OptionGroupSingleton<Conjurer_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Necronomicon;

    public override void ClickHandler()
    {
        if (Target != null)
        {
            if (MiscUtils.SuccessfulVisit(Role.Player, Target, true, true))
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

        if (Role.Player.CanKill(Target)) MiscUtils.RpcApplyDeathReason(Role.Player, Target, DeathReasonShow.KilledByTheCoven);
        else MiscUtils.ShowNotification(MessageTexts.TooMuchDefense(Role.Player, Target), Color.white);
        MiscUtils.PostSuccessfulVisit(Role.Player, Target, true, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(false, Distance);
    }

    public override bool CanUse()
    {
        return base.CanUse() && Role.Player.Data.Role is ICovenRole coven && coven.Necronomicon;
    }
}

#region Conjurer_Options
#endregion
public sealed class Conjurer_Options : AbstractOptionGroup<Conjurer>
{
    public override string GroupName => TouLocale.Get(TouNames.Conjurer, "Conjurer");

    [ModdedNumberOption("<color=#ab42ef>Conjurer</color> <color=#ab42ef>Attack</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("<color=#ab42ef>Conjurer</color> Max <color=#ab42ef>Conjures</color>", 1f, 15f, 1f)]
    public float Charges { get; set; } = 1f;
}