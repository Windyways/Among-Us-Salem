using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using AmongUsSalem.Modifiers.Crewmate;
using AmongUsSalem.Roles.Crewmate;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Buttons.Crewmate;

public sealed class CampButton : AmongUsSalemRoleButton<DeputyRole, PlayerControl>
{
    public bool Usable = true;
    public override string Name => "Camp";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => AUSColors.Deputy;
    public override float Cooldown => 0.001f + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.CampButtonSprite;

    public override bool CanUse()
    {
        return base.CanUse() && Usable;
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        return base.IsTargetValid(target) && !target?.HasModifier<DeputyCampedModifier>() == true;
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<AUSPlugin>.Error("Camp: Target is null");
            return;
        }

        var player = ModifierUtils.GetPlayersWithModifier<DeputyCampedModifier>(x => x.Deputy.AmOwner).FirstOrDefault();

        if (player != null)
        {
            player.RpcRemoveModifier<DeputyCampedModifier>();
        }

        Target.RpcAddModifier<DeputyCampedModifier>(PlayerControl.LocalPlayer);
        Usable = false;
        var notif1 = Helpers.CreateAndShowNotification(
            $"<b>Wait for {Target.Data.PlayerName}'s death so you can avenge them in the meeting.</b>", Color.white,
            new Vector3(0f, 1f, -20f), spr: TouRoleIcons.Deputy.LoadAsset());
        notif1.Text.SetOutlineThickness(0.35f);
    }
}