using UnityEngine;

namespace AmongUsSalem.MCI.SmartMCI;

public static class WitnessKill
{
    [RegisterEvent]
    public static void EjectionEvent(EjectionEvent @event)
    {
        NetworkedPlayerInfo exiled = @event.ExileController.initData.networkedPlayer;
        if (exiled != null)
        {
            PlayerControl player = exiled.Object;
            if (player.TryGetModifier<ComparedModifier>(out var compared))
            {
                if (player.Is(Faction.Town)) compared.comparedTo.AddModifier<ConfirmedEvil>();
            }
        }
    }

    [RegisterEvent]
    public static void AfterMurderEvent(AfterMurderEvent @event)
    {
        var killer = @event.Source;
        var target = @event.Target;
        TryWitness(killer, target);
    }

    [RegisterEvent]
    public static void EnterVentEvent(EnterVentEvent @event)
    {
        var venter = @event.Player;
        TryWitness(venter, venter);
    }

    [RegisterEvent]
    public static void ExitVentEvent(ExitVentEvent @event)
    {
        var venter = @event.Player;
        TryWitness(venter, venter);
    }

    public static void TryWitness(PlayerControl killer, PlayerControl target)
    {
        foreach (var witness in PlayerControl.AllPlayerControls)
        {
            if (!IgnoreKill(killer, witness) && BotCanSeeKill(killer, witness, target.transform.position) && witness != target)
            {
                if (killer.IsRole<Bodyguard>()) killer.AddModifier<Confirmed>();
                else
                {
                    // Add murder see modifier here.
                    var modifier = witness.AddModifier<SeenKill>();
                    if (modifier != null) modifier.killer = killer;

                    var modifier2 = killer.AddModifier<SeenKill>();
                    if (modifier2 != null) modifier2.killer = witness;
                }
            }
        }
    }

    public static bool IgnoreKill(PlayerControl killer, PlayerControl witness)
    {
        if (killer.Is(Faction.Mafia) && witness.Is(Faction.Mafia)) return true;
        if (killer.Is(Faction.Coven) && witness.Is(Faction.Coven)) return true;
        return false;
    }

    public static bool BotCanSeeKill(PlayerControl killer, PlayerControl witness, Vector3 killPos)
    {
        // 1) distance
        float dist = Vector3.Distance(witness.transform.position, killPos);
        float baseVision = GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;

        // impostor bots maybe get impostor mod — you decide:
        if (witness.Is(Faction.Mafia)) baseVision = GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod;
        //else if (witness.Data.Role is ICustomAURole customRole && witness.Is(Faction.Neutral)) baseVision = customRole.visionValue;

        // scale vision by map lighting (lights sabotage etc.)
        float vision = baseVision * ShipStatus.Instance.CalculateLightRadius(witness.Data);

        if (dist > vision)
            return false;

        var vector = witness.GetTruePosition() - killer.GetTruePosition();
        var magnitude = vector.magnitude;

        if (PhysicsHelpers.AnyNonTriggersBetween(killer.GetTruePosition(), vector.normalized, magnitude, Constants.ShipAndObjectsMask))
            return false;

        AUSPlugin.DebugLogMessage($"{witness.Name()} has witnessed {killer.Name()} do something bad!");
        return true;
    }
}