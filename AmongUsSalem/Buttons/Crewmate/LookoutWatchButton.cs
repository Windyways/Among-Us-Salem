using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using AmongUsSalem.Modifiers.Crewmate;
using AmongUsSalem.Options.Roles.Crewmate;
using AmongUsSalem.Roles.Crewmate;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Buttons.Crewmate;

public sealed class WatchButton : AmongUsSalemRoleButton<LookoutRole, PlayerControl>
{
    public override string Name => "Watch";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => AUSColors.Lookout;
    public override float Cooldown => OptionGroupSingleton<LookoutOptions>.Instance.WatchCooldown + MapCooldown;
    public override int MaxUses => (int)OptionGroupSingleton<LookoutOptions>.Instance.MaxWatches;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.WatchSprite;
    public int ExtraUses { get; set; }

    public override bool IsTargetValid(PlayerControl? target)
    {
        return base.IsTargetValid(target) && !target!.HasModifier<LookoutWatchedModifier>(x => x.Lookout.AmOwner);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<AUSPlugin>.Error("Watch: Target is null");
            return;
        }

        Target.RpcAddModifier<LookoutWatchedModifier>(PlayerControl.LocalPlayer);

        var notif1 = Helpers.CreateAndShowNotification(
            $"<b>You will know what roles interact with {Target.Data.PlayerName} if they are not dead by next meeting.</b>",
            Color.white, new Vector3(0f, 1f, -20f), spr: TouRoleIcons.Lookout.LoadAsset());
        notif1.Text.SetOutlineThickness(0.35f);
    }
}