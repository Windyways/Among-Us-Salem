using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement.Roles;

#region Illusionist
#endregion
public sealed class Illusionist(IntPtr cppPtr)
    : NeutralRole(cppPtr), IWikiDiscoverable, IAUSRole, ICovenRole
{
    public string RoleName => TouLocale.Get(TouNames.Illusionist, "Illusionist");
    public string revealText => ".";
    public string RoleDescription => "Placeholder.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction RoleFaction { get; set; } = Faction.Coven;
    public Color RoleColor { get; set; } = AUSColors.Coven;
    public Alignment Alignment => Alignment.CovenDeception;
    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public DeathReasonShow deathReasonShow { get; set; } = DeathReasonShow.Alive;

    public NecronomiconPriority NecronomiconPriority => NecronomiconPriority.Illusionist;
    public bool Necronomicon { get; set; }

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.IllusionistRoleCard,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IAUSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#ab42ef>Illusionist</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#ab42ef>Coven</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#ab42ef>Coven</color> <color=#1e45d4>Deception</color>" +
            "\n<color=#fdbc00>Goal:</color> Kill all who would oppose the Coven." +
            $"\n\nAttributes:" +
            "\nWith the Necronomicon you may Basic Attack someone at Night." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Conjure",
            "Conjure a meteor to illusion a player during the day." +
            "\n\nThe meteor will deal a Powerful Illusion to your target." +
            "\n\nYour identity will not be revealed when you illusion.",
            AUSAssets.Illusionist_Illusion)
    ];

    public bool WinConditionMet()
    {
        var aliveCoven = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.Is(Faction.Coven));
        if (aliveCoven == 0) return false;

        var result = Helpers.GetAlivePlayers().Count <= aliveCoven && MiscUtils.KillersAliveCount() == aliveCoven;
        return result;
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet();
    }

    [MethodRpc((uint)AUSRpc.Illusionist_Illusion, SendImmediately = true)]
    public static void RpcIllusionist_Illusion(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Illusionist)
        {
            Logger<AUSPlugin>.Error("RpcIllusionist_Illusion - Invalid Illusionist");
            return;
        }

        var illusionist = player.GetRole<Illusionist>();
        illusionist.IllusionedPlayer = target;
    }

    public override void OnVotingComplete()
    {
        RoleBehaviourStubs.OnVotingComplete(this);
        IllusionedPlayer = null;
    }

    public PlayerControl IllusionedPlayer;
}

#region Illusionist_Illusion
#endregion
public sealed class Illusionist_Illusion : AmongUsSalemRoleButton<Illusionist, PlayerControl>
{
    public override string Name => "Illusion";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Coven;
    public override float Cooldown => OptionGroupSingleton<Illusionist_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Illusionist_Illusion;

    public override void ClickHandler()
    {
        if (Target != null)
        {
            if (MiscUtils.SuccessfulVisit(Role.Player, Target, false, false))
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

        Illusionist.RpcIllusionist_Illusion(Role.Player, Target);
        MiscUtils.PostSuccessfulVisit(Role.Player, Target, true, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        if (target == null) return base.IsTargetValid(target);
        return base.IsTargetValid(target) &&
            !(target.Data.Role is IAUSRole ausrole && ausrole.RoleFaction != Role.RoleFaction);
    }
}

#region Illusionist_Attack
#endregion
public sealed class Illusionist_Attack : AmongUsSalemRoleButton<Illusionist, PlayerControl>
{
    public override string Name => "Attack";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Coven;
    public override float Cooldown => OptionGroupSingleton<Illusionist_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.NecronomiconButton;

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
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        if (target == null) return base.IsTargetValid(target);
        return base.IsTargetValid(target) &&
            !(target.Data.Role is IAUSRole ausrole && ausrole.RoleFaction == Role.RoleFaction);
    }

    public override bool CanUse()
    {
        return base.CanUse() && Role.Player.Data.Role is ICovenRole coven && coven.Necronomicon;
    }
}

#region Illusionist_Options
#endregion
public sealed class Illusionist_Options : AbstractOptionGroup<Illusionist>
{
    public override string GroupName => TouLocale.Get(TouNames.Illusionist, "Illusionist");

    [ModdedNumberOption("<color=#ab42ef>Illusionist</color> <color=#ab42ef>Illusion</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}