using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AmongUsSalem.LifeImprovement.Roles;

#region JackalRecruit
#endregion
public sealed class JackalRecruit(PlayerControl otherRec) : AllianceGameModifier
{
    public override string ModifierName => "Jackal Rec";
    public override bool Unique => false;
    public override bool HideOnUi => true;
    public override bool CrewContinuesGame => true;
    public PlayerControl OtherRecruit = otherRec;

    public override void OnActivate()
    {
        if (Player.Data.Role is IAUSRole ausRole)
        {
            ausRole.RoleName = AUSColors.GradientColorText("404040", "b8b8b8", ausRole.RoleName);
        }
    }

    public override bool? DidWin(GameOverReason reason)
    {
        var aliveJackals = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.IsRole<Jackal>());
        var aliveRecs = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.HasModifier<JackalRecruit>());
        if (aliveJackals == 0 && aliveRecs == 0) return false;

        var result = MiscUtils.GetAlivePlayersToEnd().Count <= (aliveJackals + aliveRecs) && MiscUtils.KillersAliveCount() == (aliveJackals + aliveRecs);
        return result;
    }

    public override int GetAssignmentChance()
    {
        return 0;
    }

    public override int GetAmountPerGame()
    {
        return 0;
    }
}