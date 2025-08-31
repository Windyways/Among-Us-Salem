using System.Collections;
using System.Text;
using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using AmongUsSalem.Buttons.Neutral;
using AmongUsSalem.Modifiers.Neutral;
using AmongUsSalem.Options.Roles.Neutral;
using AmongUsSalem.Roles.Crewmate;
using AmongUsSalem.Utilities;
using UnityEngine;
using Random = System.Random;

namespace AmongUsSalem.Roles.Neutral;

public sealed class PlaguebearerRole(IntPtr cppPtr)
    : NeutralRole(cppPtr), IAUSRole, IDoomable, ICrewVariant
{
    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;
    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not PlaguebearerRole || Player.HasDied() || !Player.AmOwner)
        {
            return;
        }

        var allInfected =
            ModifierUtils.GetPlayersWithModifier<PlaguebearerInfectedModifier>([HideFromIl2Cpp](x) =>
                x.PlagueBearerId == Player.PlayerId && !x.Player.HasDied());

        if (allInfected.Count() >= Helpers.GetAlivePlayers().Count - 1 &&
            (!MeetingHud.Instance || Helpers.GetAlivePlayers().Count > 2))
        {
            var players =
                ModifierUtils.GetPlayersWithModifier<PlaguebearerInfectedModifier>([HideFromIl2Cpp](x) =>
                    x.PlagueBearerId == Player.PlayerId);

            players.Do(x =>
                x.RpcRemoveModifier<PlaguebearerInfectedModifier>());

            Player.RpcChangeRole(RoleId.Get<PestilenceRole>());

            CustomButtonSingleton<PestilenceKillButton>.Instance.SetTimer(OptionGroupSingleton<PlaguebearerOptions>
                .Instance.PestKillCooldown);
        }
    }

    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<AurialRole>());
    public DoomableType DoomHintType => DoomableType.Fearmonger;
    public string RoleName => TouLocale.Get(TouNames.Plaguebearer, "Plaguebearer");
    public string RoleDescription => "Infect Everyone To Become <color=#4D4D4DFF>Pestilence</color>";
    public string RoleLongDescription => "Infect everyone to become <color=#4D4D4DFF>Pestilence</color>";
    public Color RoleColor => AUSColors.Plaguebearer;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public Alignment Alignment => Alignment.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Phantom),
        Icon = TouRoleIcons.Plaguebearer,
        MaxRoleCount = 1,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>()
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = IAUSRole.SetNewTabText(this);

        var allInfected = PlayerControl.AllPlayerControls.ToArray().Where(x =>
            !x.HasDied() && x != Player &&
            x.GetModifier<PlaguebearerInfectedModifier>()?.PlagueBearerId == Player.PlayerId);

        if (allInfected.Any())
        {
            stringB.Append("\n<b>Players Infected:</b>");
            foreach (var plr in allInfected)
            {
                stringB.Append(AUSPlugin.Culture, $"\n{Color.white.ToTextColor()}{plr.Data.PlayerName}</color>");
            }
        }

        var notInfected = PlayerControl.AllPlayerControls.ToArray().Where(x =>
            !x.HasDied() && x != Player && !x.HasModifier<PlaguebearerInfectedModifier>());
        stringB.Append(AUSPlugin.Culture, $"\n\n<b>Players Left To Infect: {notInfected.Count()}</b>");

        return stringB;
    }

    public bool WinConditionMet()
    {
        if (Player.HasDied())
        {
            return false;
        }

        var result = Helpers.GetAlivePlayers().Count <= 2 && MiscUtils.KillersAliveCount() == 1;

        return result;
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Neutral Killing role that needs to infect all other players to turn into the Pestilence." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Infect",
            "Infect a player, causing them to be infected. When a infected player or dead body interacts or get interacted with the infection will spread to all non-infected players.",
            TouNeutAssets.InfectSprite)
    ];

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner && (int)OptionGroupSingleton<PlaguebearerOptions>.Instance.PestChance != 0)
        {
            Coroutines.Start(CheckForPestChance(Player));
        }
    }

    private static IEnumerator CheckForPestChance(PlayerControl player)
    {
        yield return new WaitForSeconds(0.01f);

        Random rnd = new();
        var chance = rnd.Next(1, 101);

        if (chance <= OptionGroupSingleton<PlaguebearerOptions>.Instance.PestChance)
        {
            player.RpcChangeRole(RoleId.Get<PestilenceRole>());
            CustomButtonSingleton<PestilenceKillButton>.Instance.SetTimer(OptionGroupSingleton<PlaguebearerOptions>
                .Instance.PestKillCooldown);
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

    public static void CheckInfected(PlayerControl source, PlayerControl target)
    {
        if (source.Data.Role is PlaguebearerRole)
        {
            target.AddModifier<PlaguebearerInfectedModifier>(source.PlayerId);
        }
        else if (target.Data.Role is PlaguebearerRole)
        {
            source.AddModifier<PlaguebearerInfectedModifier>(target.PlayerId);
        }
        else if (source.TryGetModifier<PlaguebearerInfectedModifier>(out var mod) &&
                 !target.HasModifier<PlaguebearerInfectedModifier>())
        {
            target.AddModifier<PlaguebearerInfectedModifier>(mod.PlagueBearerId);
        }
        else if (target.TryGetModifier<PlaguebearerInfectedModifier>(out var mod2) &&
                 !source.HasModifier<PlaguebearerInfectedModifier>())
        {
            source.AddModifier<PlaguebearerInfectedModifier>(mod2.PlagueBearerId);
        }
    }

    [MethodRpc((uint)AUSRpc.CheckInfected, SendImmediately = true)]
    public static void RpcCheckInfected(PlayerControl source, PlayerControl target)
    {
        CheckInfected(source, target);
    }
}