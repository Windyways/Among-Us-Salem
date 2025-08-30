using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using ObjectWorkshop.Modifiers.Impostor;
using ObjectWorkshop.Options.Roles.Impostor;
using ObjectWorkshop.Roles.Impostor;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Impostor;

public sealed class BlackmailerBlackmailButton : ObjectWorkshopRoleButton<BlackmailerRole, PlayerControl>,
    IAftermathablePlayerButton
{
    public override string Name => "Blackmail";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Infiltrator;
    public override float Cooldown => OptionGroupSingleton<BlackmailerOptions>.Instance.BlackmailCooldown;
    public override int MaxUses => (int)OptionGroupSingleton<BlackmailerOptions>.Instance.MaxBlackmails;
    public override LoadableAsset<Sprite> Sprite => TouImpAssets.BlackmailSprite;

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        BlackmailerRole.RpcBlackmail(PlayerControl.LocalPlayer, Target);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, false,
            player => !player.HasModifier<BlackmailedModifier>() && !player.HasModifier<BlackmailSparedModifier>());
    }
}