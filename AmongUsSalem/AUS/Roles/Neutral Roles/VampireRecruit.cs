using AmongUs.GameOptions;
using AmongUsSalem.Patches;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
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
        if (Player.Data.Role is ICustomAURole ausRole)
        {
            ausRole.RoleColor = AUSColors.Vampire;
            ausRole.Faction = Faction.Neutral;
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
            // Ensures that Mayor being promoted still shows only if revealed!
            // Although it wont have the extra votes so everyone is gonna know lol.
            if (Player.HasModifier<GlobalReveal>())
            {
                if (Player.Data.Role is ICustomAURole customRole)
                {
                   // deepfake.roleColor = AUSColors.Town; // Only Town can GlobalReveal so far, so this will do for now.
                    Player.RpcAddModifier<DeepfakeRole>(customRole.RoleName, AUSColors.Town);
                }
            }

            if (Player.AmOwner())
            {
                Player.RpcChangeRole(RoleId.Get<Vampire>());
                var button = CustomButtonSingleton<Vampire_Convert>.Instance;
                button.DecreaseUses(3 + Owner.Charges);
            }

            Player.RpcRemoveModifier<VampireRecruit>();
        }
    }

    public override bool? DidWin(GameOverReason reason)
    {
        var aliveVampires = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.IsRole<Vampire>());
        var aliveRecs = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.HasModifier<VampireRecruit>());
        if (aliveVampires == 0 && aliveRecs == 0) return false;

        var result = MiscUtils.GetAlivePlayersToEnd().Count <= (aliveVampires + aliveRecs) && MiscUtils.KillersAliveCount() == (aliveVampires + aliveRecs);

        if (aliveVampires > 0)
        {
            foreach (var vampires in MiscUtils.GetPlayersWithRole<Vampire>())
            {
                var vampire = vampires.GetRole<Jackal>();
                return result || LogicGameFlowPatches.EndGameEarlyCheck(vampire);
            }
        }

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