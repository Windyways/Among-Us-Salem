using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using AmongUsSalem.Modifiers.Crewmate;
using AmongUsSalem.Options.Roles.Crewmate;
using AmongUsSalem.Roles.Crewmate;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Buttons.Crewmate;

public sealed class JailorJailButton : AmongUsSalemRoleButton<JailorRole, PlayerControl>
{
    public override string Name => "Jail";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => AUSColors.Jailor;
    public override float Cooldown => OptionGroupSingleton<JailorOptions>.Instance.JailCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.JailSprite;

    public bool ExecutedACrew { get; set; }

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && !ExecutedACrew;
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: player => !player.HasModifier<JailedModifier>() && !player.HasModifier<JailSparedModifier>());
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        ModifierUtils.GetPlayersWithModifier<JailedModifier>().Do(x => x.RpcRemoveModifier<JailedModifier>());
        Target?.RpcAddModifier<JailedModifier>(PlayerControl.LocalPlayer.PlayerId);
        TouAudio.PlaySound(TouAudio.JailSound);
    }
}