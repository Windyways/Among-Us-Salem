using System.Globalization;
using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using ObjectWorkshop.Events.TouEvents;
using ObjectWorkshop.Modifiers.Game.Neutral;
using ObjectWorkshop.Modifiers.Neutral;
using ObjectWorkshop.Options.Roles.Neutral;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Roles.Neutral;

public sealed class VampireRole(IntPtr cppPtr) : NeutralRole(cppPtr), IOWRole, IDoomable
{
    public string revealText => "";
    public DoomableType DoomHintType => DoomableType.Death;
    public string RoleName => TouLocale.Get(TouNames.Vampire, "Vampire");
    public string RoleDescription => "Convert Crewmates And Kill The Rest";
    public string RoleLongDescription => "Bite all other players";
    public Color RoleColor => OWColors.Vampire;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<VampireOptions>.Instance.CanVent,
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Phantom),
        Icon = TouRoleIcons.Vampire,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        MaxRoleCount = 1
    };

    public bool HasImpostorVision => OptionGroupSingleton<VampireOptions>.Instance.HasVision;

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var alignment = RoleAlignment.ToDisplayString().Replace("Neutral", "<color=#8A8A8AFF>Neutral");

        var stringB = new StringBuilder();
        stringB.AppendLine(CultureInfo.InvariantCulture,
            $"{RoleColor.ToTextColor()}You are a<b> {RoleName}.</b></color>");
        stringB.AppendLine(CultureInfo.InvariantCulture, $"<size=60%>Alignment: <b>{alignment}</color></b></size>");
        stringB.Append("<size=70%>");
        stringB.AppendLine(CultureInfo.InvariantCulture, $"{RoleLongDescription}");

        return stringB;
    }

    public bool WinConditionMet()
    {
        var vampireCount = CustomRoleUtils.GetActiveRolesOfType<VampireRole>().Count(x => !x.Player.HasDied());

        if (MiscUtils.KillersAliveCount() > vampireCount)
        {
            return false;
        }

        return vampireCount >= Helpers.GetAlivePlayers().Count - vampireCount;
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Bite",
            "Bite a player. If the bitten player is a Crewmate and you have not exceeded the maximum amount of vampires in a game yet. You convert them into a vampire. Otherwise they just get killed.",
            TouNeutAssets.BiteSprite)
    ];

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Neutral Killing role that wins by being the last killer(s) alive. They can bite, changing others into Vampires, or kill players." +
            MiscUtils.AppendOptionsText(GetType());
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TouNeutAssets.VampVentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(OWColors.Vampire);
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

    [MethodRpc((uint)ObjectWorkshopRpc.VampireBite, SendImmediately = true)]
    public static void RpcVampireBite(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not VampireRole)
        {
            Logger<ObjectWorkshopPlugin>.Error("RpcVampireBite - Invalid vampire");
            return;
        }

        var touAbilityEvent = new TouAbilityEvent(AbilityType.VampireBite, player, target);
        MiraEventManager.InvokeEvent(touAbilityEvent);

        target.ChangeRole(RoleId.Get<VampireRole>());
        target.AddModifier<VampireBittenModifier>();

        if (OptionGroupSingleton<VampireOptions>.Instance.CanGuessAsNewVamp)
        {
            target.AddModifier<NeutralKillerAssassinModifier>();
        }
    }
}