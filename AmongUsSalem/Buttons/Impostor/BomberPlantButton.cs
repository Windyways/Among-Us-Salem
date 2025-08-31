using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using AmongUsSalem.Options.Roles.Impostor;
using AmongUsSalem.Roles.Impostor;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Buttons.Impostor;

public sealed class BomberPlantButton : AmongUsSalemRoleButton<BomberRole>, IAftermathableButton, IDiseaseableButton
{
    public override string Name => "Place";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => AUSColors.Mafia;
    public override float Cooldown => PlayerControl.LocalPlayer.GetKillCooldown() + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<BomberOptions>.Instance.DetonateDelay;
    public override int MaxUses => (int)OptionGroupSingleton<BomberOptions>.Instance.MaxBombs;
    public override LoadableAsset<Sprite> Sprite => TouImpAssets.PlaceSprite;

    public void SetDiseasedTimer(float multiplier)
    {
        SetTimer(Cooldown * multiplier);
    }

    protected override void OnClick()
    {
        OverrideSprite(TouImpAssets.DetonatingSprite.LoadAsset());
        OverrideName("Detonating");

        PlayerControl.LocalPlayer.killTimer = EffectDuration + 1f;

        BomberRole.RpcPlantBomb(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.transform.position);
    }

    public override void OnEffectEnd()
    {
        OverrideSprite(TouImpAssets.PlaceSprite.LoadAsset());
        OverrideName("Place");

        PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.LocalPlayer.GetKillCooldown());

        Role.Bomb?.Detonate();
    }
}