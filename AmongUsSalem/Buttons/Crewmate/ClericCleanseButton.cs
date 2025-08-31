using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using AmongUsSalem.Modifiers.Crewmate;
using AmongUsSalem.Options.Roles.Crewmate;
using AmongUsSalem.Roles.Crewmate;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Buttons.Crewmate;

public sealed class ClericCleanseButton : AmongUsSalemRoleButton<ClericRole, PlayerControl>
{
    public override string Name => "Cleanse";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Cleric;
    public override float Cooldown => OptionGroupSingleton<ClericOptions>.Instance.CleanseCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.CleanseSprite;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<AUSPlugin>.Error($"{Name}: Target is null");
            return;
        }

        if (Target.HasModifier<ClericCleanseModifier>())
        {
            Target.RpcRemoveModifier<ClericCleanseModifier>();
        }

        Target.RpcAddModifier<ClericCleanseModifier>(PlayerControl.LocalPlayer);

        if (ClericCleanseModifier.FindNegativeEffects(Target).Count > 0)
        {
            Coroutines.Start(MiscUtils.CoFlash(AUSColors.Cleric));
        }

        CustomButtonSingleton<ClericBarrierButton>.Instance.ResetCooldownAndOrEffect();
    }
}