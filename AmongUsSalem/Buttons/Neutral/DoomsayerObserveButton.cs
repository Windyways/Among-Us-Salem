using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using AmongUsSalem.Modifiers.Neutral;
using AmongUsSalem.Options.Roles.Neutral;
using AmongUsSalem.Roles.Neutral;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Buttons.Neutral;

public sealed class DoomsayerObserveButton : AmongUsSalemRoleButton<DoomsayerRole, PlayerControl>
{
    public override string Name => "Observe";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => AUSColors.Doomsayer;
    public override float Cooldown => OptionGroupSingleton<DoomsayerOptions>.Instance.ObserveCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TouNeutAssets.Observe;

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && !OptionGroupSingleton<DoomsayerOptions>.Instance.CantObserve;
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: x => !x.HasModifier<DoomsayerObservedModifier>());
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        ModifierUtils.GetPlayersWithModifier<DoomsayerObservedModifier>()
            .Do(x => x.RemoveModifier<DoomsayerObservedModifier>());

        Target.AddModifier<DoomsayerObservedModifier>();

        var notif1 = Helpers.CreateAndShowNotification(
            $"<b>{AUSColors.Doomsayer.ToTextColor()}You will recieve a report on {Target.Data.PlayerName} during the next meeting. This will help you narrow down their role.</color></b>",
            Color.white, spr: TouRoleIcons.Doomsayer.LoadAsset());

        notif1.Text.SetOutlineThickness(0.35f);
        notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
    }
}