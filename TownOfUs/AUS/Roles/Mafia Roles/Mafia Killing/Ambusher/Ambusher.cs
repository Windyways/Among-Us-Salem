using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace AmongUsSalem.Roles;

public sealed class Ambusher(IntPtr cppPtr) : ImpostorRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Ambusher";
    public string revealText => "lies in wait.";
    public string RoleDescription => "Town Of Salem";
    public string RoleLongDescription => "You are a stealthy killer who lies in wait for the perfect moment to strike.";
    public Color RoleColor { get; set; } = RoleColors.Mafia;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;

    public Faction Faction { get; set; } = Faction.Mafia;
    public Alignment Alignment => Alignment.MafiaKilling;

    public Attack Attack { get; set; } = Attack.Basic;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.Basic;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;
    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = false,
        CanUseSabotage = false,
        Icon = AUSAssets.AmbusherRoleCard
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
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that kills players that visit its targets, being able to take down potential threats to the Mafia as well as being additional kill power.\n" +
            "Kill anyone that will not submit to the Mafia." + 
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Ambush",
            "You can Ambush a player at Night.\n" +
            "You will deal a Basic Attack to a player that visits your target, or reports your target's body.\n" +
            "All players visiting your target are aware of your identity.",
            AUSAssets.Ambusher_Ambush),
    ];

    public string GetAttributes()
    {
        return
            $"- If there are no capable Mafia Killing roles, you will be promoted to Mafioso.";
    }

    public static string Info(NotificationType type, PlayerControl ambusher, PlayerControl target)
    {
        if (type == NotificationType.Ambusher_FoundAmbusher) return $"You saw {ambusher.Name()} prepare an ambush while visiting {target.Name()}.";
        return $"You ambushed someone who visited {target.GetDefaultAppearance().PlayerName} last Night!";
    }

    [MethodRpc((uint)AUSRpc.RpcNotifyAmbusher)]
    public static void RpcNotify(PlayerControl player, int notifyType, PlayerControl ambusher, PlayerControl target)
    {
        if (player.AmOwner())
        {
            var notify = (NotificationType)notifyType;
            switch (notify)
            {
                case NotificationType.Ambusher_FoundAmbusher:
                    player.Notify(Info(notify, ambusher, target), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.AmbusherRoleCard.LoadAsset());
                    break;
                case NotificationType.Ambusher_Kill:
                    player.Notify(Info(notify, ambusher, target), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.AmbusherRoleCard.LoadAsset());
                    break;
            }
        }
    }

    public void RevealAmbusher(PlayerControl visitor, PlayerControl target)
    {
        RpcNotify(visitor, (int)NotificationType.Ambusher_FoundAmbusher, Player, target);
        if (!visitor.HasDied() && visitor.AmOwner())
        {
            Player.RpcAddModifier<RoleLearn>(visitor, false);
            Player.RpcAddModifier<ConfirmedEvil>();
        }
    }

    public void Function(PlayerControl target, int Button)
    {
        if (Button is 1)
        {
            target.RpcAddModifier<AmbushedModifier>(Player);
        }
    }
}

public sealed class Ambusher_Ambusher : TownOfUsRoleButton<Ambusher, PlayerControl>
{
    public override string Name => "Ambush";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Mafia;
    public override float Cooldown => OptionGroupSingleton<Ambusher_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Ambusher_Ambush;

    protected override void OnClick() => VisitingMechanic.CheckVisit(Player, Target, 1, false, true);
    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(false, Distance, predicate: x =>
            !x.HasModifier<AmbushedModifier>(x => x.Caster == Player));
    }
}

public sealed class Ambusher_Options : AbstractOptionGroup<Ambusher>
{
    public override string GroupName => "Ambusher";

    [ModdedNumberOption("Ambusher Ambush Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}