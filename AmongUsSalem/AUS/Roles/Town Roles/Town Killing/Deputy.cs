using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using Color = UnityEngine.Color;
using UnityEngine;
using System.Collections;

namespace AmongUsSalem.Roles;

#region Deputy
#endregion
public sealed class Deputy(IntPtr cppPtr) 
    : CrewmateRole(cppPtr), ICustomAURole, IWikiDiscoverable, IContinueGame
{
    public bool continueGame => true;
    public string RoleName { get; set; } = "Deputy";
    public string revealText => "is a powerful force for Justice.";
    public string RoleDescription => "Shoot players at Day.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction Faction { get; set; } = Faction.Town;
    public Color RoleColor { get; set; } = AUSColors.Town;
    public Alignment Alignment => Alignment.TownKilling;

    public Attack Attack { get; set; } = Attack.Basic;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.Basic;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.DeputyRoleCard,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ICustomAURole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#06E00C>Deputy</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#06E00C>Town</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#06E00C>Town</color> <color=#1e45d4>Killing</color>" +
            "\n<color=#fdbc00>Goal:</color> Hang every criminal and evildoer." +
            $"\n\nAttributes:" +
            "\nIllusionists can make you miss your shot." +
            "\nFramers can make you shoot your target successfully." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("High Noon",
            "You can Shoot a player during the Day. If your target is not a Town member and has no Defense, you will kill them. Else, you will miss your shot.",
            AUSAssets.Deputy_Shoot)
    ];


    [MethodRpc((uint)AUSRpc.Deputy_Shoot, SendImmediately = true)]
    public static void RpcDeputy_Shoot(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Deputy)
        {
            Logger<AUSPlugin>.Error("RpcDeputy_Shoot - Invalid Deputy");
            return;
        }

        if (player.CanKill(target) && !target.IsIllusioned() && !(target.Is(Faction.Town) && player.Is(Faction.Town)))
        {
            if (player.AmOwner()) MiscUtils.RpcApplyDeathReason(player, target, DeathReasonShow.ShotByADeputy, false, false);
            AUSAssets.PlaySound(AUSAssets.Deputy_Shoot_SFX);

            MiscUtils.ShowNotification(Info(Type.Shot, target), Color.white, AUSAssets.DeputyRoleCard.LoadAsset());
            MiscUtils.AddFakeChat(player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Town, "Deputy Info"), Info(Type.Shot, target));
        }
        else if (player.CanKill(target) && target.IsFramed())
        {
            if (player.AmOwner()) MiscUtils.RpcApplyDeathReason(player, target, DeathReasonShow.ShotByADeputy, false, false);
            AUSAssets.PlaySound(AUSAssets.Deputy_Shoot_SFX);

            MiscUtils.ShowNotification(Info(Type.Shot, target), Color.white, AUSAssets.DeputyRoleCard.LoadAsset());
            MiscUtils.AddFakeChat(player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Town, "Deputy Info"), Info(Type.Shot, target));
        }
        else if (player.AmOwner())
        {
            MiscUtils.ShowNotification(Info(Type.Missed, target), Color.white, AUSAssets.DeputyRoleCard.LoadAsset());
            MiscUtils.AddFakeChat(player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Town, "Deputy Info"), Info(Type.Missed, target));
        }
        
        if (Debugger.IsDebuggerActive)
        {
        CalculatedVoting.EvidenceAgainst.Remove(target);
        CalculatedVoting.KillerContagious.Remove(target);
        CalculatedVoting.QueueEvidenceAgainst.Remove(target);
        CalculatedVoting.QueueKillerContagious.Remove(target);
        }
    }

    public enum Type { Shot, Missed }
    public static string Info(Type type, PlayerControl target)
    {
        if (type == Type.Shot) return "A <b><color=#06E00C>Deputy</color></b> decided to fire their Revolver!";
        return $"You missed your shot! This could be because {target.GetDefaultAppearance().PlayerName} has.";
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
                AUSAssets.Deputy_Shoot,
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
        
        RpcDeputy_Shoot(Player, target);
        Charges--;

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

#region Deputy_Options
#endregion
public sealed class Deputy_Options : AbstractOptionGroup<Deputy>
{
    public override string GroupName => "Deputy";

    [ModdedNumberOption("<color=#06E00C>Deputy</color> Max <color=#4a86e8>Shots</color>", 1f, 30f, 1f)]
    public float Charges { get; set; } = 1f;
}