using UnityEngine;

namespace AmongUsSalem.LifeImprovement.GameMechanics;

public static class MafiosoPromotionMechanic
{
    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            return; // Only run when round starts.
        }

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            // later update this so no one can be promoted with an active Godfather.
            var aliveMafiosos = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.IsRole<Mafioso>());
            if (aliveMafiosos == 0 && player.Is(Faction.Mafia) && !player.IsRole<Mafioso>())
            {
                player.RpcChangeRole(RoleId.Get<Mafioso>());
                if (player.AmOwner())
                {
                    MiscUtils.ShowNotification(Mafioso.Info(), Color.white, AUSAssets.MafiosoRoleCard.LoadAsset());
                    MiscUtils.AddFakeChat(player.CachedPlayerData, "Mafioso Info", Mafioso.Info());
                }
            }
        }
    }
}