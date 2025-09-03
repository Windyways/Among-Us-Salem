using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AmongUsSalem.LifeImprovement.Roles;

#region VampireRecruit
#endregion
public sealed class VampireRecruit(int num) : AllianceGameModifier
{
    public override string ModifierName => "Vampire Rec";
    public override bool Unique => false;
    public override bool HideOnUi => true;
    public override bool CrewContinuesGame => true;
    public int id = num;
    public Vampire Owner;

    public override void OnActivate()
    {
        if (Player.Data.Role is IAUSRole ausRole)
        {
            ausRole.RoleColor = AUSColors.Vampire;
            ausRole.RoleFaction = Faction.Neutral;
        }
    }

    public void LowerIds()
    {
        id--;
    }

    public void Promote()
    {
        if (id == 0)
        {
            Player.RpcChangeRole(RoleId.Get<Vampire>());
            if (Player.AmOwner)
            {
                var button = CustomButtonSingleton<Vampire_Convert>.Instance;
                button.UsesLeft = Owner.Charges;
            }

            Player.RpcRemoveModifier<VampireRecruit>();
        }
    }

    public override bool? DidWin(GameOverReason reason)
    {
        var aliveJackals = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.IsRole<Jackal>());
        var aliveRecs = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.HasModifier<JackalRecruit>());
        if (aliveJackals == 0 && aliveRecs == 0) return false;

        var result = Helpers.GetAlivePlayers().Count <= (aliveJackals + aliveRecs) && MiscUtils.KillersAliveCount() == (aliveJackals + aliveRecs);
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