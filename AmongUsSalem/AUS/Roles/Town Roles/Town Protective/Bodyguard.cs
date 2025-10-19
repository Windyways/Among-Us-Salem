using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using Color = UnityEngine.Color;
using UnityEngine;

namespace AmongUsSalem.Roles;

#region Bodyguard
#endregion
public sealed class Bodyguard(IntPtr cppPtr)
    : CrewmateRole(cppPtr), IWikiDiscoverable, ICustomAURole
{
    public string RoleName { get; set; } = "Bodyguard";
    public string revealText => "is a trained protector.";
    public string RoleDescription => "Guard a player with your life.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction Faction { get; set; } = Faction.Town;
    public Color RoleColor { get; set; } = AUSColors.Town;
    public Alignment Alignment => Alignment.TownProtective;

    public Attack Attack { get; set; } = Attack.Powerful;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.Powerful;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;


    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.BodyguardRoleCard,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ICustomAURole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#06E00C>Bodyguard</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#06E00C>Town</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#06E00C>Town</color> <color=#1e45d4>Protective</color>" +
            "\n<color=#fdbc00>Goal:</color> Hang every criminal and evildoer." +
            $"\n\nAttributes:" +
            "\nYou cannot counterattack passive attacks." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Guard",
            "If your target is directly attacked or is the victim of a harmful visit, you and the visitor will fight, stopping them from attacking your target." +
            "\n\nYou will deal a Powerful Attack to your foe and yourself.",
            AUSAssets.Bodyguard_Guard),

        new("Self Protect",
            "You may stay at gome and protect yourself to gain Basic Defense.",
            AUSAssets.Bodyguard_SelfProtect)
    ];

    public override void OnMeetingStart()
    {
        GuardedPlayer = null;
        isSelfProtected = false;
    }

    public enum Type { AttackedButProtected, AttackedButSelfProtected }
    public static string Info(Type type)
    {
        if (type is Type.AttackedButProtected) return "Someone attacked you, but a <b><color=#06E00C>Bodyguard</color></b> protected you!";
        return "Someone attacked you, but your armor protected you!";
    }

    [MethodRpc((uint)AUSRpc.Bodyguard_Guard, SendImmediately = true)]
    public static void RpcBodyguard_Guard(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Bodyguard)
        {
            Logger<AUSPlugin>.Error("RpcBodyguard_Guard - Invalid Bodyguard");
            return;
        }

        var bodyguard = player.GetRole<Bodyguard>();
        bodyguard.GuardedPlayer = target;
    }

    [MethodRpc((uint)AUSRpc.Bodyguard_SelfProtect, SendImmediately = true)]
    public static void RpcBodyguard_SelfProtect(PlayerControl player)
    {
        if (player.Data.Role is not Bodyguard)
        {
            Logger<AUSPlugin>.Error("RpcBodyguard_SelfProtect - Invalid Bodyguard");
            return;
        }

        var bodyguard = player.GetRole<Bodyguard>();
        if (bodyguard.Player.Data.Role is ICustomAURole ausRole) ausRole.ApplyDefense(Defense.Basic);
    }

    [MethodRpc((uint)AUSRpc.Bodyguard_Notify, SendImmediately = true)]
    public static void RpcBodyguard_Notify(PlayerControl visitor, PlayerControl target)
    {
        foreach (var bodyguards in MiscUtils.GetPlayersWithRole<Bodyguard>())
        {
            var bodyguard = bodyguards.GetRole<Bodyguard>();
            if (bodyguard.Player == target && bodyguard.Player.AmOwner())
            {
                MiscUtils.AddFakeChat(bodyguard.Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Town, "Bodyguard Info"), Info(Type.AttackedButSelfProtected));
            }

            if (bodyguard.GuardedPlayer == target)
            {
                if (target.AmOwner()) // Always put AmOwner inside of Rpcs that have RpcCustomMurder, or for client side notifications!
                {
                    // Might have to test these with other ppl since this doesnt appear to kill using MCI but shows the info.
                    if (bodyguard.Player.CanKill(visitor)) MiscUtils.RpcApplyDeathReason(bodyguard.Player, visitor, DeathReasonShow.KilledByABodyguard);
                    if (bodyguard.Player.CanKill(bodyguard.Player)) MiscUtils.RpcApplyDeathReason(bodyguard.Player, bodyguard.Player, DeathReasonShow.DiedWhileDefendingTheirTarget);

                    MiscUtils.AddFakeChat(target.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Town, "Bodyguard Info"), Info(Type.AttackedButProtected));
                }
            }
        }
    }

    public PlayerControl GuardedPlayer;
    public bool isSelfProtected;
}

#region Bodyguard_Guard
#endregion
public sealed class Bodyguard_Guard : AmongUsSalemRoleButton<Bodyguard, PlayerControl>
{
    public override string Name => "Guard";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Town;
    public override float Cooldown => OptionGroupSingleton<Bodyguard_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Bodyguard_Guard;

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
            var button = CustomButtonSingleton<Bodyguard_SelfProtect>.Instance;
            button.ResetCooldownAndOrEffect();
        }

        Bodyguard.RpcBodyguard_Guard(Player, Target);
        MiscUtils.PostSuccessfulVisit(Player, Target, false, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        if (target == null) return base.IsTargetValid(target);
        return base.IsTargetValid(target) && Role.GuardedPlayer != target;
    }
}

#region Bodyguard_SelfProtect
#endregion
public sealed class Bodyguard_SelfProtect : AmongUsSalemRoleButton<Bodyguard>
{
    public override string Name => "Self Protect";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => AUSColors.Town;
    public override float Cooldown => OptionGroupSingleton<Bodyguard_Options>.Instance.Cooldown;
    public override int MaxUses => (int)OptionGroupSingleton<Bodyguard_Options>.Instance.Charges;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Bodyguard_SelfProtect;

    public override void ClickHandler()
    {
        if (MiscUtils.SuccessfulVisit(Player, Player, false, false))
        {
            base.ClickHandler();
        }
    }

    protected override void OnClick()
    {
        if (Player.AmOwner)
        {
            var button = CustomButtonSingleton<Bodyguard_Guard>.Instance;
            button.ResetCooldownAndOrEffect();
        }

        Bodyguard.RpcBodyguard_SelfProtect(Player);
        MiscUtils.PostSuccessfulVisit(Player, Player, false, false);
    }
}

#region Bodyguard_Options
#endregion
public sealed class Bodyguard_Options : AbstractOptionGroup<Bodyguard>
{
    public override string GroupName => "Bodyguard";

    [ModdedNumberOption("<color=#06E00C>Bodyguard</color> <color=#4a86e8>Guard</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("<color=#06E00C>Bodyguard</color> Max <color=#4a86e8>Self Protects</color>", 1f, 30f, 1f)]
    public float Charges { get; set; } = 2;
}