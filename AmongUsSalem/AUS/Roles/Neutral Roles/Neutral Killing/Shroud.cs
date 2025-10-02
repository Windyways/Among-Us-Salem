using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using AmongUsSalem.Utilities;
using UnityEngine;
using System.Collections;

namespace AmongUsSalem.LifeImprovement.Roles;

#region Shroud
#endregion
public sealed class Shroud(IntPtr cppPtr)
    : NeutralRole(cppPtr), IWikiDiscoverable, IAUSRole
{
    public string RoleName { get; set; } = "Shroud";
    public string revealText => "is like a ghost.";
    public string RoleDescription => "";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.Neutral;
    public Color RoleColor { get; set; } = AUSColors.Shroud;
    public Alignment Alignment => Alignment.NeutralKilling;
    public Attack Attack { get; set; } = Attack.Basic;
    public Defense Defense { get; set; } = Defense.Basic;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.Basic;
    public Defense ogDefense { get; set; } = Defense.Basic;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;


    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.ShroudRoleCard,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IAUSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#6699ff>Shroud</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#A9A9A9>Neutral</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#A9A9A9>Neutral</color> <color=#1e45d4>Killing</color>" +
            "\n<color=#fdbc00>Goal:</color> Kill everyone in the town." +
            $"\n\nAttributes:" +
            "\nN/A." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Attack",
            "N/A",
            AUSAssets.Shroud_Attack),
            
        new("Shroud",
            "N/A",
            AUSAssets.Shroud_Shroud)
    ];

    public bool WinConditionMet()
    {
        var aliveShrouds = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.IsRole<Shroud>());
        if (aliveShrouds == 0) return false;

        var result = MiscUtils.GetAlivePlayersToEnd().Count <= aliveShrouds && MiscUtils.KillersAliveCount() == aliveShrouds;
        return result;
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet();
    }

    public override void OnMeetingStart()
    {
        if (ShroudedPlayer != null && !ShroudedPlayer.HasDied() && !targetVisited && OptionGroupSingleton<Shroud_Options>.Instance.AttacksShroudedIfTheyDontVisit && Player.CanKill(ShroudedPlayer))
        {
            Coroutines.Start(PostMeetingIntro());
        }
        else ShroudedPlayer = null;
    }

    public IEnumerator PostMeetingIntro()
    {
        yield return new WaitForSeconds(DayNightMechanic.PostMeetingIntroTime);

        MiscUtils.RpcApplyDeathReason(Player, ShroudedPlayer, DeathReasonShow.KilledByAShroud, false, false);
        ShroudedPlayer = null;
    }

    public static string Info(PlayerControl shrouded, PlayerControl target)
    {
        return shrouded.GetDefaultAppearance().PlayerName + " visited " + target.GetDefaultAppearance().PlayerName + " and you attacked them!";
    }

    [MethodRpc((uint)AUSRpc.Shroud_Shroud, SendImmediately = true)]
    public static void RpcShroud_Shroud(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Shroud)
        {
            Logger<AUSPlugin>.Error("RpcShroud_Shroud - Invalid Shroud");
            return;
        }

        var shroud = player.GetRole<Shroud>();
        shroud.ShroudedPlayer = target;
    }

    [MethodRpc((uint)AUSRpc.Shroud_Notify, SendImmediately = true)]
    public static void RpcShroud_Notify(PlayerControl player, PlayerControl target)
    {
        foreach (var shrouds in MiscUtils.GetPlayersWithRole<Shroud>())
        {
            var shroud = shrouds.GetRole<Shroud>();
            if (shroud.ShroudedPlayer == player)
            {
                shroud.targetVisited = true;
                if (shroud.Player.AmOwner())
                {
                    if (player.CanKill(target, Attack.Basic))
                    {
                        MiscUtils.RpcApplyDeathReason(player, target, DeathReasonShow.KilledByAShroud);
                        MiscUtils.ShowNotification(Info(player, target), Color.white, AUSAssets.ShroudRoleCard.LoadAsset());
                        MiscUtils.AddFakeChat(shroud.Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Shroud, "Shroud Info"), Info(player, target));
                    }
                }

                if (player.AmOwner)
                {
                    var buttons = CustomButtonManager.Buttons.Where(x => x.Enabled(player.Data.Role) && x.Timer <= 0).ToList();
                    foreach (var button in buttons) button.ResetCooldownAndOrEffect();
                }

                shroud.ShroudedPlayer = null;
            }
        }
    }

    public PlayerControl ShroudedPlayer;
    public bool targetVisited;
}

#region Shroud_Attack
#endregion
public sealed class Shroud_Attack : AmongUsSalemRoleButton<Shroud, PlayerControl>
{
    public override string Name => "Attack";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Shroud;
    public override float Cooldown => OptionGroupSingleton<Shroud_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Shroud_Attack;

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

        if (Player.AmOwner)
        {
            var button = CustomButtonSingleton<Shroud_Shroud>.Instance;
            button.ResetCooldownAndOrEffect();
        }

        if (Player.CanKill(Target)) MiscUtils.RpcApplyDeathReason(Player, Target, DeathReasonShow.KilledByAShroud);
        else
        {
            MiscUtils.ShowNotification(MessageTexts.TooMuchDefense(Player, Target), Color.white);
            MiscUtils.AddFakeChat(Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Neutral, "General Info"), MessageTexts.TooMuchDefense(Player, Target));
        }
        MiscUtils.PostSuccessfulVisit(Player, Target, true, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }
}

#region Shroud_Shroud
#endregion
public sealed class Shroud_Shroud : AmongUsSalemRoleButton<Shroud, PlayerControl>
{
    public override string Name => "Shroud";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => AUSColors.Shroud;
    public override float Cooldown => OptionGroupSingleton<Shroud_Options>.Instance.ShroudCD;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Shroud_Shroud;

    public override void ClickHandler()
    {
        if (Target != null && Timer <= 0)
        {
            if (MiscUtils.SuccessfulVisit(Player, Target, false, true))
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

        if (Player.AmOwner)
        {
            var button = CustomButtonSingleton<Shroud_Attack>.Instance;
            button.ResetCooldownAndOrEffect();
        }

        Shroud.RpcShroud_Shroud(Role.Player, Target);
        MiscUtils.PostSuccessfulVisit(Player, Target, false, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        if (target == null) return base.IsTargetValid(target);
        return base.IsTargetValid(target) &&
            Role.ShroudedPlayer != target;
    }
}

#region Shroud_Options
#endregion
public sealed class Shroud_Options : AbstractOptionGroup<Shroud>
{
    public override string GroupName => "Shroud";

    [ModdedNumberOption("<color=#6699ff>Shroud</color> <color=#4a86e8>Attack</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("<color=#6699ff>Shroud</color> <color=#4a86e8>Shroud</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float ShroudCD { get; set; } = 25f;

    [ModdedToggleOption("<color=#6699ff>Shroud</color> Attacks <color=#4a86e8>Shrouded</color> If They Don't Visit")]
    public bool AttacksShroudedIfTheyDontVisit { get; set; } = true;
}