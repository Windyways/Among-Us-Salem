using System.Collections;
using Il2CppInterop.Runtime;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using ObjectWorkshop.Events;
using ObjectWorkshop.Modifiers;
using ObjectWorkshop.Modifiers.Game;
using ObjectWorkshop.Options.Modifiers.Alliance;
using ObjectWorkshop.Options.Roles.Crewmate;
using ObjectWorkshop.Roles;
using ObjectWorkshop.Roles.Crewmate;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Crewmate;

public sealed class SheriffShootButton : ObjectWorkshopRoleButton<SheriffRole, PlayerControl>, IKillButton
{
    public override string Name => "Shoot";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => OWColors.Sheriff;
    public override float Cooldown => OptionGroupSingleton<SheriffOptions>.Instance.KillCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.SheriffShootSprite;

    public bool Usable { get; set; } =
        OptionGroupSingleton<SheriffOptions>.Instance.FirstRoundUse || TutorialManager.InstanceExists;

    public bool FailedShot { get; set; }

    public override bool CanUse()
    {
        return base.CanUse() && Usable && !FailedShot;
    }

    private void Misfire()
    {
        if (Target == null)
        {
            Logger<ObjectWorkshopPlugin>.Error("Misfire: Target is null");
            return;
        }

        var missType = OptionGroupSingleton<SheriffOptions>.Instance.MisfireType;
        SheriffRole.RpcSheriffMisfire(PlayerControl.LocalPlayer);

        if (missType is MisfireOptions.Target or MisfireOptions.Both)
        {
            PlayerControl.LocalPlayer.RpcCustomMurder(Target);
        }

        if (missType is MisfireOptions.Sheriff or MisfireOptions.Both)
        {
            PlayerControl.LocalPlayer.RpcCustomMurder(PlayerControl.LocalPlayer);
            DeathHandlerModifier.RpcUpdateDeathHandler(PlayerControl.LocalPlayer, "Suicide", DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetTrue, lockInfo: DeathHandlerOverride.SetTrue);
        }

        FailedShot = true;

        var notif1 = Helpers.CreateAndShowNotification("<b>You have lost the ability to shoot!</b>", Color.white,
            spr: TouRoleIcons.Sheriff.LoadAsset());

        notif1.Text.SetOutlineThickness(0.35f);
        notif1.transform.localPosition = new Vector3(0f, 1f, -20f);

        Coroutines.Start(MiscUtils.CoFlash(OWColors.Infiltrator));
    }

    private static IEnumerator CoSetBodyReportable(byte bodyId)
    {
        var waitDelegate =
            DelegateSupport.ConvertDelegate<Il2CppSystem.Func<bool>>(() => Helpers.GetBodyById(bodyId) != null);
        yield return new WaitUntil(waitDelegate);
        var body = Helpers.GetBodyById(bodyId);

        if (body != null)
        {
            body.gameObject.layer = LayerMask.NameToLayer("Ship");
            body.Reported = true;
        }
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<ObjectWorkshopPlugin>.Error("Sheriff Shoot: Target is null");
            return;
        }

        if (Target.HasModifier<FirstDeadShield>())
        {
            return;
        }

        if (Target.HasModifier<BaseShieldModifier>())
        {
            return;
        }

        var alignment = RoleAlignment.CrewmateSupport;
        var options = OptionGroupSingleton<SheriffOptions>.Instance;

        if (Target.Data.Role is IOWRole touRole)
        {
            alignment = touRole.RoleAlignment;
        }
        else if (Target.IsImpostor())
        {
            alignment = RoleAlignment.InfiltratorSupport;
        }

        if (!(PlayerControl.LocalPlayer.TryGetModifier<AllianceGameModifier>(out var allyMod) &&
              !allyMod.GetsPunished) &&
            !(Target.TryGetModifier<AllianceGameModifier>(out var allyMod2) && !allyMod2.GetsPunished))
        {
            switch (alignment)
            {
                case RoleAlignment.None:
                    PlayerControl.LocalPlayer.RpcCustomMurder(Target);
                    break;

                case RoleAlignment.NeutralAssociative:
                case RoleAlignment.CrewmateInvestigative:
                case RoleAlignment.CrewmateKilling:
                case RoleAlignment.CrewmateProtective:
                case RoleAlignment.CrewmateSupport:
                    Misfire();
                    break;

                case RoleAlignment.NeutralPredator:
                    if (!options.ShootNeutralKiller)
                    {
                        Misfire();
                    }
                    else
                    {
                        PlayerControl.LocalPlayer.RpcCustomMurder(Target);
                    }

                    break;

                case RoleAlignment.NeutralEvil:
                    if (!options.ShootNeutralEvil)
                    {
                        Misfire();
                    }
                    else
                    {
                        PlayerControl.LocalPlayer.RpcCustomMurder(Target);
                    }

                    break;
                default:
                    Misfire();
                    break;
            }
        }
        else
        {
            PlayerControl.LocalPlayer.RpcCustomMurder(Target);
        }

        if (!OptionGroupSingleton<SheriffOptions>.Instance.SheriffBodyReport)
        {
            Coroutines.Start(CoSetBodyReportable(Target.PlayerId));
        }
    }

    public override PlayerControl? GetTarget()
    {
        if (!OptionGroupSingleton<LoversOptions>.Instance.LoversKillEachOther && PlayerControl.LocalPlayer.IsLover())
        {
            return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, false, x => !x.IsLover());
        }
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }
}