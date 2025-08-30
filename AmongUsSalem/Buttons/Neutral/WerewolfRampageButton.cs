using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Utilities.Assets;
using ObjectWorkshop.Options.Roles.Neutral;
using ObjectWorkshop.Roles.Neutral;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Neutral;

public sealed class WerewolfRampageButton : ObjectWorkshopRoleButton<WerewolfRole>, IAftermathableButton
{
    public override string Name => "Rampage";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Werewolf;
    public override float Cooldown => OptionGroupSingleton<WerewolfOptions>.Instance.RampageCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<WerewolfOptions>.Instance.RampageDuration;
    public override LoadableAsset<Sprite> Sprite => TouNeutAssets.RampageSprite;

    public override bool CanUse()
    {
        return base.CanUse() && Role?.Rampaging == false;
    }

    protected override void OnClick()
    {
        if (Role == null)
        {
            return;
        }

        Role.Rampaging = true;

        CustomButtonSingleton<WerewolfKillButton>.Instance.SetActive(true, Role);
        CustomButtonSingleton<WerewolfKillButton>.Instance.SetTimer(0.01f);
        TouAudio.PlaySound(TouAudio.WerewolfRampageSound);
    }

    public override void OnEffectEnd()
    {
        if (Role == null)
        {
            return;
        }

        Role.Rampaging = false;

        CustomButtonSingleton<WerewolfKillButton>.Instance.SetActive(false, Role);
    }
}