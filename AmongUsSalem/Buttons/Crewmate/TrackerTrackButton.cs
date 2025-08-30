using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using ObjectWorkshop.Modifiers.Crewmate;
using ObjectWorkshop.Options.Roles.Crewmate;
using ObjectWorkshop.Roles.Crewmate;
using ObjectWorkshop.Utilities;
using ObjectWorkshop.Utilities.Appearances;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Crewmate;

public sealed class TrackerTrackButton : ObjectWorkshopRoleButton<TrackerTouRole, PlayerControl>
{
    public override string Name => "Track";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Tracker;
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
            Logger<ObjectWorkshopPlugin>.Error("Track: Target is null");
            return;
        }

        Color color = Palette.PlayerColors[Target.GetDefaultAppearance().ColorId];
        var update = OptionGroupSingleton<TrackerOptions>.Instance.UpdateInterval;

        Target.AddModifier<TrackerArrowTargetModifier>(PlayerControl.LocalPlayer, color, update);

        TouAudio.PlaySound(TouAudio.TrackerActivateSound);
    }
}