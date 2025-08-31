using System.Globalization;
using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using AmongUsSalem.Modifiers.Crewmate;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Roles.Crewmate;

public sealed class WardenRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITOURole, IDoomable
{
    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;
    public DeathReasonShow deathReasonShow { get; set; } = DeathReasonShow.Alive;
    public string revealText => "";
    public override bool IsAffectedByComms => false;

    public PlayerControl? Fortified { get; set; }

    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not WardenRole)
        {
            return;
        }

        if (Fortified != null && Fortified.HasDied())
        {
            Clear();
        }
    }

    public DoomableType DoomHintType => DoomableType.Protective;
    public string RoleName => TouLocale.Get(TouNames.Warden, "Warden");
    public string RoleDescription => "Fortify Crewmates";
    public string RoleLongDescription => "Fortify crewmates to prevent interactions with them";
    public Color RoleColor => AUSColors.Warden;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public Alignment Alignment => Alignment.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TouAudio.SpyIntroSound,
        Icon = TouRoleIcons.Warden
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = IAUSRole.SetNewTabText(this);

        if (Fortified != null)
        {
            stringB.Append(CultureInfo.InvariantCulture,
                $"\n<b>Fortified: </b>{Color.white.ToTextColor()}{Fortified.Data.PlayerName}</color>");
        }

        return stringB;
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Crewmate Protective role that can fortify players to prevent them from being interacted with. "
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Fortify",
            "Fortify a player to prevent them from being interacted with. If anyone tries to interact with a fortified player, the ability will not work and both the Warden and fortified player will be alerted with a purple flash.",
            TouCrewAssets.FortifySprite)
    ];

    public void Clear()
    {
        SetFortifiedPlayer(null);
    }

    public override void OnDeath(DeathReason reason)
    {
        RoleBehaviourStubs.OnDeath(this, reason);

        Clear();
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);

        Clear();
    }

    public void SetFortifiedPlayer(PlayerControl? player)
    {
        Fortified?.RemoveModifier<WardenFortifiedModifier>();

        Fortified = player;

        Fortified?.AddModifier<WardenFortifiedModifier>(Player);
    }

    [MethodRpc((uint)AUSRpc.WardenFortify, SendImmediately = true)]
    public static void RpcWardenFortify(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not WardenRole)
        {
            Logger<AUSPlugin>.Error("RpcWardenFortify - Invalid warden");
            return;
        }

        var warden = player.GetRole<WardenRole>();
        warden?.SetFortifiedPlayer(target);
    }

    [MethodRpc((uint)AUSRpc.ClearWardenFortify, SendImmediately = true)]
    public static void RpcClearWardenFortify(PlayerControl player)
    {
        if (player.Data.Role is not WardenRole)
        {
            Logger<AUSPlugin>.Error("RpcClearWardenFortify - Invalid warden");
            return;
        }

        var warden = player.GetRole<WardenRole>();
        warden?.SetFortifiedPlayer(null);
    }

    [MethodRpc((uint)AUSRpc.WardenNotify, SendImmediately = true)]
    public static void RpcWardenNotify(PlayerControl player, PlayerControl source, PlayerControl target)
    {
        if (player.Data.Role is not WardenRole)
        {
            Logger<AUSPlugin>.Error("RpcWardenNotify - Invalid warden");
            return;
        }

        // Logger<AUSPlugin>.Error("RpcWardenNotify");
        if (player.AmOwner)
        {
            Coroutines.Start(MiscUtils.CoFlash(AUSColors.Warden));
        }

        if (source.AmOwner)
        {
            Coroutines.Start(MiscUtils.CoFlash(AUSColors.Warden));
        }
    }
}