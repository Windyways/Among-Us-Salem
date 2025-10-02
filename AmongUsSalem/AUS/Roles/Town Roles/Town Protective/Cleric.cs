using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using Color = UnityEngine.Color;
using UnityEngine;

namespace AmongUsSalem.Roles;

#region Cleric
#endregion
public sealed class Cleric(IntPtr cppPtr)
    : CrewmateRole(cppPtr), IWikiDiscoverable, IAUSRole
{
    public string RoleName { get; set; } = "Cleric";
    public string revealText => "is a trained protector.";
    public string RoleDescription => "";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction Faction { get; set; } = Faction.Town;
    public Color RoleColor { get; set; } = AUSColors.Town;
    public Alignment Alignment => Alignment.TownProtective;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;


    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.ClericRoleCard,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IAUSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#06E00C>Cleric</color>" +
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
        new("Barrier",
            "If your target is directly attacked or is the victim of a harmful visit, you and the visitor will fight, stopping them from attacking your target." +
            "\n\nYou will deal a Powerful Attack to your foe and yourself.",
            AUSAssets.Cleric_Barrier),

        new("Self Barrier",
            "You may stay at gome and protect yourself to gain Basic Defense.",
            AUSAssets.Cleric_SelfBarrier)
    ];

    public override void OnMeetingStart()
    {
        BarrieredPlayers.Clear();
    }

    public enum Type { AttackedButProtected, TargetAttacked }
    public static string Info(Type type, PlayerControl target)
    {
        if (type is Type.AttackedButProtected) return "Someone attacked you, but a <b><color=#4a86e8>Barrier</color></b> protected you!";
        return target.GetDefaultAppearance().PlayerName + " was attacked last Night!";
    }

    [MethodRpc((uint)AUSRpc.Cleric_Barrier, SendImmediately = true)]
    public static void RpcCleric_Barrier(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Cleric)
        {
            Logger<AUSPlugin>.Error("RpcCleric_Barrier - Invalid Cleric");
            return;
        }

        var cleric = player.GetRole<Cleric>();
        cleric.BarrieredPlayers.Add(target.PlayerId);

        if (target.Data.Role is IAUSRole ausRole) ausRole.ApplyDefense(Defense.Powerful);
    }

    [MethodRpc((uint)AUSRpc.Cleric_SelfBarrier, SendImmediately = true)]
    public static void RpcCleric_SelfBarrier(PlayerControl player)
    {
        if (player.Data.Role is not Cleric)
        {
            Logger<AUSPlugin>.Error("RpcCleric_SelfBarrier - Invalid Cleric");
            return;
        }

        var cleric = player.GetRole<Cleric>();
        if (cleric.Player.Data.Role is IAUSRole ausRole) ausRole.ApplyDefense(Defense.Powerful);
    }

    [MethodRpc((uint)AUSRpc.Cleric_Notify, SendImmediately = true)]
    public static bool RpcCleric_Notify(PlayerControl visitor, PlayerControl target)
    {
        foreach (var clerics in MiscUtils.GetPlayersWithRole<Cleric>())
        {
            var cleric = clerics.GetRole<Cleric>();
            if (cleric.Player.AmOwner())
            {
                MiscUtils.AddFakeChat(cleric.Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Town, "Cleric Info"), Info(Type.TargetAttacked, target));
            }

            if (cleric.BarrieredPlayers.Contains(target.PlayerId))
            {
                if (target.AmOwner()) // Always put AmOwner inside of Rpcs that have RpcCustomMurder, or for client side notifications!
                {
                    MiscUtils.AddFakeChat(target.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Town, "Cleric Info"), Info(Type.AttackedButProtected, target));
                }
            }
        }

        return false;
    }

    public List<byte> BarrieredPlayers = new List<byte>();
}

#region Cleric_Barrier
#endregion
public sealed class Cleric_Barrier : AmongUsSalemRoleButton<Cleric, PlayerControl>
{
    public override string Name => "Barrier";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Town;
    public override float Cooldown => OptionGroupSingleton<Cleric_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Cleric_Barrier;

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
            var button = CustomButtonSingleton<Cleric_SelfBarrier>.Instance;
            button.ResetCooldownAndOrEffect();
        }

        Cleric.RpcCleric_Barrier(Player, Target);
        MiscUtils.PostSuccessfulVisit(Player, Target, false, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        if (target == null) return base.IsTargetValid(target);
        return base.IsTargetValid(target) && !Role.BarrieredPlayers.Contains(target.PlayerId);
    }
}

#region Cleric_SelfBarrier
#endregion
public sealed class Cleric_SelfBarrier : AmongUsSalemRoleButton<Cleric>
{
    public override string Name => "Self Barrier";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => AUSColors.Town;
    public override float Cooldown => OptionGroupSingleton<Cleric_Options>.Instance.Cooldown;
    public override int MaxUses => (int)OptionGroupSingleton<Cleric_Options>.Instance.Charges;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Cleric_SelfBarrier;

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
            var button = CustomButtonSingleton<Cleric_Barrier>.Instance;
            button.ResetCooldownAndOrEffect();
        }

        Cleric.RpcCleric_SelfBarrier(Player);
        MiscUtils.PostSuccessfulVisit(Player, Player, false, false);
    }
}

#region Cleric_Options
#endregion
public sealed class Cleric_Options : AbstractOptionGroup<Cleric>
{
    public override string GroupName => "Cleric";

    [ModdedNumberOption("<color=#06E00C>Cleric</color> <color=#4a86e8>Barrier</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("<color=#06E00C>Cleric</color> Max <color=#4a86e8>Self Barriers</color>", 1f, 30f, 1f)]
    public float Charges { get; set; } = 1;
}