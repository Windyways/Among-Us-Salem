using System.Text;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace ObjectWorkshop.LifeImprovement.Roles;

#region Aimsman
#endregion
public sealed class Aimsman(IntPtr cppPtr)
    : ImpostorRole(cppPtr), IOWRole, IWikiDiscoverable, IVisualAppearance
{
    public bool attractsMetal => true;
    public string RoleName => TouLocale.Get(TouNames.Aimsman, "Aimsman");
    public string revealText => "is a corrupt huntsman.";
    public string RoleDescription => "Shoot players down remotely.";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => OWColors.Infiltrator;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.InfiltratorGunsman;

    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = false,
        Icon = OWAssets.Aimsman,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IOWRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Infiltrator Gunsman role that go into Aim mode, and can Fire to kill their respective target."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Aim",
            "You can begin an Aim during the round. You will enter Target Mode, visually showing a target to all players, which starts at the middle of your screen. Using this ability again while in Target Mode will turn you back to normal.",
            OWAssets.Aimsman_Aim),

        new("Fire",
            $"You can Fire at a target during the round. The player within your target will die, and you will exit Target Mode 1 second later. You may shoot people in vents.",
            OWAssets.Aimsman_Fire),
    ];

    public override void OnMeetingStart()
    {
		Crosshair.DestroyAll();
    }

    public void OnDeath(DeathReason? reason)
    {
        if (reason != null && AmongUsClient.Instance.AmHost && isAiming)
        {
            var crosshair = Crosshair.GetObjectByPlayer(Player);
            crosshair.DestroyGameObject();
		}
    }

    public void LobbyStart()
    {
        Crosshair.CleanUp();
    }

    #region RpcFire
    #endregion
    [MethodRpc((uint)ObjectWorkshopRpc.Fire, SendImmediately = true)]
    public static void RpcFire(PlayerControl player)
    {
        if (player.Data.Role is not Aimsman)
        {
            Logger<ObjectWorkshopPlugin>.Error("RpcFire - Invalid Aimsman");
            return;
        }

        var aimsman = player.GetRole<Aimsman>();
        if (aimsman.isAiming)
        {
            var crosshair = Crosshair.GetObjectByPlayer(aimsman.Player);
            crosshair.TryShoot(aimsman, null);
        }
    }

    #region RpcAim
    #endregion
    [MethodRpc((uint)ObjectWorkshopRpc.Aim, SendImmediately = true)]
    public static void RpcAim(PlayerControl player)
    {
        if (player.Data.Role is not Aimsman)
        {
            Logger<ObjectWorkshopPlugin>.Error("RpcAim - Invalid Aimsman");
            return;
        }

        var aimsman = player.GetRole<Aimsman>();

        if (!aimsman.isAiming)
        {
            WitnessKillEvent.WitnessSuspiciousActivity(aimsman.Player, aimsman.Player, false, false, true, true);

            aimsman.isAiming = true;
            Crosshair.Begin(aimsman.Player);
        }
        else
        {
            var crosshair = Crosshair.GetObjectByPlayer(aimsman.Player);
            crosshair.DestroyGameObject();
            aimsman.isAiming = false;

            if (aimsman.Player.AmOwner)
            {
                var button = CustomButtonSingleton<Aimsman_Aim>.Instance;
                button.EffectActive = false;
                button.Timer = OptionGroupSingleton<Aimsman_Options>.Instance.Cooldown;
            }
        }
    }

    #region RpcMoveCrosshair
    #endregion
    [MethodRpc((uint)ObjectWorkshopRpc.MoveCrosshair, SendImmediately = true)]
    public static void RpcMoveCrosshair(PlayerControl player)
    {
        if (player.Data.Role is not Aimsman)
        {
            Logger<ObjectWorkshopPlugin>.Error("RpcMoveCrosshair - Invalid Aimsman");
            return;
        }

        var aimsman = player.GetRole<Aimsman>();
        var crosshair = Crosshair.GetObjectByPlayer(aimsman.Player);

        /*if (Shield.IsInRange(gameObject) || isChronoNullified) For Chronoguard!!!
        {
            if (!isChronoNullified)
            {
                Coroutines.Start(Shield.DestroyCrosshair(aimsman));
            }
            isChronoNullified = true;
            moveSpeed = 0f;
        }*/

        Vector2 move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        crosshair.targetPos += move * crosshair.moveSpeed * Time.deltaTime;
        crosshair.gameObject.transform.position = crosshair.targetPos;
    }

    #region RpcDestroyCrosshair
    #endregion
    [MethodRpc((uint)ObjectWorkshopRpc.DestroyCrosshair, SendImmediately = true)]
    public static void RpcDestroyCrosshair(PlayerControl player)
    {
        if (player.Data.Role is not Aimsman)
        {
            Logger<ObjectWorkshopPlugin>.Error("RpcDestroyCrosshair - Invalid Aimsman");
            return;
        }

        var aimsman = player.GetRole<Aimsman>();
        var crosshair = Crosshair.GetObjectByPlayer(aimsman.Player);
        crosshair.DestroyGameObject();
    }

    public VisualAppearance GetVisualAppearance()
    {
        var appearance = Player.GetDefaultAppearance();
        if (!isAiming) return appearance;

        appearance.Speed = 0;
        return appearance;
    }

    public bool isAiming;
}

#region Aimsman_Aim
#endregion
public sealed class Aimsman_Aim : ObjectWorkshopRoleButton<Aimsman>
{
    public override string Name => "Aim";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Infiltrator;
    public override float Cooldown => OptionGroupSingleton<Aimsman_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => OWAssets.Aimsman_Aim;
    public override float EffectDuration => OptionGroupSingleton<Aimsman_Options>.Instance.Duration;

    public override bool CanClick()
    {
        return (Timer <= 0 && CanUse()) || EffectActive;
    }

    protected override void OnClick()
    {
        Aimsman.RpcAim(Role.Player);
    }

    public override void OnEffectEnd()
    {
        var crosshair = Crosshair.GetObjectByPlayer(Role.Player);
        crosshair.DestroyGameObject();
        Role.isAiming = false;
    }
}

#region Aimsman_Fire
#endregion
public sealed class Aimsman_Fire : ObjectWorkshopRoleButton<Aimsman>
{
    public override string Name => "Fire";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => OWColors.Infiltrator;
    public override float Cooldown => OptionGroupSingleton<Aimsman_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => OWAssets.Aimsman_Fire;

    public override bool CanUse()
    {
        var crosshair = Crosshair.GetObjectByPlayer(PlayerControl.LocalPlayer);
        if (crosshair == null) return false;
        return base.CanUse() && crosshair.currentTarget != null && Role.isAiming;
    }

    protected override void OnClick()
    {
        Aimsman.RpcFire(Role.Player);
    }
}

#region Aimsman_Options
#endregion
public sealed class Aimsman_Options : AbstractOptionGroup<Aimsman>
{
    public override string GroupName => TouLocale.Get(TouNames.Aimsman, "Aimsman");

    [ModdedNumberOption("<color=#ff5050>Aimsman</color> <color=#4a86e8>Aim</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("<color=#ff5050>Aimsman</color> <color=#4a86e8>Aim</color> Duration", 0.5f, 60f, 0.5f, MiraNumberSuffixes.Seconds, "0.0")]
    public float Duration { get; set; } = 30f;

    [ModdedToggleOption("Players On Ladder Are Immune")]
    public bool LadderImmunity { get; set; } = true;
}