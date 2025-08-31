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
            var aliveMafiosos = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.IsRole<Mafioso>());
            if (aliveMafiosos == 0 && player.Is(Faction.Mafia) && !player.IsRole<Mafioso>())
            {
                player.RpcChangeRole(RoleId.Get<Mafioso>());
                if (player.AmOwner())
                {
                    MiscUtils.ShowNotification("You were promoted to a <b><color=#dd0000>Mafioso</color></b>!", Color.white, AUSAssets.MafiosoRoleCard.LoadAsset());
                    MiscUtils.AddFakeChat(player.CachedPlayerData, "Mafioso Info", "You were promoted to a <b><color=#dd0000>Mafioso</color></b>!");
                }
            }
        }
    }
}