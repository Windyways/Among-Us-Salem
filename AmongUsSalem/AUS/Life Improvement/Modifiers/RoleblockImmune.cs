using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AmongUsSalem.LifeImprovement.Roles;

#region RBimmune
#endregion
public sealed class RBimmune(bool perma) : TouGameModifier
{
    public override string ModifierName => "Roleblock Immunity";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public bool permanent = perma;

    public override void OnMeetingStart()
    {
        if (!permanent) Player.RpcRemoveModifier<RBimmune>();
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

public static class RBimmune_Events
{
    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        if (!@event.TriggeredByIntro)
        {
            return; // Only run when game starts.
        }

        var player = PlayerControl.LocalPlayer;
        if (player.IsRole<Escort>()/* || player.IsRole<Consort>()*/)
        {
            player.RpcAddModifier<RBimmune>(true);
        }
    }
}