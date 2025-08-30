using MiraAPI.GameOptions;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using ObjectWorkshop.Options.Roles.Impostor;
using ObjectWorkshop.Roles.Impostor;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Impostor;

public sealed class JanitorCleanButton : ObjectWorkshopRoleButton<JanitorRole, DeadBody>, IAftermathableBodyButton,
    IDiseaseableButton
{
    public override string Name => "Clean";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Infiltrator;
    public override float Cooldown => OptionGroupSingleton<JanitorOptions>.Instance.CleanCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<JanitorOptions>.Instance.CleanDelay + 0.001f;
    public override int MaxUses => (int)OptionGroupSingleton<JanitorOptions>.Instance.MaxClean;
    public override LoadableAsset<Sprite> Sprite => TouImpAssets.CleanButtonSprite;

    public DeadBody? CleaningBody { get; set; }

    public void SetDiseasedTimer(float multiplier)
    {
        SetTimer(Cooldown * multiplier);
    }

    public override DeadBody? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetNearestDeadBody(Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        CleaningBody = Target;
        OverrideName("Cleaning");
    }

    public override void OnEffectEnd()
    {
        OverrideName("Clean");
        if (CleaningBody == Target && CleaningBody != null)
        {
            JanitorRole.RpcCleanBody(PlayerControl.LocalPlayer, CleaningBody.ParentId);
            TouAudio.PlaySound(TouAudio.JanitorCleanSound);
        }

        CleaningBody = null;
        if (OptionGroupSingleton<JanitorOptions>.Instance.ResetCooldowns)
        {
            PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.LocalPlayer.GetKillCooldown());
        }
    }
}