using MiraAPI.GameOptions;
using MiraAPI.Networking;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using ObjectWorkshop.Options.Modifiers.Alliance;
using ObjectWorkshop.Options.Roles.Neutral;
using ObjectWorkshop.Roles.Neutral;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Neutral;

public sealed class WerewolfKillButton : ObjectWorkshopRoleButton<WerewolfRole, PlayerControl>, IDiseaseableButton,
    IKillButton
{
    public override string Name => "Kill";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => OWColors.Werewolf;
    public override float Cooldown => OptionGroupSingleton<WerewolfOptions>.Instance.RampageKillCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TouNeutAssets.WerewolfKillSprite;

    public void SetDiseasedTimer(float multiplier)
    {
        SetTimer(Cooldown * multiplier);
    }

    public override bool Enabled(RoleBehaviour? role)
    {
        return role is WerewolfRole { Rampaging: true };
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<ObjectWorkshopPlugin>.Error("Werewolf Shoot: Target is null");
            return;
        }

        PlayerControl.LocalPlayer.RpcCustomMurder(Target);
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