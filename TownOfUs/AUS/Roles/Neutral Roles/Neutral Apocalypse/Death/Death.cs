using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;

namespace AmongUsSalem.Roles;

public sealed class Death(IntPtr cppPtr) : CovenRole(cppPtr), ICustomAURole, IWikiDiscoverable, ISpawnChange
{
    public string RoleName { get; set; } = "Death";
    public string revealText => "fills you with a sense of dread.";
    public string RoleDescription => "Town Of Salem 2";
    public string RoleLongDescription => "";
    public Color RoleColor { get; set; } = RoleColors.Apocalypse;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.Neutral;
    public Alignment Alignment => Alignment.NeutralApocalypse;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.Invincible;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.Invincible;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public bool NoSpawn => true;
    public CustomRoleConfiguration Configuration => new(this)
    {
        CanModifyChance = false,
        MaxRoleCount = 0,

        DefaultChance = 0,
        DefaultRoleCount = 0,

        CanUseSabotage = OptionGroupSingleton<ApocOptions>.Instance.CanSabotage,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        Icon = AUSAssets.DeathRoleCard
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
            $"{RoleName} is a {Alignment.ToSpacedString()} role that will kill everyone if they are not lynched, automatically resulting in an Apocalypse win.\n" +
            "Bring forth the Apocalypse, kill everyone in the town. You will win with other Neutral Apocalypse members." + 
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Armageddon ",
            "Perform Armageddon if you are alive when the sun sets.\n" +
            "This will kill all non-Apocalypse members.",
            AUSAssets.Death_Armageddon),
    ];

    public string GetAttributes()
    {
        return
            $"- All players are notified of Death’s arrival.";
    }

    [MethodRpc((uint)AUSRpc.RpcNotifyDeath)]
    public static void RpcNotify(PlayerControl player, int notifyType, bool warlock)
    {
        if (player.AmOwner())
        {
            var notify = (NotificationType)notifyType;
            switch (notify)
            {
                case NotificationType.Death_Reveal:
                    player.Notify(Info(warlock), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.DeathRoleCard.LoadAsset());
                    break;
            }
        }
    }

    public bool WinConditionMet() => ApocGameOver.WinConditionMet(this);
    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet() || ApocGameOver.AnyApocWon(gameOverReason);
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        RpcNotify(PlayerControl.LocalPlayer, (int)NotificationType.Death_Reveal, true);
    }

    public static string Info(bool warlock)
    {
        if (warlock) return $"Now the Warlock has become Death, Destroyer of Worlds and Horseman of the Apocalypse!";
        return $"Now Soul Collector has become Death, Destroyer of Worlds and Horseman of the Apocalypse!";
    }

    public void Role_OnRoundStart()
    {
        if (!Player.HasDied())
        {
            var nonApoc = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && !x.Is(Alignment.NeutralApocalypse)).ToList();
            foreach (var player in nonApoc)
            {
                Player.RpcCustomMurder(player, teleportMurderer: false);
                VisitingMechanic.RpcAddDeathReason(player, (int)DeathReasonShow.KilledByDeathHorsemanOfApocalypse);
            }
        }
    }
}