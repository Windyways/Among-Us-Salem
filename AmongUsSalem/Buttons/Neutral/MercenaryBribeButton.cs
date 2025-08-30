using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using ObjectWorkshop.Modifiers.Neutral;
using ObjectWorkshop.Roles.Neutral;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Neutral;

public sealed class MercenaryBribeButton : ObjectWorkshopRoleButton<MercenaryRole, PlayerControl>
{
    public override string Name => "Bribe";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => OWColors.Mercenary;
    public override float Cooldown => 0.001f + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TouNeutAssets.BribeSprite;

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && role is MercenaryRole && !PlayerControl.LocalPlayer.Data.IsDead && Role.CanBribe;
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<ObjectWorkshopPlugin>.Error("Mercenary Bribed: Target is null");
            return;
        }

        Target.RpcAddModifier<MercenaryBribedModifier>(PlayerControl.LocalPlayer);
        var notif1 = Helpers.CreateAndShowNotification(
            $"<b>If {Target.Data.PlayerName} wins, you will win as well.</b>", Color.white, new Vector3(0f, 1f, -20f),
            spr: TouRoleIcons.Mercenary.LoadAsset());
        notif1.Text.SetOutlineThickness(0.35f);

        Role.Gold -= MercenaryRole.BrideCost;

        SetActive(false, Role);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: x => !x.HasModifier<MercenaryBribedModifier>());
    }
}