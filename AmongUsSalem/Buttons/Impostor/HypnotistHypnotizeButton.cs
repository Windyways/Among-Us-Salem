using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using ObjectWorkshop.Modifiers.Impostor;
using ObjectWorkshop.Options.Roles.Impostor;
using ObjectWorkshop.Roles.Impostor;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Impostor;

public sealed class HypnotistHypnotizeButton : ObjectWorkshopRoleButton<HypnotistRole, PlayerControl>,
    IAftermathablePlayerButton
{
    public override string Name => "Hypnotize";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Infiltrator;
    public override float Cooldown => OptionGroupSingleton<HypnotistOptions>.Instance.HypnotiseCooldown;
    public override LoadableAsset<Sprite> Sprite => TouImpAssets.HypnotiseButtonSprite;

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && role is HypnotistRole hypno && !hypno.HysteriaActive;
    }

    public override bool CanUse()
    {
        return base.CanUse() && !Role.HysteriaActive;
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        Target.RpcAddModifier<HypnotisedModifier>(PlayerControl.LocalPlayer);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(false, Distance, false,
            player => !player.HasModifier<HypnotisedModifier>());
    }
}