using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using AmongUsSalem.Modifiers.Neutral;
using AmongUsSalem.Options.Roles.Neutral;
using AmongUsSalem.Roles.Neutral;
using UnityEngine;

namespace AmongUsSalem.Buttons.Neutral;

public sealed class GuardianAngelProtectButton : AmongUsSalemRoleButton<GuardianAngelTouRole>
{
    public override string Name => "Protect";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => AUSColors.GuardianAngel;
    public override float Cooldown => OptionGroupSingleton<GuardianAngelOptions>.Instance.ProtectCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<GuardianAngelOptions>.Instance.ProtectDuration;
    public override int MaxUses => (int)OptionGroupSingleton<GuardianAngelOptions>.Instance.MaxProtects;
    public override LoadableAsset<Sprite> Sprite => TouNeutAssets.ProtectSprite;

    protected override void OnClick()
    {
        Role.Target?.RpcAddModifier<GuardianAngelProtectModifier>(PlayerControl.LocalPlayer);
    }
}