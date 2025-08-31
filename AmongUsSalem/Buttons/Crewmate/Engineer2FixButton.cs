using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities.Extensions;
using AmongUsSalem.Options.Roles.Crewmate;
using AmongUsSalem.Roles.Crewmate;
using UnityEngine;

namespace AmongUsSalem.Buttons.Crewmate;

public sealed class EngineerFixButton : AmongUsSalemRoleButton<EngineerTouRole>
{
    public override string Name => "Fix";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => AUSColors.Engineer;
    public override float Cooldown => 0.001f + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<EngineerOptions>.Instance.FixDelay + 0.01f;
    public override int MaxUses => (int)OptionGroupSingleton<EngineerOptions>.Instance.MaxFixes;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.FixButtonSprite;
    public override bool ShouldPauseInVent => false;

    protected override void FixedUpdate(PlayerControl playerControl)
    {
        Button?.cooldownTimerText.gameObject.SetActive(false);
    }

    public override bool CanUse()
    {
        var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();

        return base.CanUse() && system is { AnyActive: true };
    }

    protected override void OnClick()
    {
        OverrideName("Fixing");
        var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();

        if (system is not { AnyActive: true })
        {
            ResetCooldownAndOrEffect();
        }
    }

    public override void OnEffectEnd()
    {
        OverrideName("Fix");
        var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();

        if (system is { AnyActive: true })
        {
            List<LoadableAsset<AudioClip>> audio = [TouAudio.EngiFix1, TouAudio.EngiFix2, TouAudio.EngiFix3];
            TouAudio.PlaySound(audio.Random()!, 4f);
            EngineerTouRole.EngineerFix(PlayerControl.LocalPlayer);
        }
    }
}