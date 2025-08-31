using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using AmongUsSalem.Modifiers.Crewmate;
using AmongUsSalem.Options.Roles.Crewmate;
using AmongUsSalem.Roles.Crewmate;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Buttons.Crewmate;

public sealed class PoliticianCampaignButton : AmongUsSalemRoleButton<PoliticianRole, PlayerControl>
{
    public override string Name => "Campaign";
    public override string Keybind => Keybinds.SecondaryAction;
    public override float Cooldown => OptionGroupSingleton<PoliticianOptions>.Instance.CampaignCooldown + MapCooldown;
    public override Color TextOutlineColor => AUSColors.Politician;
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