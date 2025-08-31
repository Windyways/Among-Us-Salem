using MiraAPI.GameOptions;
using MiraAPI.Networking;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using AmongUsSalem.Options.Modifiers.Alliance;
using AmongUsSalem.Options.Roles.Crewmate;
using AmongUsSalem.Roles.Crewmate;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Buttons.Crewmate;

public sealed class MirrorcasterUnleashButton : AmongUsSalemRoleButton<MirrorcasterRole, PlayerControl>, IDiseaseableButton, IKillButton
{
    public override string Name => "Unleash";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Mirrorcaster;
    public override float Cooldown => OptionGroupSingleton<MirrorcasterOptions>.Instance.UnleashCooldown.Value + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.UnleashSprite;

    public void SetDiseasedTimer(float multiplier)
    {
        SetTimer(Cooldown * multiplier);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<AUSPlugin>.Error("Mirrorcaster Unleash: Target is null");
            return;
        }

        PlayerControl.LocalPlayer.RpcCustomMurder(Target);
        MirrorcasterRole.RpcMirrorcasterUnleash(PlayerControl.LocalPlayer);
    }

    public override PlayerControl? GetTarget()
    {
        if (!OptionGroupSingleton<LoversOptions>.Instance.LoversKillEachOther && PlayerControl.LocalPlayer.IsLover())
        {
            return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, false, x => !x.IsLover());
        }
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        if (Role.UnleashesAvailable <= 0)
        {
            return false;
        }

        return base.IsTargetValid(target);
    }
}