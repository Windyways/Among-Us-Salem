using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using ObjectWorkshop.Modifiers.Impostor;
using ObjectWorkshop.Options.Roles.Impostor;
using ObjectWorkshop.Roles.Impostor;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Impostor;

public sealed class AmbusherAmbushButton : ObjectWorkshopRoleButton<AmbusherRole, PlayerControl>, IKillButton, IDiseaseableButton
{
    public override string Name => "Ambush";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Infiltrator;
    public override float Cooldown => PlayerControl.LocalPlayer.GetKillCooldown() + MapCooldown;
    public override int MaxUses => (int)OptionGroupSingleton<AmbusherOptions>.Instance.MaxAmbushes;
    public override LoadableAsset<Sprite> Sprite => TouImpAssets.AmbushSprite;

    public void SetDiseasedTimer(float multiplier)
    {
        SetTimer(Cooldown * multiplier);
    }
    
    public override PlayerControl? GetTarget()
    {
        return Role.Pursued?.GetClosestLivingPlayer(false, Distance);
    }

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && Role is not { Pursued: null };
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }
        AmbusherRole.RpcAmbushPlayer(PlayerControl.LocalPlayer, Target);
        if (OptionGroupSingleton<AmbusherOptions>.Instance.ResetAmbush)
        {
            Role.Pursued = null;
            CustomButtonSingleton<AmbusherPursueButton>.Instance.SetActive(true, Role);
            CustomButtonSingleton<AmbusherPursueButton>.Instance.ResetCooldownAndOrEffect();
            SetActive(false, Role);
            ModifierUtils.GetActiveModifiers<AmbusherArrowTargetModifier>().Do(x => x.ModifierComponent?.RemoveModifier(x));
        }
    }
}