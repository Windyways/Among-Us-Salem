using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using ObjectWorkshop.Modifiers.Crewmate;
using ObjectWorkshop.Options.Roles.Crewmate;
using ObjectWorkshop.Roles.Crewmate;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Crewmate;

public sealed class JailorJailButton : ObjectWorkshopRoleButton<JailorRole, PlayerControl>
{
    public override string Name => "Jail";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Jailor;
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