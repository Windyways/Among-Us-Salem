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

public sealed class WerewolfRole(IntPtr cppPtr)
    : NeutralRole(cppPtr), IOWRole, IDoomable, ICrewVariant
{
    public string revealText => "";
    public bool Rampaging { get; set; }
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<HunterRole>());
    public DoomableType DoomHintType => DoomableType.Hunter;
    public string RoleName => TouLocale.Get(TouNames.Werewolf, "Werewolf");
    public string RoleDescription => "Rampage To Kill Everyone";
    public string RoleLongDescription => "Rampage to kill everyone in your path";
    public Color RoleColor => OWColors.Werewolf;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<WerewolfOptions>.Instance.CanVent /* && (Rampaging || Player.inVent)*/,
        IntroSound = TouAudio.WerewolfRampageSound,
        Icon = TouRoleIcons.Werewolf,
        MaxRoleCount = 1,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>()
    };

    public bool HasImpostorVision => Rampaging;

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
            $"The {RoleName} is a Neutral Killing role that wins by being the last killer alive. They can go on a rampage to gain the ability to kill." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Rampage",
            "Go on a Rampage, gaining the ability to kill and gain Impostor vision. During the Rampage your kill cooldown is significantly lower.",
            TouNeutAssets.RampageSprite)
    ];

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TouNeutAssets.WerewolfVentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(OWColors.Werewolf);
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

    public override bool CanUse(IUsable usable)
    {
        if (!GameManager.Instance.LogicUsables.CanUse(usable, Player))
        {
            return false;
        }

        var console = usable.TryCast<Console>()!;
        return console == null || console.AllowImpostor;
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet();
    }
}