using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using ObjectWorkshop.Modifiers.Crewmate;
using ObjectWorkshop.Options.Modifiers.Alliance;
using ObjectWorkshop.Options.Roles.Crewmate;
using ObjectWorkshop.Roles.Crewmate;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Crewmate;

public sealed class HunterStalkButton : ObjectWorkshopRoleButton<HunterRole, PlayerControl>
{
    public override string Name => "Stalk";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Hunter;
    public override float Cooldown => OptionGroupSingleton<HunterOptions>.Instance.HunterStalkCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<HunterOptions>.Instance.HunterStalkDuration;
    public override int MaxUses => (int)OptionGroupSingleton<HunterOptions>.Instance.StalkUses;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.StalkButtonSprite;
    public int ExtraUses { get; set; }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<ObjectWorkshopPlugin>.Error("Stalk: Target is null");
            return;
        }

        var notif1 = Helpers.CreateAndShowNotification(
            $"<b>If {Target.Data.PlayerName} uses an ability, you will be able to kill them at any time in the round.</b>",
            Color.white, new Vector3(0f, 1f, -20f), spr: TouRoleIcons.Hunter.LoadAsset());
        notif1.Text.SetOutlineThickness(0.35f);

        Target.RpcAddModifier<HunterStalkedModifier>(PlayerControl.LocalPlayer);
        OverrideName("Stalking");
    }

    public override void OnEffectEnd()
    {
        OverrideName("Stalk");
    }

    public override PlayerControl? GetTarget()
    {
        if (!OptionGroupSingleton<LoversOptions>.Instance.LoversKillEachOther && PlayerControl.LocalPlayer.IsLover())
        {
            return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, false, x => !x.IsLover());
        }
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }
}