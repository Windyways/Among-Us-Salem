using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using AmongUsSalem.Events.TouEvents;
using AmongUsSalem.Modules.Anims;
using AmongUsSalem.Options;
using AmongUsSalem.Options.Roles.Impostor;
using AmongUsSalem.Roles.Crewmate;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Roles.Impostor;

public sealed class EscapistRole(IntPtr cppPtr)
    : ImpostorRole(cppPtr), IAUSRole, IDoomable, ICrewVariant
{
    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;
    public Vector2? MarkedLocation { get; set; }
    public GameObject? EscapeMark { get; set; }

    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not EscapistRole || Player.HasDied())
        {
            return;
        }

        if (EscapeMark != null)
        {
            EscapeMark.SetActive(PlayerControl.LocalPlayer.IsImpostor() || (PlayerControl.LocalPlayer.HasDied() &&
                                                                            OptionGroupSingleton<GeneralOptions>
                                                                                .Instance.TheDeadKnow));
            if (MarkedLocation == null)
            {
                EscapeMark.gameObject.Destroy();
                EscapeMark = null;
            }
        }
    }

    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<TransporterRole>());
    public DoomableType DoomHintType => DoomableType.Protective;
    public string RoleName => TouLocale.Get(TouNames.Escapist, "Escapist");
    public string RoleDescription => "Get Away From Kills With Ease";
    public string RoleLongDescription => "Teleport to get away from the scene of the crime";
    public Color RoleColor => AUSColors.Mafia;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public Alignment Alignment => Alignment.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TouRoleIcons.Escapist,
        IntroSound = TouAudio.TimeLordIntroSound,
        CanUseVent = OptionGroupSingleton<EscapistOptions>.Instance.CanVent
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IAUSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is an Impostor Concealing role that can mark a location and then recall (teleport) to that location."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Mark",
            "Mark a location for later use.",
            TouImpAssets.MarkSprite),
        new("Recall",
            "Recall to the marked location.",
            TouImpAssets.RecallSprite)
    ];

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        EscapeMark?.gameObject.Destroy();
    }

    [MethodRpc((uint)AUSRpc.Recall, SendImmediately = true)]
    public static void RpcRecall(PlayerControl player)
    {
        if (player.Data.Role is not EscapistRole)
        {
            Logger<AUSPlugin>.Error("RpcRecall - Invalid escapist");
            return;
        }

        var touAbilityEvent = new TouAbilityEvent(AbilityType.EscapistRecall, player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
    }

    [MethodRpc((uint)AUSRpc.MarkLocation, SendImmediately = true)]
    public static void RpcMarkLocation(PlayerControl player, Vector2 pos)
    {
        if (player.Data.Role is not EscapistRole henry)
        {
            Logger<AUSPlugin>.Error("RpcRecall - Invalid escapist");
            return;
        }

        var touAbilityEvent = new TouAbilityEvent(AbilityType.EscapistMark, player);
        MiraEventManager.InvokeEvent(touAbilityEvent);

        henry.MarkedLocation = pos;
        henry.EscapeMark = AnimStore.SpawnAnimAtPlayer(player, TouAssets.EscapistMarkPrefab.LoadAsset());
        henry.EscapeMark.transform.localPosition = new Vector3(pos.x, pos.y + 0.3f, 0.1f);
        henry.EscapeMark.SetActive(false);
    }
}