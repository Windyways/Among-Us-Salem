using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace AmongUsSalem.Roles;

public sealed class PotionMaster(IntPtr cppPtr) : CovenRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Potion Master";
    public string revealText => "works with alchemy.";
    public string RoleDescription => "Town Of Salem 2";
    public string RoleLongDescription => "You are an alchemist who concocts powerful potions.";
    public Color RoleColor { get; set; } = RoleColors.Coven;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.Coven;
    public Alignment Alignment => Alignment.CovenUtility;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;
    public CustomRoleConfiguration Configuration => new(this)
    {
        // CanUseSabotage = OptionGroupSingleton<CovenOptions>.Instance.CanSabotage,
        CanUseVent = OptionGroupSingleton<CovenOptions>.Instance.CanVent,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        Icon = AUSAssets.PotionMasterRoleCard
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
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can protect the Coven and help reveal potential targets.\n" +
            "Kill all who would oppose the Coven." + 
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Harmful",
            "You can use a Harmful potion on a player at Night.\n" +
            "You will deal a Basic Attack to your target. You can only use this if you hold the Necronomicon.",
            AUSAssets.PotionMaster_Harmful),

        new("Barrier",
            "You can Barrier a player at Night.\n" +
            "You will grant your target Powerful Defense for the Night.\n" +
            "You will know if your target is attacked. Your target will know if they were attacked.",
            AUSAssets.PotionMaster_Barrier),

        new("Reveal",
            "You can Reveal a player at Night.\n" +
            "You and your Coven members will learn your target's role. Your target will become Illuminated for 3 Nights.",
            AUSAssets.PotionMaster_Reveal),
    ];

    public bool WinConditionMet() => CovenGameOver.WinConditionMet(this);
    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet() || CovenGameOver.AnyCovenWon(gameOverReason);
    }

    public static string Info(NotificationType type, PlayerControl target = null)
    {
        if (type is NotificationType.PotionMaster_AttackAndBarriered) return "Someone attacked you, but a Barrier protected you!";
        return target.Name() + " was attacked last Night!";
    }

    [MethodRpc((uint)AUSRpc.RpcNotifyPotionMaster)]
    public static void RpcNotify(PlayerControl player, int notifyType, PlayerControl target)
    {
        if (player.AmOwner())
        {
            var notify = (NotificationType)notifyType;
            switch (notify)
            {
                case NotificationType.PotionMaster_AttackAndBarriered:
                    target.AddModifier<SoftCleared>();
                    player.Notify(Info(notify), NotifyMode.OnlyMeeting, sprite: AUSAssets.ClericRoleCard.LoadAsset());
                    break;
                case NotificationType.PotionMaster_TargetAttacked:
                    player.Notify(Info(notify, target), NotifyMode.OnlyMeeting, sprite: AUSAssets.ClericRoleCard.LoadAsset());
                    break;
            }
        }
    }

    public void Function(PlayerControl target, int Button)
    {
        if (Button == 1)
        {
            if (Player.HasNecronomicon())
            {
                if (Player.CanKill(target))
                {
                    Player.RpcCustomMurder(target);
                    VisitingMechanic.RpcAddDeathReason(target, (int)DeathReasonShow.KilledByTheCoven);
                }
                else Player.Notify(Feedback.TooMuchDefense(Player, target), NotifyMode.InstantlyAndMeeting);
            }
        }
        else if (Button == 2)
        {
            if (Player == target)
            {
                Player.RpcAddModifier<BarrieredModifier>(Player);
                AttackDefenseMechanic.RpcApplyDefense(Player, Defense.Powerful, visualize: true);
            }
            else
            {
                target.RpcAddModifier<BarrieredModifier>(Player);
                AttackDefenseMechanic.RpcApplyDefense(target, Defense.Powerful);
            }
        }
        else if (Button == 3)
        {
            target.RpcAddModifier<IlluminatedModifier>(Player);
            if (target.TryGetModifier<SelfReflectionModifier>(out var selfReflection)) // Pacifist Patch!
            {
                // Since all Coven member see revealed players, we have to make sure they get Deepfaked too (Includes this player too).
                foreach (var players in PlayerControl.AllPlayerControls)
                {
                    if (players.Is(Faction.Coven) && players.AmOwner())
                        target.RpcAddModifier<DeepfakeRole>(players, selfReflection.GetRole().NiceName, RoleColors.Town);
                }
            }
            target.RpcAddModifier<RoleLearn>(Player, true);
        }
    }
}

public sealed class PotionMaster_Harmful : TownOfUsRoleButton<PotionMaster, PlayerControl>
{
    public override string Name => "Harmful";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Coven;
    public override float Cooldown => OptionGroupSingleton<CovenOptions>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.PotionMaster_Harmful;

    protected override void OnClick() => VisitingMechanic.CheckVisit(Player, Target, 1, Player.HasNecronomicon(), true);
    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance, predicate: x => 
            !x.Is(Faction.Coven));
    }

    public override bool CanUse()
    {
        return base.CanUse() && Player.HasNecronomicon();
    }
}

public sealed class PotionMaster_Barrier : TownOfUsRoleButton<PotionMaster, PlayerControl>
{
    public override string Name => "Barrier";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => RoleColors.Coven;
    public override float Cooldown => OptionGroupSingleton<PotionMaster_Options>.Instance.BarrierCD;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.PotionMaster_Barrier;

    protected override void OnClick() => VisitingMechanic.CheckVisit(Player, Target, 2, false, true);
    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance);
    }
}

public sealed class PotionMaster_Reveal : TownOfUsRoleButton<PotionMaster, PlayerControl>
{
    public override string Name => "Reveal";
    public override BaseKeybind Keybind => Keybinds.TertiaryAction;
    public override Color TextOutlineColor => RoleColors.Coven;
    public override float Cooldown => OptionGroupSingleton<PotionMaster_Options>.Instance.RevealCD;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.PotionMaster_Reveal;

    protected override void OnClick() => VisitingMechanic.CheckVisit(Player, Target, 3, false, true);
    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance, predicate: x =>
            !x.Is(Faction.Coven) && !x.HasModifier<RoleLearn>(x => x.Visitor == Player) && !x.HasModifier<GlobalReveal>());
    }
}

public sealed class PotionMaster_SelfBarrier : TownOfUsRoleButton<PotionMaster>
{
    public override string Name => "Self Barrier";
    public override BaseKeybind Keybind => Keybinds.ModifierAction;
    public override Color TextOutlineColor => RoleColors.Coven;
    public override float Cooldown => OptionGroupSingleton<PotionMaster_Options>.Instance.BarrierCD;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.PotionMaster_Barrier;
    public override ButtonLocation Location => ButtonLocation.BottomLeft;

    protected override void OnClick() => VisitingMechanic.CheckVisit(Player, Player, 2, false, false);
}

public sealed class PotionMaster_Options : AbstractOptionGroup<PotionMaster>
{
    public override string GroupName => "Potion Master";

    [ModdedNumberOption("Potion Master Barrier Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float BarrierCD { get; set; } = 25f;

    [ModdedNumberOption("Potion Master Reveal Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float RevealCD { get; set; } = 25f;
}