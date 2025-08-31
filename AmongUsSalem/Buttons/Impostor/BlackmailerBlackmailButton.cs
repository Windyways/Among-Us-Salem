using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using AmongUsSalem.Modifiers.Impostor;
using AmongUsSalem.Options.Roles.Impostor;
using AmongUsSalem.Roles.Impostor;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Buttons.Impostor;

public sealed class BlackmailerBlackmailButton : AmongUsSalemRoleButton<BlackmailerRole, PlayerControl>,
    IAftermathablePlayerButton
{
    public override string Name => "Blackmail";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => AUSColors.Mafia;
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