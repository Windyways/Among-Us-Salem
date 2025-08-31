using MiraAPI.GameOptions;
using AmongUsSalem.Options.Roles.Crewmate;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Modifiers.Crewmate;

public sealed class SnitchImpostorRevealModifier()
    : RevealModifier((int)ChangeRoleResult.Nothing, true, null!)
{
    public override string ModifierName => "Revealed Impostor";
    
    public override void OnActivate()
    {
        base.OnActivate();
        SetNewInfo(false, null,null, null, AUSColors.Mafia);
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
            NameColor = AUSColors.Mafia;
        }
        else if (!Player.HasDied())
        {
            NameColor = Player.Data.Role.TeamColor;
        }
    }
}