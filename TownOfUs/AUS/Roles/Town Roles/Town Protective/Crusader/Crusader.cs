using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;

namespace AmongUsSalem.Roles;

public sealed class Crusader(IntPtr cppPtr) : CrewmateRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Crusader";
    public string revealText => "is a divine protector.";
    public string RoleDescription => "";
    public string RoleLongDescription => "You are a divine protector skilled in the art of combat.";
    public Color RoleColor { get; set; } = RoleColors.Town;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction Faction { get; set; } = Faction.Town;
    public Alignment Alignment => Alignment.TownProtective;

    public Attack Attack { get; set; } = Attack.Basic;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.Basic;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.CrusaderRoleCard
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
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can grant defense to players and attack a visitor of its target, being good at taking down unharmful evil roles.\n" +
            "Hang every criminal and evildoer." +
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return
            $"- You know if your target is attacked.\n" +
            $"- Your target knows if they were attacked.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Fortify",
            "You can Fortify a player at Night.\n" +
            "You will grant your target Powerful Defense.\n" +
            "You will deal a Basic Attack to a player that visits your target, or reports your target’s body.",
            AUSAssets.Crusader_Fortify),
    ];

    public static string Info(NotificationType type, PlayerControl target)
    {
        if (type is NotificationType.Crusader_AttackAndFortified) return "Someone attacked you, but a Crusader protected you!";
        return $"You attacked someone visiting {target.Name()}.";
    }

    [MethodRpc((uint)AUSRpc.RpcNotifyCrusader)]
    public static void RpcNotify(PlayerControl player, int notifyType, PlayerControl target)
    {
        if (player.AmOwner())
        {
            var notify = (NotificationType)notifyType;
            switch (notify)
            {
                case NotificationType.Crusader_AttackAndFortified:
                    player.Notify(Info(notify, target), NotifyMode.OnlyMeeting, sprite: AUSAssets.CrusaderRoleCard.LoadAsset());
                    break;
                case NotificationType.Crusader_AttackedVisitor:
                    player.Notify(Info(notify, target), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.CrusaderRoleCard.LoadAsset());
                    break;
            }
        }
    }
}

public sealed class Crusader_Fortify : TownOfUsRoleButton<Crusader, PlayerControl>
{
    public override string Name => "Fortify";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Town;
    public override float Cooldown => OptionGroupSingleton<Crusader_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Crusader_Fortify;

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Target, false, true)) base.ClickHandler();
    }

    protected override void OnClick()
    {
        if (Target == null)
            return;

        Target.RpcAddModifier<FortifiedModifier>(Player);
        Target.RpcAddModifier<HideGainedDefense>();
        AttackDefenseMechanic.RpcApplyDefense(Target, Defense.Powerful);
    }

    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance, predicate: x => 
            !x.HasModifier<FortifiedModifier>(x => x.Caster == Player));
    }
}

public sealed class Crusader_Options : AbstractOptionGroup<Crusader>
{
    public override string GroupName => "Crusader";

    [ModdedNumberOption("Crusader Fortify Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}