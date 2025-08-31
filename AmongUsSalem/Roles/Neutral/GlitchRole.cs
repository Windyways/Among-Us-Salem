using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using AmongUsSalem.Options.Roles.Neutral;
using AmongUsSalem.Roles.Crewmate;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Roles.Neutral;

public sealed class GlitchRole(IntPtr cppPtr)
    : NeutralRole(cppPtr), IAUSRole, IDoomable, ICrewVariant
{
    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;
    public DeathReasonShow deathReasonShow { get; set; } = DeathReasonShow.Alive;
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<SheriffRole>());
    public DoomableType DoomHintType => DoomableType.Perception;
    public string RoleName => TouLocale.Get(TouNames.Glitch, "Glitch");
    public string RoleDescription => "Murder, Mimic, Hack... Data Lost";
    public string RoleLongDescription => "Murder everyone to win with your abilities!";
    public Color RoleColor => AUSColors.Glitch;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public Alignment Alignment => Alignment.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<GlitchOptions>.Instance.CanVent,
        IntroSound = TouAudio.GlitchSound,
        MaxRoleCount = 1,
        Icon = TouRoleIcons.Glitch,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>()
    };

    public bool HasImpostorVision => true;

    public bool WinConditionMet()
    {
        if (Player.HasDied())
        {
            return false;
        }

        var result = Helpers.GetAlivePlayers().Count <= 2 && MiscUtils.KillersAliveCount() == 1;

        return result;
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IAUSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Neutral Killing role that wins by being the last killer alive. They can Mimic into another player or they can hack a player." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Mimic",
            "Mimic the appearance of another player, taking on their whole look.",
            TouNeutAssets.MimicSprite),
        new("Hack",
            "Disable a player's abilities.",
            TouNeutAssets.HackSprite)
    ];

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TouNeutAssets.GlitchVentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(AUSColors.Glitch);
        }
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TouAssets.VentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(AUSColors.Mafia);
        }
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet();
    }

    public override bool CanUse(IUsable usable)
    {
        if (!GameManager.Instance.LogicUsables.CanUse(usable, Player))
        {
            return false;
        }

        var console = usable.TryCast<Console>()!;
        return console == null || console.AllowImpostor;
    }
}