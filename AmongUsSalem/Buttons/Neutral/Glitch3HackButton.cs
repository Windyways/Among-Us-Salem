using MiraAPI.GameOptions;
using MiraAPI.Hud;
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

public sealed class GlitchHackButton : ObjectWorkshopRoleButton<GlitchRole, PlayerControl>, IAftermathablePlayerButton
{
    public override string Name => "Hack";
    public override string Keybind => "tou.ActionCustom";
    public override Color TextOutlineColor => OWColors.Glitch;
    public override float Cooldown => OptionGroupSingleton<GlitchOptions>.Instance.HackCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TouNeutAssets.HackSprite;
    public override ButtonLocation Location => ButtonLocation.BottomRight;
    public override bool ShouldPauseInVent => false;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<ObjectWorkshopPlugin>.Error("Glitch Hack: Target is null");
            return;
        }

        var notif1 = Helpers.CreateAndShowNotification(
            $"<b>Once {Target.Data.PlayerName} attempts to use an ability, all their abilities will get disabled.</b>",
            Color.white, new Vector3(0f, 1f, -20f), spr: TouRoleIcons.Glitch.LoadAsset());
        notif1.Text.SetOutlineThickness(0.35f);

        TouAudio.PlaySound(TouAudio.HackedSound);
        Target.RpcAddModifier<GlitchHackedModifier>(PlayerControl.LocalPlayer.PlayerId);
    }
}