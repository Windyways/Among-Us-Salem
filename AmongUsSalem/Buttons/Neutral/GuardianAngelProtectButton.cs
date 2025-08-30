using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using ObjectWorkshop.Modifiers.Neutral;
using ObjectWorkshop.Options.Roles.Neutral;
using ObjectWorkshop.Roles.Neutral;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Neutral;

public sealed class GuardianAngelProtectButton : ObjectWorkshopRoleButton<GuardianAngelTouRole>
{
    public override string Name => "Protect";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.GuardianAngel;
    public override float Cooldown => OptionGroupSingleton<GuardianAngelOptions>.Instance.ProtectCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<GuardianAngelOptions>.Instance.ProtectDuration;
    public override int MaxUses => (int)OptionGroupSingleton<GuardianAngelOptions>.Instance.MaxProtects;
    public override LoadableAsset<Sprite> Sprite => TouNeutAssets.ProtectSprite;

    protected override void OnClick()
    {
        Role.Target?.RpcAddModifier<GuardianAngelProtectModifier>(PlayerControl.LocalPlayer);
    }
}