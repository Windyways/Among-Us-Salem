using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;

namespace AmongUsSalem.Roles;

public sealed class Jinx(IntPtr cppPtr) : CovenRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Jinx";
    public string revealText => "lies in wait.";
    public string RoleDescription => "Town Of Salem 2";
    public string RoleLongDescription => "You are a bad omen who causes harm to those around you.";
    public Color RoleColor { get; set; } = RoleColors.Coven;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.Coven;
    public Alignment Alignment => Alignment.CovenKilling;

    public Attack Attack { get; set; } = Attack.Basic;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.Basic;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;
    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseSabotage = OptionGroupSingleton<CovenOptions>.Instance.CanSabotage,
        CanUseVent = OptionGroupSingleton<CovenOptions>.Instance.CanVent,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        Icon = AUSAssets.JinxRoleCard
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
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that kills players that visit its targets, being able to take down potential threats to the Coven as well as being additional kill power.\n" +
            "Kill all who would oppose the Coven." + 
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return
            $"- With the Necronomicon, you will also deal a Basic Attack to your targets.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Jinx",
            "You can Jinx a player at Night.\n" +
            "You will deal a Basic Attack to a player that visits your target, or reports your target's body.\n" +
            "All players visiting your target are aware of your identity.",
            AUSAssets.Jinx_Jinx),
    ];

    public bool WinConditionMet() => CovenGameOver.WinConditionMet(this);
    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet() || CovenGameOver.AnyCovenWon(gameOverReason);
    }

    public static string Info(NotificationType type, PlayerControl jinx, PlayerControl target)
    {
        if (type == NotificationType.Jinx_FoundJinx) return $"You saw the Jinx {jinx.GetDefaultAppearance().PlayerName} visit {target.GetDefaultAppearance().PlayerName}!";
        return $"You jinxed someone who visited {target.GetDefaultAppearance().PlayerName}!";
    }

    [MethodRpc((uint)AUSRpc.RpcNotifyJinx)]
    public static void RpcNotify(PlayerControl player, int notifyType, PlayerControl jinx, PlayerControl target)
    {
        if (player.AmOwner())
        {
            var notify = (NotificationType)notifyType;
            switch (notify)
            {
                case NotificationType.Jinx_FoundJinx:
                    player.Notify(Info(notify, jinx, target), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.JinxRoleCard.LoadAsset());
                    break;
                case NotificationType.Jinx_JinxedVisitor:
                    player.Notify(Info(notify, jinx, target), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.JinxRoleCard.LoadAsset());
                    break;
            }
        }
    }

    public void RevealJinx(PlayerControl visitor, PlayerControl target)
    {
        RpcNotify(visitor, (int)NotificationType.Jinx_FoundJinx, Player, target);
        if (!visitor.HasDied() && visitor.AmOwner())
        {
            Player.RpcAddModifier<RoleLearn>(visitor, false);
            Player.RpcAddModifier<ConfirmedEvil>();
        }
    }
}

public sealed class Jinx_Jinx : TownOfUsRoleButton<Jinx, PlayerControl>, IButtonClick
{
    public override string Name => "Jinx";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Coven;
    public override float Cooldown => Player.HasNecronomicon() ? OptionGroupSingleton<CovenOptions>.Instance.Cooldown : 
        OptionGroupSingleton<Jinx_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Jinx_Jinx;

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Target, Player.HasNecronomicon(), true)) base.ClickHandler();
    }

    public override PlayerControl? GetTarget()
    {
        if (Player.HasNecronomicon())
            return Player.GetClosestLivingPlayer(true, Distance, predicate: x => 
                !x.Is(Faction.Coven));

        return Player.GetClosestLivingPlayer(true, Distance, predicate: x => 
            !x.Is(Faction.Coven) && !x.HasModifier<JinxedModifier>(x => x.Caster == Player));
    }

    protected override void OnClick() => Click(Player, Target);
    public void Click(PlayerControl player, PlayerControl Target = null)
    {
        if (Target == null)
            return;

        Target.RpcAddModifier<JinxedModifier>(Player);
        if (Player.HasNecronomicon())
        {
            if (Player.CanKill(Target))
            {
                Player.RpcCustomMurder(Target);
                VisitingMechanic.RpcAddDeathReason(Target, (int)DeathReasonShow.KilledByTheCoven);
            }
            else Player.Notify(Feedback.TooMuchDefense(Player, Target), NotifyMode.InstantlyAndMeeting);
        }
    }
}

public sealed class Jinx_Options : AbstractOptionGroup<Jinx>
{
    public override string GroupName => "Jinx";

    [ModdedNumberOption("Jinx Jinx Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}