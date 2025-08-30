using BepInEx.Unity.IL2CPP.Utils.Collections;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using ObjectWorkshop.Modifiers.Impostor;
using ObjectWorkshop.Options.Roles.Impostor;
using ObjectWorkshop.Roles.Impostor;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Impostor;

public sealed class GrenadierFlashButton : ObjectWorkshopRoleButton<GrenadierRole>, IAftermathableButton
{
    public override string Name => "Flash";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Infiltrator;
    public override float Cooldown => OptionGroupSingleton<GrenadierOptions>.Instance.GrenadeCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<GrenadierOptions>.Instance.GrenadeDuration;
    public override int MaxUses => (int)OptionGroupSingleton<GrenadierOptions>.Instance.MaxFlashes;
    public override LoadableAsset<Sprite> Sprite => TouImpAssets.FlashSprite;

    protected override void OnClick()
    {
        var flashRadius = OptionGroupSingleton<GrenadierOptions>.Instance.FlashRadius;
        var flashedPlayers =
            Helpers.GetClosestPlayers(PlayerControl.LocalPlayer, flashRadius * ShipStatus.Instance.MaxLightRadius);

        foreach (var player in flashedPlayers)
        {
            player.RpcAddModifier<GrenadierFlashModifier>(PlayerControl.LocalPlayer);
        }

        PlayerControl.LocalPlayer.RpcAddModifier<GrenadierFlashModifier>(PlayerControl.LocalPlayer);
        var notif1 = Helpers.CreateAndShowNotification(
            $"<b>{OWColors.Infiltrator.ToTextColor()}All players around you are now flashbanged!</color></b>", Color.white,
            spr: TouRoleIcons.Grenadier.LoadAsset());
        
        notif1.Text.SetOutlineThickness(0.35f);
        notif1.transform.localPosition = new Vector3(0f, 1f, -150f);

        Coroutines.Start(
            Effects.Shake(HudManager.Instance.PlayerCam.transform, 0.2f, 0.1f, true, true).WrapToManaged());

        SoundManager.Instance.PlaySound(TouAudio.GrenadeSound.LoadAsset(), false);
    }
}