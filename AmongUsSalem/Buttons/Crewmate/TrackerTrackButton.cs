using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using AmongUsSalem.Modifiers.Crewmate;
using AmongUsSalem.Options.Roles.Crewmate;
using AmongUsSalem.Roles.Crewmate;
using AmongUsSalem.Utilities;
using AmongUsSalem.Utilities.Appearances;
using UnityEngine;

namespace AmongUsSalem.Buttons.Crewmate;

public sealed class TrackerTrackButton : AmongUsSalemRoleButton<TrackerTouRole, PlayerControl>
{
    public override string Name => "Track";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => AUSColors.Tracker;
    public override float Cooldown => OptionGroupSingleton<TrackerOptions>.Instance.TrackCooldown + MapCooldown;
    public override int MaxUses => (int)OptionGroupSingleton<TrackerOptions>.Instance.MaxTracks;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.TrackSprite;
    public int ExtraUses { get; set; }

    public override bool IsTargetValid(PlayerControl? target)
    {
        return base.IsTargetValid(target) && !target!.HasModifier<TrackerArrowTargetModifier>();
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<AUSPlugin>.Error("Track: Target is null");
            return;
        }

        Color color = Palette.PlayerColors[Target.GetDefaultAppearance().ColorId];
        var update = OptionGroupSingleton<TrackerOptions>.Instance.UpdateInterval;

        Target.AddModifier<TrackerArrowTargetModifier>(PlayerControl.LocalPlayer, color, update);

        TouAudio.PlaySound(TouAudio.TrackerActivateSound);
    }
}