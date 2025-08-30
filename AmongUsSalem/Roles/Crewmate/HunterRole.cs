using System.Globalization;
using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Roles;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using ObjectWorkshop.Buttons.Crewmate;
using ObjectWorkshop.Modifiers.Crewmate;
using ObjectWorkshop.Options.Roles.Crewmate;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Roles.Crewmate;

public sealed class HunterRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITouCrewRole, IDoomable
{
    public string revealText => "";
    public override bool IsAffectedByComms => false;

    public PlayerControl? LastVoted { get; set; }

    [HideFromIl2Cpp] public List<PlayerControl> CaughtPlayers { get; } = [];

    public DoomableType DoomHintType => DoomableType.Hunter;
    public string RoleName => TouLocale.Get(TouNames.Hunter, "Hunter");
    public string RoleDescription => "Stalk The <color=#FF0000FF>Impostor</color>";
    public string RoleLongDescription => "Stalk player interactions and kill impostors, but not Crewmates";
    public Color RoleColor => OWColors.Hunter;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.None;

    public bool IsPowerCrew =>
        CaughtPlayers.Any(x => !x.HasDied()); // Disable end game checks if a Hunter has alive targets

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TouRoleIcons.Hunter,
        IntroSound = TouAudio.OtherIntroSound
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = IOWRole.SetNewTabText(this);
        var stalkedPlayer = ModifierUtils.GetPlayersWithModifier<HunterStalkedModifier>(x => x.Hunter.AmOwner)
            .FirstOrDefault();
        var stalked = stalkedPlayer != null && !stalkedPlayer.HasDied() ? stalkedPlayer.Data.PlayerName : "Nobody";
        stringB.AppendLine(CultureInfo.InvariantCulture, $"Stalking: <b>{stalked}</b>");
        if (CaughtPlayers.Count != 0)
        {
            stringB.AppendLine(CultureInfo.InvariantCulture, $"<b>Caught Players:</b>");
        }

        foreach (var player in CaughtPlayers)
        {
            var newText = $"<b><size=80%>{player.Data.PlayerName}</size></b>";
            stringB.AppendLine(CultureInfo.InvariantCulture, $"{newText}");
        }

        return stringB;
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Crewmate Killing role that can stalk players during the round. "
            + "If a stalked player uses an ability, they can be killed by the Hunter at any point in the game, even Crew."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Stalk",
            $"Choose a target to stalk. You can stalk {OptionGroupSingleton<HunterOptions>.Instance.StalkUses} players. " +
            $"If they use any ability while stalked, they’re added to your hitlist and can be killed.",
            TouCrewAssets.StalkButtonSprite)
    ];

    [MethodRpc((uint)ObjectWorkshopRpc.CatchPlayer, SendImmediately = true)]
    public static void RpcCatchPlayer(PlayerControl hunter, PlayerControl source)
    {
        if (hunter.Data.Role is not HunterRole role)
        {
            Logger<ObjectWorkshopPlugin>.Error("RpcCatchPlayer - Invalid hunter");
            return;
        }

        if (!role.CaughtPlayers.Contains(source))
        {
            role.CaughtPlayers.Add(source);

            if (hunter.AmOwner)
            {
                Coroutines.Start(MiscUtils.CoFlash(OWColors.Hunter));

                CustomButtonSingleton<HunterStalkButton>.Instance.ResetCooldownAndOrEffect();
            }
        }
    }

    public static void Retribution(PlayerControl hunter, PlayerControl target)
    {
        if (hunter.Data.Role is not HunterRole)
        {
            Logger<ObjectWorkshopPlugin>.Error("RpcCatchPlayer - Invalid hunter");
            return;
        }

        if (hunter.AmOwner)
        {
            hunter.RpcCustomMurder(target, resetKillTimer: false, createDeadBody: false, teleportMurderer: false,
                showKillAnim: false, playKillSound: false);
        }

        // this sound normally plays on the source only
        if (!hunter.AmOwner)
        {
            SoundManager.Instance.PlaySound(hunter.KillSfx, false, 0.8f);
        }

        // this kill animations normally plays on the target only
        if (!target.AmOwner)
        {
            HudManager.Instance.KillOverlay.ShowKillAnimation(hunter.Data, target.Data);
        }
    }
}