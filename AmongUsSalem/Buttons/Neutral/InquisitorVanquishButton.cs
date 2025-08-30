using MiraAPI.GameOptions;
using MiraAPI.Networking;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using ObjectWorkshop.Options.Roles.Neutral;
using ObjectWorkshop.Roles.Neutral;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Neutral;

public sealed class InquisitorVanquishButton : ObjectWorkshopRoleButton<InquisitorRole, PlayerControl>, IDiseaseableButton,
    IKillButton
{
    public override string Name => "Vanquish";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => OWColors.Inquisitor;
    public override float Cooldown => OptionGroupSingleton<InquisitorOptions>.Instance.VanquishCooldown;
    public override LoadableAsset<Sprite> Sprite => TouNeutAssets.InquisKillSprite;

    public void SetDiseasedTimer(float multiplier)
    {
        SetTimer(Cooldown * multiplier);
    }

    public override bool CanUse()
    {
        return base.CanUse() && Role.CanVanquish;
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<ObjectWorkshopPlugin>.Error("Inquisitor Vanquish: Target is null");
            return;
        }

        PlayerControl.LocalPlayer.RpcCustomMurder(Target);
    }
}