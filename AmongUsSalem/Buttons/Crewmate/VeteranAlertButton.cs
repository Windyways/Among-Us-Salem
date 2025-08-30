using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using ObjectWorkshop.Modifiers.Crewmate;
using ObjectWorkshop.Options.Roles.Crewmate;
using ObjectWorkshop.Roles.Crewmate;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Crewmate;

public sealed class VeteranAlertButton : ObjectWorkshopRoleButton<VeteranRole>
{
    public override string Name => "Alert";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Veteran;
    public override float Cooldown => OptionGroupSingleton<VeteranOptions>.Instance.AlertCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<VeteranOptions>.Instance.AlertDuration;
    public override int MaxUses => (int)OptionGroupSingleton<VeteranOptions>.Instance.MaxNumAlerts;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.AlertSprite;
    public int ExtraUses { get; set; }

    protected override void OnClick()
    {
        PlayerControl.LocalPlayer.RpcAddModifier<VeteranAlertModifier>();
        OverrideName("Alerting");
    }

    public override void OnEffectEnd()
    {
        OverrideName("Alert");
    }
}