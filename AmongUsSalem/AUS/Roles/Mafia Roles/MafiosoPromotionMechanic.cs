using AmongUsSalem.Patches;
using AmongUsSalem.Patches.Options;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement.GameMechanics;

public static class MafiosoPromotionMechanic
{
    public static bool GodfatherDiedWithMafiosoAlive;
    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            return; // Only run when round starts.
        }

        /*if (PlayerControl.LocalPlayer.Is(Faction.Mafia))
        {
            TeamChatPatches.ToggleTeamChat();
        }*/

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            var promotionCandidates =
                PlayerControl.AllPlayerControls.ToArray()
                .Where(x => !x.HasDied() && x.Is(Faction.Mafia))
                .OrderBy(x => !x.Is(Alignment.MafiaKilling)).FirstOrDefault();

            var aliveMafiosos = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.IsRole<Mafioso>());
            //var aliveGodfathers = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.IsRole<Godfather>());

            /*if (aliveGodfathers == 0 && GodfatherDiedWithMafiosoAlive && player.Is(Faction.Mafia) && player.IsRole<Mafioso>())
            {
                player.RpcChangeRole(RoleId.Get<Godfather>());
                if (player.AmOwner())
                {
                    MiscUtils.ShowNotification(Godfather.Info(Godfather.Type.Promoted), Color.white, AUSAssets.GodfatherRoleCard.LoadAsset());
                    MiscUtils.AddFakeChat(player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Mafia, "Godfather Info"), Godfather.Info(Godfather.Type.Promoted));
                }
                GodfatherDiedWithMafiosoAlive = false;
            }
            else */if (/*aliveGodfathers == 0 && */aliveMafiosos == 0 && player.Is(Faction.Mafia) && promotionCandidates != null)
            {
                promotionCandidates.RpcChangeRole(RoleId.Get<Mafioso>());
                if (promotionCandidates.AmOwner())
                {
                    MiscUtils.ShowNotification(Mafioso.Info(), Color.white, AUSAssets.MafiosoRoleCard.LoadAsset());
                    MiscUtils.AddFakeChat(promotionCandidates.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Mafia, "Mafioso Info"), Mafioso.Info());
                }
            }
        }
    }
}