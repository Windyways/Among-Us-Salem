using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using AmongUsSalem.Modifiers.Neutral;
using AmongUsSalem.Options.Roles.Neutral;
using AmongUsSalem.Roles.Neutral;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Buttons.Neutral;

public sealed class MercenaryGuardButton : AmongUsSalemRoleButton<MercenaryRole, PlayerControl>
{
    public override string Name => "Guard";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => AUSColors.Mercenary;
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
            Logger<AUSPlugin>.Error("Mercenary Guard: Target is null");
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