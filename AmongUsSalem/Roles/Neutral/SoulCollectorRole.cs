using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using ObjectWorkshop.Options.Roles.Neutral;
using ObjectWorkshop.Roles.Crewmate;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Roles.Neutral;

public sealed class SoulCollectorRole(IntPtr cppPtr)
    : NeutralRole(cppPtr), IOWRole, IDoomable, ICrewVariant
{
    public string revealText => "";
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<MediumRole>());
    public DoomableType DoomHintType => DoomableType.Death;
    public string RoleName => TouLocale.Get(TouNames.SoulCollector, "Soul Collector");
    public string RoleDescription => "Reap The Souls From Your Crewmates";
    public string RoleLongDescription => "Reap the souls of others, leaving behind a lasting image";
    public Color RoleColor => OWColors.SoulCollector;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<SoulCollectorOptions>.Instance.CanVent,
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Phantom),
        Icon = TouRoleIcons.SoulCollector,
        MaxRoleCount = 1,
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
        return IOWRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Neutral Killing role that takes the soul of players. Instead of leaving a body behind, they leave behind an soul-less decoy that looks identical to the reaped player, standing still."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Reap",
            "Reaping acts like a kill button, but instead of making a dead body, makes a fake player in its place, appearing alive.",
            TouNeutAssets.ReapSprite)
    ];

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TouNeutAssets.ReaperVentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(OWColors.SoulCollector);
        }
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TouAssets.VentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(OWColors.Infiltrator);
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