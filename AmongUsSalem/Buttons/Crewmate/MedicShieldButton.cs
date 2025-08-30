using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using ObjectWorkshop.Options.Roles.Crewmate;
using ObjectWorkshop.Roles.Crewmate;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Crewmate;

public sealed class MedicShieldButton : ObjectWorkshopRoleButton<MedicRole, PlayerControl>
{
    public bool CanChangeTarget = OptionGroupSingleton<MedicOptions>.Instance.ChangeTarget;
    public override string Name => "Shield";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Medic;
    public override int MaxUses => OptionGroupSingleton<MedicOptions>.Instance.ChangeTarget ? (int)OptionGroupSingleton<MedicOptions>.Instance.MedicShieldUses : 0;
    public override float Cooldown => 0.001f + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.MedicSprite;

    public override bool CanUse()
    {
        return base.CanUse() && (Role is { Shielded: null } || CanChangeTarget);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<ObjectWorkshopPlugin>.Error("Medic Shield: Target is null");
            return;
        }

        MedicRole.RpcMedicShield(PlayerControl.LocalPlayer, Target);
        CanChangeTarget = false;
    }
}