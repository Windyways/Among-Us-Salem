using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using ObjectWorkshop.Modifiers.Crewmate;
using ObjectWorkshop.Options.Roles.Crewmate;
using ObjectWorkshop.Roles.Crewmate;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ObjectWorkshop.Buttons.Crewmate;

public sealed class MediumMediateButton : ObjectWorkshopRoleButton<MediumRole>
{
    public override string Name => "Mediate";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Medium;
    public override float Cooldown => OptionGroupSingleton<MediumOptions>.Instance.MediateCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.MediateSprite;

    protected override void OnClick()
    {
        var deadPlayers = PlayerControl.AllPlayerControls.ToArray()
            .Where(plr => plr.Data.IsDead && !plr.Data.Disconnected &&
                          Object.FindObjectsOfType<DeadBody>().Any(x => x.ParentId == plr.PlayerId)
                          && !plr.HasModifier<MediatedModifier>()).ToList();

        if (deadPlayers.Count == 0)
        {
            return;
        }

        var targets = OptionGroupSingleton<MediumOptions>.Instance.WhoIsRevealed switch
        {
            MediateRevealedTargets.NewestDead => [deadPlayers[0]],
            MediateRevealedTargets.AllDead => deadPlayers,
            MediateRevealedTargets.OldestDead => [deadPlayers[^1]],
            MediateRevealedTargets.RandomDead => deadPlayers.Randomize(),
            _ => []
        };

        foreach (var plr in targets)
        {
            MediumRole.RpcMediate(PlayerControl.LocalPlayer, plr);
        }
    }
}