using MiraAPI.GameOptions;
using MiraAPI.Networking;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using ObjectWorkshop.Options.Modifiers.Alliance;
using ObjectWorkshop.Options.Roles.Neutral;
using ObjectWorkshop.Roles.Neutral;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Neutral;

public sealed class GlitchKillButton : ObjectWorkshopRoleButton<GlitchRole, PlayerControl>, IDiseaseableButton, IKillButton
{
    public override string Name => "Kill";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => OWColors.Glitch;
    public override float Cooldown => OptionGroupSingleton<GlitchOptions>.Instance.KillCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TouNeutAssets.GlitchKillSprite;
    public override bool ShouldPauseInVent => false;

    public void SetDiseasedTimer(float multiplier)
    {
        SetTimer(Cooldown * multiplier);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<ObjectWorkshopPlugin>.Error("Glitch Shoot: Target is null");
            return;
        }

        PlayerControl.LocalPlayer.RpcCustomMurder(Target);
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