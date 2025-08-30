using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using ObjectWorkshop.Modifiers.Neutral;
using ObjectWorkshop.Options.Roles.Neutral;
using ObjectWorkshop.Roles.Neutral;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Neutral;

public sealed class MercenaryGuardButton : ObjectWorkshopRoleButton<MercenaryRole, PlayerControl>
{
    public override string Name => "Guard";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Mercenary;
    public override float Cooldown => OptionGroupSingleton<MercenaryOptions>.Instance.GuardCooldown + MapCooldown;
    public override int MaxUses => (int)OptionGroupSingleton<MercenaryOptions>.Instance.MaxUses;
    public override LoadableAsset<Sprite> Sprite => TouNeutAssets.GuardSprite;

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && !PlayerControl.LocalPlayer.Data.IsDead;
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<ObjectWorkshopPlugin>.Error("Mercenary Guard: Target is null");
            return;
        }

        Target.RpcAddModifier<MercenaryGuardModifier>(PlayerControl.LocalPlayer);
        var notif1 = Helpers.CreateAndShowNotification(
            $"<b>Once {Target.Data.PlayerName} is interacted with, you will get one gold.</b>", Color.white,
            new Vector3(0f, 1f, -20f), spr: TouRoleIcons.Mercenary.LoadAsset());
        notif1.Text.SetOutlineThickness(0.35f);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: x => !x.HasModifier<MercenaryGuardModifier>());
    }
}