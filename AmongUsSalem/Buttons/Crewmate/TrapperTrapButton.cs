using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using ObjectWorkshop.Modules;
using ObjectWorkshop.Options.Roles.Crewmate;
using ObjectWorkshop.Roles.Crewmate;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Crewmate;

public sealed class TrapperTrapButton : ObjectWorkshopRoleButton<TrapperRole>
{
    public override string Name => "Trap";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Trapper;
    public override float Cooldown => OptionGroupSingleton<TrapperOptions>.Instance.TrapCooldown + MapCooldown;
    public override int MaxUses => (int)OptionGroupSingleton<TrapperOptions>.Instance.MaxTraps;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.TrapSprite;
    public int ExtraUses { get; set; }

    protected override void OnClick()
    {
        var role = PlayerControl.LocalPlayer.GetRole<TrapperRole>();

        if (role == null)
        {
            return;
        }

        var pos = PlayerControl.LocalPlayer.transform.position;
        pos.z += 0.001f;

        Trap.CreateTrap(role, pos);

        TouAudio.PlaySound(TouAudio.TrapperPlaceSound);
    }
}