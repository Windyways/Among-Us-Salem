using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using ObjectWorkshop.Modules.Components;
using ObjectWorkshop.Roles.Crewmate;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Crewmate;

public sealed class DetectiveInspectButton : ObjectWorkshopRoleButton<DetectiveRole, CrimeSceneComponent>
{
    public override string Name => "Inspect";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Detective;
    public override float Cooldown => 1f + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.InspectSprite;

    public override CrimeSceneComponent? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetNearestObjectOfType<CrimeSceneComponent>(Distance,
            Helpers.CreateFilter(Constants.NotShipMask));
    }

    public override void SetOutline(bool active)
    {
        // placeholder
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        Role.InvestigatingScene = Target;
        Role.InvestigatedPlayers.AddRange(Target.GetScenePlayers());
        var notif1 = Helpers.CreateAndShowNotification(
            $"<b>{OWColors.Detective.ToTextColor()}You have inspected the crime scene of {Target.DeadPlayer!.Data.PlayerName}. The killer or anyone that steps foot in the crime scene will flash red when examined.</b></color>",
            Color.white, new Vector3(0f, 1f, -20f), spr: TouRoleIcons.Detective.LoadAsset());
        notif1.Text.SetOutlineThickness(0.35f);
        // TouAudio.PlaySound(TouAudio.QuestionSound);
    }
}