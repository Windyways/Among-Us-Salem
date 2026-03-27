using Il2CppInterop.Runtime.Attributes;
using System.Collections;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace AmongUsSalem.Roles;

public sealed class Veteran(IntPtr cppPtr) : CrewmateRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Veteran";
    public string revealText => "is a paranoid war hero.";
    public string RoleDescription => "Town Of Salem 2";
    public string RoleLongDescription => "You are a war hero who will shoot intruders on sight.";
    public Color RoleColor { get; set; } = RoleColors.Town;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction Faction { get; set; } = Faction.Town;
    public Alignment Alignment => Alignment.TownKilling;

    public Attack Attack { get; set; } = Attack.Powerful;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.Powerful;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.VeteranRoleCard
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var info = ICustomAURole.SetNewTabText(this);
        return info;
    }

    public string GetAdvancedDescription()
    {
        return
            $"Attack: {Attack}\n" +
            $"Defense: {Defense}\n" +
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can bait evils into visiting them to counterattack them, as well as having defense against them.\n" +
            "Hang every criminal and evildoer." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Alert",
            "You can go on Alert at Night.\n" +
            "You have Basic Defense while on Alert.\n" +
            "You will deal a Powerful Attack to all players visiting you or your body.",
            AUSAssets.Veteran_Alert),
    ];

    public static string Info(NotificationType type)
    {
        if (type == NotificationType.Veteran_Shot) return $"You shot someone who visited you last night!";
        if (type == NotificationType.Veteran_Kill) return $"You were killed by the Veteran you visited!";
        if (type == NotificationType.Veteran_TT) return $"You have lost any remaining Alerts while you hunt for the Town Traitor.";
        return $"Someone attacked you, but your defense while on Alert was too high!";
    }

    [MethodRpc((uint)AUSRpc.RpcAlert)]
    public static void RpcAlert(PlayerControl player)
    {
        if (player.Data.Role is not Veteran)
        {
            Logger<AUSPlugin>.Error("RpcAlert - Invalid Veteran");
            return;
        }

        var veteran = player.GetRole<Veteran>();
        veteran.isAlerted = true;
    }

    [MethodRpc((uint)AUSRpc.RpcNotifyVeteran)]
    public static void RpcNotify(PlayerControl player, int notifyType, bool instant = false)
    {
        if (player.AmOwner())
        {
            var notify = (NotificationType)notifyType;

            if (instant) player.Notify(Info(notify), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.VeteranRoleCard.LoadAsset());
            else player.Notify(Info(notify), NotifyMode.OnlyMeeting, sprite: AUSAssets.VeteranRoleCard.LoadAsset());
        }
    }

    public bool isAlerted;
    public void Role_OnMeetingStart()
    {
        isAlerted = false;
    }

    public int PerformInteraction(PlayerControl visitor, bool isAttacking)
    {
        if (Player.CanKill(visitor))
        {
            RpcNotify(visitor, (int)NotificationType.Veteran_Shot, true);
            if (isAttacking) RpcNotify(visitor, (int)NotificationType.Veteran_Attacked, true);

            visitor.Notify(Info(NotificationType.Veteran_Kill), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.VeteranRoleCard.LoadAsset());
            Player.RpcCustomMurder(visitor);
            VisitingMechanic.RpcAddDeathReason(visitor, (int)DeathReasonShow.ShotByAVeteran);

            Player.AddModifier<Confirmed>();
        }
        else
        {
            RpcNotify(visitor, (int)NotificationType.Veteran_Shot);
            if (isAttacking) RpcNotify(visitor, (int)NotificationType.Veteran_Attacked, true);
        }

        return 0;
    }
}

public sealed class Veteran_Alert : TownOfUsRoleButton<Veteran>, IButtonClick
{
    public override string Name => "Alert";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Town;
    public override float Cooldown => OptionGroupSingleton<Veteran_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Veteran_Alert;
    public override int MaxUses => (int)OptionGroupSingleton<Veteran_Options>.Instance.Charges;

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Player, false, false)) base.ClickHandler();
    }

    public override bool CanUse()
    {
        return base.CanUse() && !Role.isAlerted;
    }

    protected override void OnClick() => Click(Player);
    public void Click(PlayerControl player, PlayerControl Target = null)
    {
        Veteran.RpcAlert(Player);
        AttackDefenseMechanic.RpcApplyDefense(Player, Defense.Basic, visualize: true);
    }
}

public sealed class Veteran_Options : AbstractOptionGroup<Veteran>
{
    public override string GroupName => "Veteran";

    [ModdedNumberOption("Veteran Alert Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("Veteran Max Alerts", 0f, 30f, 1f, MiraNumberSuffixes.None, zeroInfinity: true)]
    public float Charges { get; set; } = 3;
}