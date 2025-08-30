using MiraAPI.GameOptions;
using ObjectWorkshop.Options.Roles.Crewmate;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Modifiers.Crewmate;

public sealed class SnitchImpostorRevealModifier()
    : RevealModifier((int)ChangeRoleResult.Nothing, true, null!)
{
    public override string ModifierName => "Revealed Impostor";
    
    public override void OnActivate()
    {
        base.OnActivate();
        SetNewInfo(false, null,null, null, OWColors.Infiltrator);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!OptionGroupSingleton<SnitchOptions>.Instance.SnitchSeesImpostorsMeetings)
        {
            Visible = !MeetingHud.Instance;
        }

        if (Player.IsImpostor())
        {
            NameColor = OWColors.Infiltrator;
        }
        else if (!Player.HasDied())
        {
            NameColor = Player.Data.Role.TeamColor;
        }
    }
}