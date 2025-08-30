using MiraAPI.Hud;
using MiraAPI.Utilities.Assets;
using ObjectWorkshop.Roles.Impostor;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Impostor;

public sealed class EscapistMarkButton : ObjectWorkshopRoleButton<EscapistRole>, IAftermathableButton
{
    public override string Name => "Mark Location";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Infiltrator;
    public override float Cooldown => 0.001f;
    public override float InitialCooldown => 0.001f;
    public override LoadableAsset<Sprite> Sprite => TouImpAssets.MarkSprite;

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && Role is { MarkedLocation: null };
    }

    public override bool CanUse()
    {
        return base.CanUse() && Role is { MarkedLocation: null };
    }

    protected override void OnClick()
    {
        EscapistRole.RpcMarkLocation(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.transform.position);

        // TouAudio.PlaySound(TouAudio.EscapistMarkSound);
        CustomButtonSingleton<EscapistRecallButton>.Instance.SetActive(true, Role);
        CustomButtonSingleton<EscapistRecallButton>.Instance.ResetCooldownAndOrEffect();
        SetActive(false, Role);
    }
}