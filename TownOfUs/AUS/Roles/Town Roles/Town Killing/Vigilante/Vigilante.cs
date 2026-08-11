using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace AmongUsSalem.Roles;

public sealed class Vigilante(IntPtr cppPtr) : CrewmateRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Vigilante";
    public string revealText => "will bend the law to enact justice.";
    public string RoleDescription => "Town Of Salem 2";
    public string RoleLongDescription => "You are a townie taking justice into your own hands.";
    public Color RoleColor { get; set; } = RoleColors.Town;//{ get => FlexibleFactions.GetNewFaction(OptionGroupSingleton<Vigilante_Options>.Instance.faction.Value).Item2; set { } }
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;// FlexibleFactions.GetNewFaction(OptionGroupSingleton<Vigilante_Options>.Instance.faction.Value).Item3;

    public Faction Faction { get; set; } = Faction.Town;// FlexibleFactions.GetNewFaction(OptionGroupSingleton<Vigilante_Options>.Instance.faction.Value).Item1;
    public Alignment Alignment => Alignment.TownKilling;

    public Attack Attack { get; set; } = Attack.Basic;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.Basic;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.VigilanteRoleCard
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
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can shoot players at Night to attack them, not shooting will reload a bullet into your gun, allowing you to shoot. Killing a Town member has its consequences.\n" +
            "Hang every criminal and evildoer." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Shoot",
            "You can Shoot a player at Night.\n" +
            "You will deal a Basic Attack to your target.\n" +
            "If you kill a Town member, you will lose all remaining charges of Shoot.\n" +
            "If you do not Shoot at Night, you will instead reload a bullet at Day. You can only Shoot equal to the amount of reloaded bullets.",
            AUSAssets.Vigilante_Shoot),
    ];

    public static string Info(NotificationType type, PlayerControl? player)
    {
        if (type == NotificationType.Vigilante_Kill) return $"You were killed by a Vigilante!";
        if (type == NotificationType.Vigilante_KillTown) return $"You have put your gun away for accidentally killing a Town member!";
        if (type == NotificationType.Vigilante_Reload) return $"You have Reloaded a bullet into your gun!";

        if (player != null && player.GetRoleWhenAlive() is Vigilante vigilante)
        {
            if (vigilante.Charges == 0) return $"You have no unloaded bullets remaining.";
            if (vigilante.Charges == 1) return $"You have 1 unloaded bullet remaining.";
            return $"You have {vigilante.Charges} unloaded bullets remaining.";
        }

        return "Error.";
    }

    [MethodRpc((uint)AUSRpc.RpcNotifyVigilante)]
    public static void RpcNotify(PlayerControl player, int notifyType)
    {
        if (player.AmOwner())
        {
            var notify = (NotificationType)notifyType;
            player.Notify(Info(notify, null), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.VigilanteRoleCard.LoadAsset());
        }
    }

    public void Role_OnMeetingStart()
    {
        if (Player.AmOwner() && !Player.HasDied() && DayNightMechanic.DayCount > 1)
        {
            if (!shotTonight && Charges > 0)
            {
                bulletsReloaded++;
                Charges--;
                Player.Notify(Info(NotificationType.Vigilante_Reload, Player), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.VigilanteRoleCard.LoadAsset());
            }
        }

        shotTonight = false;
    }

    public int bulletsReloaded;
    public int Charges = (int)OptionGroupSingleton<Vigilante_Options>.Instance.Charges;
    public bool shotTonight;
    public void Function(PlayerControl target, int Button)
    {
        if (Button is 1)
        {
            shotTonight = true;
            bulletsReloaded--;
            if (Player.CanKill(target))
            {
                Player.RpcCustomMurder(target);
                VisitingMechanic.RpcAddDeathReason(target, (int)DeathReasonShow.ShotByAVigilante);
                RpcNotify(target, (int)NotificationType.Vigilante_Kill);

                if (target.Is(Faction.Town) && Player.Is(Faction.Town))
                {
                    bulletsReloaded = 0;
                    Charges = 0;
                    Player.Notify(Info(NotificationType.Vigilante_KillTown, Player), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.VigilanteRoleCard.LoadAsset());
                }
            }
            else Player.Notify(Feedback.TooMuchDefense(Player, target), NotifyMode.InstantlyAndMeeting);
        }
    }
}

public sealed class Vigilante_Shoot : TownOfUsRoleButton<Vigilante, PlayerControl>
{
    public override string Name => "Shoot";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Town;
    public override float Cooldown => OptionGroupSingleton<Vigilante_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Vigilante_Shoot;

    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);
        if (KeybindIcon != null)
        {
            KeybindIcon.transform.localPosition = new Vector3(0.4f, 0.45f, -9f);
        }
    }

    protected override void FixedUpdate(PlayerControl playerControl)
    {
        if (playerControl.IsRole<Vigilante>())
        {
            var role = playerControl.GetRole<Vigilante>();
            Button?.usesRemainingText.gameObject.SetActive(true);
            Button?.usesRemainingSprite.gameObject.SetActive(true);

            if (role.bulletsReloaded == 0 && role.Charges == 0) Button!.usesRemainingText.text = $"0";
            else Button!.usesRemainingText.text = role.bulletsReloaded.ToString() + $"/{role.Charges}";
        }

        base.FixedUpdate(playerControl);
    }

    public override bool CanUse()
    {
        return base.CanUse() && Role.bulletsReloaded > 0;
    }

    protected override void OnClick() => VisitingMechanic.CheckVisit(Player, Target, 1, true, true);
    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance);
    }
}

public sealed class Vigilante_Options : AbstractOptionGroup<Vigilante>
{
    public override string GroupName => "Vigilante";
    /*public ModdedEnumOption faction { get; set; } = new("[FLEXIBLE FACTIONS] Vigilante Faction",
        (int)Faction.Town, typeof(Faction), ["Town", "Coven", "Apocalypse"]);
    */
    [ModdedNumberOption("Vigilante Shoot Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("Vigilante Max Shots", 0f, 30f, 1f, MiraNumberSuffixes.None, zeroInfinity: true)]
    public float Charges { get; set; } = 3;
}