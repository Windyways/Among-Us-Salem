using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using ObjectWorkshop.Options.Roles.Crewmate;
using ObjectWorkshop.Roles.Crewmate;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Crewmate;

public sealed class DetectiveExamineButton : ObjectWorkshopRoleButton<DetectiveRole, PlayerControl>
{
    public override string Name => "Examine";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => OWColors.Detective;
    public override float Cooldown => OptionGroupSingleton<DetectiveOptions>.Instance.ExamineCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.ExamineSprite;

    public override bool CanUse()
    {
        return base.CanUse() && Role.InvestigatingScene;
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        Role.ExaminePlayer(Target);
    }
}