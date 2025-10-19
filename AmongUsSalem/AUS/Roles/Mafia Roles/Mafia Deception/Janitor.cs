using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement.Roles;

#region Janitor
#endregion
public sealed class Janitor(IntPtr cppPtr)
    : ImpostorRole(cppPtr), IWikiDiscoverable, ICustomAURole
{
    public string RoleName { get; set; } = "Janitor";
    public string revealText => "cleans up dead bodies.";
    public string RoleDescription => "Make roles hidden.";
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
        Icon = AUSAssets.JanitorRoleCard,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ICustomAURole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#DD0000>Janitor</color>" +
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
        new("Clean",
            "You can clean a player or a dead body at Night" +
            "\n\nIf your target dies, or is dead, their role will appear 'Cleaned' to others." +
            "\n\nYou will know your targets true role.",
            AUSAssets.Janitor_Clean)
    ];

    [MethodRpc((uint)AUSRpc.Janitor_DoClean, SendImmediately = true)]
    public static void RpcJanitor_DoClean(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Janitor)
        {
            Logger<AUSPlugin>.Error("RpcJanitor_DoClean - Invalid Janitor");
            return;
        }

        var janitor = player.GetRole<Janitor>();
        if (player.AmOwner())
        {
            janitor.Charges--;

            var button = CustomButtonSingleton<Janitor_CleanPlayer>.Instance;
            button.UsesLeft = janitor.Charges;
            
            var button2 = CustomButtonSingleton<Janitor_CleanBody>.Instance;
            button2.UsesLeft = janitor.Charges;

            target.RpcAddModifier<DeepfakeRole>("Cleaned", AUSColors.Neutral);

            var roleWhenAlive = target.GetRoleWhenAlive();
            MiscUtils.ShowNotification(Info(target, roleWhenAlive.Role()), Color.white, AUSAssets.JanitorRoleCard.LoadAsset());
            MiscUtils.AddFakeChat(player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Mafia, "Janitor Info"), Info(target, roleWhenAlive.Role()));
        }
    }

    [MethodRpc((uint)AUSRpc.Janitor_Clean, SendImmediately = true)]
    public static void RpcJanitor_Clean(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Janitor)
        {
            Logger<AUSPlugin>.Error("RpcJanitor_Clean - Invalid Janitor");
            return;
        }

        var janitor = player.GetRole<Janitor>();
        janitor.CleanedPlayer = target;
    }

    public override void OnMeetingStart()
    {
        CleanedPlayer = null;
    }

    public static string Info(PlayerControl target, string text)
    {
        return $"You secretly know that {target.GetDefaultAppearance().PlayerName}'s role was {text}.";
    }

    public void OnTargetDeath(PlayerControl target, DeathReason? reason)
    {
        if (target == CleanedPlayer) 
        {
            RpcJanitor_DoClean(Player, target);
            CleanedPlayer = null;
        }
    }

    public int Charges = (int)OptionGroupSingleton<Janitor_Options>.Instance.Charges;
    public PlayerControl CleanedPlayer;
}

#region Janitor_CleanPlayer
#endregion
public sealed class Janitor_CleanPlayer : AmongUsSalemRoleButton<Janitor, PlayerControl>
{
    public override string Name => "Clean";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Mafia;
    public override float Cooldown => OptionGroupSingleton<Janitor_Options>.Instance.Cooldown;
    public override int MaxUses => (int)OptionGroupSingleton<Janitor_Options>.Instance.Charges;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Janitor_Clean;

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
            var button = CustomButtonSingleton<Janitor_CleanBody>.Instance;
            button.ResetCooldownAndOrEffect();
        }

        IncreaseUses(); // don't use up a charge here.

        Janitor.RpcJanitor_Clean(Player, Target);
        MiscUtils.PostSuccessfulVisit(Player, Target, false, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(false, Distance);
    }
}

#region Janitor_CleanBody
#endregion
public sealed class Janitor_CleanBody : AmongUsSalemRoleButton<Janitor, DeadBody>
{
    public override string Name => "Clean (Body)";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Mafia;
    public override float Cooldown => OptionGroupSingleton<Janitor_Options>.Instance.Cooldown;
    public override int MaxUses => (int)OptionGroupSingleton<Janitor_Options>.Instance.Charges;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Janitor_Clean;

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
            var button = CustomButtonSingleton<Janitor_CleanPlayer>.Instance;
            button.ResetCooldownAndOrEffect();
            button.DecreaseUses();
        }

        Janitor.RpcJanitor_DoClean(Player, targetPlayer);
        MiscUtils.PostSuccessfulVisit(Player, targetPlayer, false, true);
    }

    public override DeadBody? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetNearestDeadBody(Distance);
    }
}

#region Janitor_Options
#endregion
public sealed class Janitor_Options : AbstractOptionGroup<Janitor>
{
    public override string GroupName => "Janitor";

    [ModdedNumberOption("<color=#DD0000>Janitor</color> <color=#4a86e8>Clean</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
    
    [ModdedNumberOption("<color=#DD0000>Janitor</color> Max <color=#4a86e8>Cleans</color>", 1f, 30f, 1f)]
    public float Charges { get; set; } = 3f;
}