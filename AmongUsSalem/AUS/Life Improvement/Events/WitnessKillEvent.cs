using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Hud;
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

        WitnessSuspiciousActivity(source, target, true, false, true);

        CalculatedVoting.EvidenceAgainst.Remove(target);
        CalculatedVoting.KillerContagious.Remove(target);
        CalculatedVoting.QueueEvidenceAgainst.Remove(target);
        CalculatedVoting.QueueKillerContagious.Remove(target);
    }

    public static void WitnessSuspiciousActivity(PlayerControl killer, PlayerControl target, bool murder, bool incriminating = false, bool caught = false)
    {
        foreach (PlayerControl players in PlayerControl.AllPlayerControls)
        {
            Vector2 truePosition = players.GetTruePosition();
            float maxDistance = GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod * 3.8f;
            float closestDistance = float.MaxValue;

            if (Vector2.Distance(truePosition, target.GetTruePosition()) <= maxDistance)
            {
                float distance = Vector2.Distance(truePosition, target.GetTruePosition());

                if (distance < closestDistance && players != target && players != killer && !players.HasDied())
                {
                    CallFind(killer, target, players, caught, incriminating);
                }
            }
        }
    }

    public static void CallFind(PlayerControl killer, PlayerControl target, PlayerControl players, bool caught, bool incriminating)
    {
        if (
            !(killer.Is(Faction.Mafia) && !players.Is(Faction.Mafia)) && 
            !(killer.Is(Faction.Coven) && !players.Is(Faction.Coven)) && 
            !(killer.Is(Faction.Traitor) && !players.Is(Faction.Traitor)) && 
            !(killer.IsRole<Jackal>() && !players.HasModifier<JackalRecruit>()) && 
            !(killer.HasModifier<JackalRecruit>() && !players.IsRole<Jackal>()) && 
            !(killer.IsRole<Vampire>() && !players.HasModifier<VampireRecruit>()) && 
            !(killer.HasModifier<VampireRecruit>() && !players.IsRole<Vampire>())
            ) {
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
            CalculatedVoting.QueueEvidenceAgainst.Add(target, killer);
        }
        else
        {
            CalculatedVoting.QueueKillerContagious.Add(target, killer);
        }
    }

    public static void ResetButtonTimer(PlayerControl source, CustomActionButton? button = null, float cooldown = 0f)
    {
        button?.SetTimer(cooldown);

        if (!source.AmOwner() || !source.IsImpostor())
        {
            return;
        }

        source.SetKillTimer(cooldown);
    }

    public static void ResetCooldown()
    {
        PlayerControl.LocalPlayer.SetKillTimer(0f);
        foreach (var button in CustomButtonManager.Buttons.Where(x => x.Enabled(PlayerControl.LocalPlayer.Data.Role)))
        {
            button.SetTimer(button.InitialCooldown);
        }
    }
}