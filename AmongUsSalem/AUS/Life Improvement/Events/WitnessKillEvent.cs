using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Hud;
using AmongUsSalem.Buttons.Neutral;
using AmongUsSalem.Roles.Neutral;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement.Events;

public static class WitnessKillEvent
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;
        var target = @event.Target;
        if (!source.AmOwner || MeetingHud.Instance)
        {
            return;
        }

        bool deadViaSabotage = source != target;

        WitnessSuspiciousActivity(source, target, deadViaSabotage, true, false, true);
        RoleFunctionOnDeath(source, target);
    }

    public static void WitnessSuspiciousActivity(PlayerControl killer, PlayerControl target, bool deadViaSabotage, bool murder, bool incriminating = false, bool caught = false)
    {
        foreach (PlayerControl players in PlayerControl.AllPlayerControls)
        {
            Vector2 truePosition = players.GetTruePosition();
            float maxDistance = GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod * 3.8f;
            float closestDistance = float.MaxValue;

            if (Vector2.Distance(truePosition, target.GetTruePosition()) <= maxDistance)
            {
                float distance = Vector2.Distance(truePosition, target.GetTruePosition());
                float alternativeDist = Vector2.Distance(truePosition, target.GetTruePosition());

                if ((distance < closestDistance || alternativeDist < closestDistance) && players != target && players != killer && (!players.HasDied()/* || players.Is(RoleEnum.Astral)*/))
                {
                    CallFind(killer, target, players, caught, incriminating);
                }
            }
        }
    }


    public static void CallFind(PlayerControl killer, PlayerControl target, PlayerControl players, bool caught, bool incriminating)
    {
        if (killer.Is(Faction.Mafia) && !players.Is(Faction.Mafia))
        {
            string witnessedKiller = killer.GetDefaultAppearance().PlayerName;
            if (!caught)
            {
                WitnessNow(killer, players, incriminating);
            }
            AUSPlugin.DebugLogMessage(players.name + " has witnessed " + witnessedKiller + " killing!", AUSPlugin.MsgType.Message);
        }
    }

    public static void WitnessNow(PlayerControl killer, PlayerControl target, bool incriminating)
    {
        if (incriminating)
        {
            target.ApplyEvidenceAgainst(killer);
        }
        else
        {
            target.ApplyWitnessedKills(killer);
        }
    }

    public static void RoleFunctionOnDeath(PlayerControl killer, PlayerControl target)
    {
        foreach (PlayerControl players in PlayerControl.AllPlayerControls)
        {
            if (players.GetEvidenceAgainst().Contains(target))
            {
                players.RemoveEvidenceAgainst(target);
            }

            if (players.GetWitnessedKills().Contains(target))
            {
                players.RemoveWitnessedKills(target);
            }
        }
    }
}