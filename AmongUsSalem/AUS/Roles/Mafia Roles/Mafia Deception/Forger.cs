using System.Text;
using AmongUsSalem.Events;
using AmongUsSalem.Modules;
using AmongUsSalem.Modules.Components;
using AmongUsSalem.Options;
using AmongUsSalem.Utilities;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement.Roles;

#region Forger
#endregion
public sealed class Forger(IntPtr cppPtr)
    : ImpostorRole(cppPtr), IWikiDiscoverable, ICustomAURole
{
    public string RoleName { get; set; } = "Forger";
    public string revealText => "is good at forging documents.";
    public string RoleDescription => "Make players appear as they aren't.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;

    public Faction Faction { get; set; } = Faction.Mafia;
    public Color RoleColor { get; set; } = AUSColors.Mafia;
    public Alignment Alignment => Alignment.MafiaDeception;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public DeathReasonShow deathReasonShow { get; set; } = DeathReasonShow.Alive;

    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = false,
        Icon = AUSAssets.ForgerRoleCard,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ICustomAURole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#DD0000>Forger</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#DD0000>Mafia</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#DD0000>Mafia</color> <color=#1e45d4>Deception</color>" +
            "\n<color=#fdbc00>Goal:</color> Kill anyone that will not submit to the Mafia." +
            $"\n\nAttributes:" +
            "\nIf all Mafia Killing roles are dead, you will be promoted to Mafioso." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Forge",
            "You can forge a player or a dead body at Night." +
            "\n\nIf your target dies, or is dead, you will make their role appear to be the one you choose.",
            AUSAssets.Forger_Forge)
    ];

    [MethodRpc((uint)AUSRpc.Forger_DoForge, SendImmediately = true)]
    public static void RpcForger_DoForge(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Forger)
        {
            Logger<AUSPlugin>.Error("RpcForger_DoForge - Invalid Forger");
            return;
        }

        var forger = player.GetRole<Forger>();
        if (player.AmOwner())
        {
            forger.Charges--;

            var button = CustomButtonSingleton<Forger_ForgePlayer>.Instance;
            button.UsesLeft = forger.Charges;
            
            var button2 = CustomButtonSingleton<Forger_ForgeBody>.Instance;
            button2.UsesLeft = forger.Charges;

            if (forger.forgingRole is ICustomAURole customRole)
            {
                target.RpcAddModifier<DeepfakeRole>(customRole.RoleName, customRole.RoleColor);
            }
        }
    }

    [MethodRpc((uint)AUSRpc.Forger_Forge, SendImmediately = true)]
    public static void RpcForger_Forge(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Forger)
        {
            Logger<AUSPlugin>.Error("RpcForger_Forge - Invalid Forger");
            return;
        }

        var forger = player.GetRole<Forger>();
        forger.ForgedPlayer = target;
    }

    public override void OnMeetingStart()
    {
        ForgedPlayer = null;
    }

    public void OnTargetDeath(PlayerControl target, DeathReason? reason)
    {
        if (target == ForgedPlayer) 
        {
            RpcForger_DoForge(Player, target);
            ForgedPlayer = null;
        }
    }

    public int Charges = (int)OptionGroupSingleton<Forger_Options>.Instance.Charges;
    public PlayerControl ForgedPlayer;
    public RoleBehaviour forgingRole;

    public void ForgeMenu()
    {
        if (Minigame.Instance != null)
        {
            return;
        }

        var shapeMenu = GuesserMenu.Create();
        shapeMenu.Begin(IsRoleValid, ClickRoleHandle);

        void ClickRoleHandle(RoleBehaviour role)
        {
            forgingRole = role;
            shapeMenu.Close();
        }
    }

    private bool IsRoleValid(RoleBehaviour role)
    {
        if (role.IsDead) return false;
        if (role is IGhostRole) return false;
        return true;
    }
}

#region Forger_ForgePlayer
#endregion
public sealed class Forger_PickForge : AmongUsSalemRoleButton<Forger>
{
    public override string Name => "Forging Role";
    public override BaseKeybind Keybind => Keybinds.TertiaryAction;
    public override Color TextOutlineColor => AUSColors.Mafia;
    public override float Cooldown => 0.1f;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Forger_Forge;
    public override ButtonLocation Location => ButtonLocation.BottomLeft;

    public override void ClickHandler()
    {
        if (Timer <= 0)
        {
            if (MiscUtils.SuccessfulVisit(Player, Player, false, false))
            {
                base.ClickHandler();
            }
        }
    }

    protected override void OnClick()
    {
        Role.ForgeMenu();

        MiscUtils.PostSuccessfulVisit(Player, Player, false, false, false);
    }
}

#region Forger_ForgePlayer
#endregion
public sealed class Forger_ForgePlayer : AmongUsSalemRoleButton<Forger, PlayerControl>
{
    public override string Name => "Forge";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Mafia;
    public override float Cooldown => OptionGroupSingleton<Forger_Options>.Instance.Cooldown;
    public override int MaxUses => (int)OptionGroupSingleton<Forger_Options>.Instance.Charges;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Forger_Forge;

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
            var button = CustomButtonSingleton<Forger_ForgeBody>.Instance;
            button.ResetCooldownAndOrEffect();
            button.UsesLeft = Role.Charges;
        }

        Forger.RpcForger_Forge(Player, Target);
        MiscUtils.PostSuccessfulVisit(Player, Target, false, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(false, Distance, predicate: x =>
            x != Role.ForgedPlayer);
    }

    public override bool CanUse()
    {
        return base.CanUse() && Role.forgingRole != null;
    }
}

#region Forger_ForgeBody
#endregion
public sealed class Forger_ForgeBody : AmongUsSalemRoleButton<Forger, DeadBody>
{
    public override string Name => "Forge (Body)";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => AUSColors.Mafia;
    public override float Cooldown => OptionGroupSingleton<Forger_Options>.Instance.Cooldown;
    public override int MaxUses => (int)OptionGroupSingleton<Forger_Options>.Instance.Charges;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Forger_Forge;

    public override void ClickHandler()
    {
        if (Target != null && Timer <= 0)
        {
            var targetId = Target.ParentId;
            var targetPlayer = MiscUtils.PlayerById(targetId);

            if (MiscUtils.SuccessfulVisit(Player, targetPlayer, false, true))
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

        var targetId = Target.ParentId;
        var targetPlayer = MiscUtils.PlayerById(targetId);

        if (targetPlayer == null)
        {
            return; // Someone may have left mid game or something and gc just vacuumed, but idk. better safe than sorry ig.
        }

        if (Player.AmOwner)
        {
            var button = CustomButtonSingleton<Forger_ForgePlayer>.Instance;
            button.ResetCooldownAndOrEffect();
            button.DecreaseUses();
        }

        Forger.RpcForger_DoForge(Player, targetPlayer);
        MiscUtils.PostSuccessfulVisit(Player, targetPlayer, false, true);
    }

    public override DeadBody? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetNearestDeadBody(Distance);
    }

    public override bool CanUse()
    {
        return base.CanUse() && Role.forgingRole != null;
    }

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && Target != null;
    }
}

#region Forger_Options
#endregion
public sealed class Forger_Options : AbstractOptionGroup<Forger>
{
    public override string GroupName => "Forger";

    [ModdedNumberOption("<color=#DD0000>Forger</color> <color=#4a86e8>Forge</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
    
    [ModdedNumberOption("<color=#DD0000>Forger</color> Max <color=#4a86e8>Forges</color>", 1f, 30f, 1f)]
    public float Charges { get; set; } = 2f;
}