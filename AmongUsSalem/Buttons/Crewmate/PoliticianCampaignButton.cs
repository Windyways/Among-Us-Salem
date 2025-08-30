using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using ObjectWorkshop.Modifiers.Crewmate;
using ObjectWorkshop.Options.Roles.Crewmate;
using ObjectWorkshop.Roles.Crewmate;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Crewmate;

public sealed class PoliticianCampaignButton : ObjectWorkshopRoleButton<PoliticianRole, PlayerControl>
{
    public override string Name => "Campaign";
    public override string Keybind => Keybinds.SecondaryAction;
    public override float Cooldown => OptionGroupSingleton<PoliticianOptions>.Instance.CampaignCooldown + MapCooldown;
    public override Color TextOutlineColor => OWColors.Politician;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.CampaignButtonSprite;

    public override bool CanUse()
    {
        return base.CanUse() && Role is { CanCampaign: true };
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: x => !x.HasModifier<PoliticianCampaignedModifier>());
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        Target?.RpcAddModifier<PoliticianCampaignedModifier>(PlayerControl.LocalPlayer);
    }
}